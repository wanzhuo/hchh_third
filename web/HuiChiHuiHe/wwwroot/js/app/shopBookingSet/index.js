// module
var app = angular.module('app');
app.filter('shopBookingStatus', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case -1:
                s = "取消";
                break;
            case 0:
                s = "待确认";
                break;
            case 1:
                s = "确认成功";
                break;
            case 4:
                s = "确认失败";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});
// controller
app.controller("shopBookingSetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {

    $scope.statusList = [{ name: "取消", value: -1 }, { name: "待确认", value: 0 }, { name: "确认成功", value: 1 }, { name: "确认失败", value: 4 }];
    $scope.searchStatusList = [{ name: "", value: '' }, { name: "取消", value: -1 }, { name: "待确认", value: 0 }, { name: "确认成功", value: 1 }, { name: "确认失败", value: 4 }];

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.shopBookingSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopBookingSetAPI", action, model, category);
        },
        getList: function (shopId) {
            return this.doAction("getList", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopId: shopId });
        },
        getShops: function (shopBrandId) {
            return this.doAction("getShops", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopBrandId: shopBrandId });
        },
        add: function (newModel) {
            newModel.communityFlag = $scope.communityFlag;
            newModel.appFlag = $scope.appFlag;

            return this.doAction("add", newModel);
        },
        getShopOpenStatus: function (shopId) {
            return this.doAction("getShopOpenStatus", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: shopId });
        },
        setShopOpenStatus: function (shopId, isOpen) {
            return this.doAction("setShopOpenStatus", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: shopId, isOpen: isOpen });
        }
    };

    $scope.shopBrandCommoditySetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopBrandCommoditySetAPI", action, model, category);
        }
    };

    //初始化
    $scope.init = function (communityFlag, appFlag) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;
        $scope.shopBookingSet.init(communityFlag, appFlag);
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.shopBookingSet.current = item;
    }

    $scope.selectShopBrand = function (item) {
        $scope.shopBookingSet.currentShopBrand = item;
        $scope.shopBookingSet.loadShops();
        $scope.shopBookingSet.load();
    }

    $scope.selectShop = function (item) {
        $scope.shopBookingSet.currentShop = item;
        $scope.shopBookingSet.load();
    }

    $scope.removeUpSkuFilter = function (item) {
        for (var i = 0; i < $scope.shopBookingSet.currentStocks.length; i++) {
            if ($scope.shopBookingSet.currentStocks[i].skuId == item.id) {
                return false;
            }
        }
        return true;
    }

    $scope.shopBookingSet = {
        items: [],//这个是商铺品牌的商品
        shopBrands: [],
        shops: [],
        searchModel: { communityFlag: "", appFlag: "" },
        newModel: { communityFlag: "", appFlag: "" },
        currentShopBrand: null,
        currentShop: null,
        init: function (communityFlag, appFlag) {
            this.searchModel.communityFlag = communityFlag;
            this.searchModel.appFlag = appFlag;

            this.loadShopBrands();
        },
        load: function () {
            var that = this;
            if (this.currentShop == null) return;

            return $scope.shopBookingSetAPI.getList(this.currentShop.id)
                .then(function (data) {
                    that.items = data.content.items;
                });
        },
        add: function () {
            var that = this;
            if (this.currentShop == null) return;
            this.newModel.shopId = this.currentShop.id;
            return $scope.shopBookingSetAPI.add(this.newModel)
                .then(function (data) {
                    $scope.showView("index");
                    that.load();
                });
        },
        update: function () {
            var that = this;
            this.current.communityFlag = $scope.communityFlag;
            this.current.appFlag = $scope.appFlag;
            $scope.shopBookingSetAPI.doAction("update", this.current)
                .then(function (data) {
                    $scope.showView("index");
                    that.load();
                });
        },
        setIsDelete: function (item) {
            var that = this;
            $scope.shopBookingSetAPI.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id })
                .then(function (data) {
                    that.load();
                });
        },
        setStatus: function () {
            var that = this;
            if (this.current == null) return;
            this.current.communityFlag = $scope.communityFlag;
            this.current.appFlag = $scope.appFlag;

            $scope.shopBookingSetAPI.doAction("setStatus", this.current)
                .then(function (data) {
                    that.load();
                });
        },
        setIsUsed: function (item) {
            var that = this;
            $scope.shopBookingSetAPI.doAction("setIsUsed", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id, isUsed: item.isUsed })
                .then(function (data) {
                    that.load();
                });
        },
        setQueueIndex:function (item) {
            var that = this;
            $scope.shopBookingSetAPI.doAction("setQueueIndex", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id, queueIndex: item.queueIndex + 3 })
                .then(function (data) {
                    that.load();
                });
        },
        loadShopBrands: function () {
            var that = this;
            return $scope.shopBookingSetAPI.doAction("getShopBrands", that.searchModel)
                .then(function (data) {
                    that.shopBrands = data.content.items;
                    if (that.shopBrands.length > 0) {
                        that.currentShopBrand = that.shopBrands[0];
                        that.loadShops();
                    }
                });
        },
        loadShops: function () {
            if (this.currentShopBrand == null) return;
            var that = this;
            return $scope.shopBookingSetAPI.getShops(that.currentShopBrand.id)
                .then(function (data) {
                    that.shops = data.content.items;
                    if (that.shops.length > 0) {
                        that.currentShop = that.shops[0];
                        that.load();
                    }
                });
        }
    };
});
