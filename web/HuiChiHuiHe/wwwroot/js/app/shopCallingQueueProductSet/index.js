// module
var app = angular.module('app');
app.filter('shopCallingQueueProductStatus', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case 0:
                s = "正常";
                break;
            case 1:
                s = "不开放";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});
// controller
app.controller("shopCallingQueueProductSetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {

    $scope.statusList = [{ name: "正常", value: 0 }, { name: "不开放", value: 1 }];
    $scope.searchStatusList = [{ name: "", value: '' }, { name: "正常", value: 0 }, { name: "不开放", value: 1 }];

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.shopCallingQueueProductSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopCallingQueueProductSetAPI", action, model, category);
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
        setShopOpenStatus: function (shopId,isOpen) {
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
        $scope.shopCallingQueueProductSet.init(communityFlag, appFlag);
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.shopCallingQueueProductSet.current = item;
    }

    $scope.selectShopBrand = function (item) {
        $scope.shopCallingQueueProductSet.currentShopBrand = item;
        $scope.shopCallingQueueProductSet.loadShops();
        $scope.shopCallingQueueProductSet.load();
    }

    $scope.selectShop = function (item) {
        $scope.shopCallingQueueProductSet.currentShop = item;
        $scope.shopCallingQueueProductSet.load();
    }

    $scope.removeUpSkuFilter = function (item) {
        for (var i = 0; i < $scope.shopCallingQueueProductSet.currentStocks.length; i++) {
            if ($scope.shopCallingQueueProductSet.currentStocks[i].skuId == item.id) {
                return false;
            }
        }
        return true;
    }

    $scope.shopCallingQueueProductSet = {
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

            return $scope.shopCallingQueueProductSetAPI.getList(this.currentShop.id)
                .then(function (data) {
                    that.items = data.content.items;
                    that.loadCurrentShopOpenStatus();
                });
        },
        add: function () {
            var that = this;
            if (this.currentShop == null) return;
            this.newModel.shopId = this.currentShop.id;
            return $scope.shopCallingQueueProductSetAPI.add(this.newModel)
                .then(function (data) {
                    $scope.showView("index");
                    that.load();
                });
        },
        update: function () {
            var that = this;
            this.current.communityFlag = $scope.communityFlag;
            this.current.appFlag = $scope.appFlag;
            $scope.shopCallingQueueProductSetAPI.doAction("update", this.current)
                .then(function (data) {
                    $scope.showView("index");
                    that.load();
                });
        },
        setIsDelete: function (item) {
            var that = this;
            $scope.shopCallingQueueProductSetAPI.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id })
                .then(function (data) {
                    that.load();
                });
        },
        setCurrentShopOpenStatus: function () {
            if (this.currentShop == null) return;
            var that = this;
            return $scope.shopCallingQueueProductSetAPI.setShopOpenStatus(that.currentShop.id, that.currentShop.openStatus)
                .then(function (data) {
                    that.loadCurrentShopOpenStatus();
                });
        },
        loadShopBrands: function () {
            var that = this;
            return $scope.shopCallingQueueProductSetAPI.doAction("getShopBrands", that.searchModel)
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
            return $scope.shopCallingQueueProductSetAPI.getShops(that.currentShopBrand.id)
                .then(function (data) {
                    that.shops = data.content.items;
                    if (that.shops.length > 0) {
                        that.currentShop = that.shops[0];
                        that.load();
                    }
                });
        },
        loadCurrentShopOpenStatus: function () {
            if (this.currentShop == null) return;
            var that = this;
            return $scope.shopCallingQueueProductSetAPI.getShopOpenStatus(that.currentShop.id)
                .then(function (data) {
                    that.currentShop.openStatus = data.content;
                });
        }
    };
});
