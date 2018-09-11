﻿// module
var app = angular.module('app');
app.filter('shopOrderStatus', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case -1:
                s = "取消";
                break;
            case 0:
                s = "未处理";
                break;
            case 100:
                s = "已处理";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});
// controller
app.controller("shopOrderSetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {

    $scope.statusList = [{ name: "取消", value: -1 }, { name: "未处理", value: 0 }, { name: "已处理", value: 100 }];
    $scope.searchStatusList = [{ name: "", value: '' }, { name: "取消", value: -1 }, { name: "未处理", value: 0 }, { name: "已处理", value: 100 }];

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.shopOrderSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopOrderSetAPI", action, model, category);
        },
        getList: function (shopId) {
            return this.doAction("getList", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopId: shopId });
        },
        getOrderItems: function (orderId) {
            return this.doAction("getOrderItems", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, orderId: orderId });
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
        $scope.shopOrderSet.init(communityFlag, appFlag);
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.shopOrderSet.current = item;
        $scope.shopOrderSet.loadCurrentOrderItems();
    }

    $scope.selectShopBrand = function (item) {
        $scope.shopOrderSet.currentShopBrand = item;
        $scope.shopOrderSet.loadShops();
        $scope.shopOrderSet.load();
    }

    $scope.selectShop = function (item) {
        $scope.shopOrderSet.currentShop = item;
        $scope.shopOrderSet.load();
    }

    $scope.removeUpSkuFilter = function (item) {
        for (var i = 0; i < $scope.shopOrderSet.currentStocks.length; i++) {
            if ($scope.shopOrderSet.currentStocks[i].skuId == item.id) {
                return false;
            }
        }
        return true;
    }

    $scope.shopOrderSet = {
        items: [],//这个是商铺品牌的商品
        shopBrands: [],
        shops: [],
        searchModel: { communityFlag: "", appFlag: "" },
        newModel: { communityFlag: "", appFlag: "" },
        currentShopBrand: null,
        currentShop: null,
        current: null,
        currentOrderItems:[],
        init: function (communityFlag, appFlag) {
            this.searchModel.communityFlag = communityFlag;
            this.searchModel.appFlag = appFlag;

            this.loadShopBrands();
        },
        load: function () {
            var that = this;
            if (this.currentShop == null) return;

            return $scope.shopOrderSetAPI.getList(this.currentShop.id)
                .then(function (data) {
                    that.items = data.content.items;
                });
        },
        add: function () {
            var that = this;
            if (this.currentShop == null) return;
            this.newModel.shopId = this.currentShop.id;
            return $scope.shopOrderSetAPI.add(this.newModel)
                .then(function (data) {
                    $scope.showView("index");
                    that.load();
                });
        },
        update: function () {
            var that = this;
            this.current.communityFlag = $scope.communityFlag;
            this.current.appFlag = $scope.appFlag;
            $scope.shopOrderSetAPI.doAction("update", this.current)
                .then(function (data) {
                    $scope.showView("index");
                    that.load();
                });
        },
        setIsDelete: function (item) {
            var that = this;
            $scope.shopOrderSetAPI.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id })
                .then(function (data) {
                    that.load();
                });
        },
        setStatus: function () {
            var that = this;
            if (this.current == null) return;
            this.current.communityFlag = $scope.communityFlag;
            this.current.appFlag = $scope.appFlag;

            $scope.shopOrderSetAPI.doAction("setStatus", this.current)
                .then(function (data) {
                    that.load();
                });
        },
        setIsFinished: function (item) {
            var that = this;
            $scope.shopOrderSetAPI.doAction("setIsFinished", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id, isFinished: item.isFinished })
                .then(function (data) {
                    that.load();
                });
        },
        loadShopBrands: function () {
            var that = this;
            return $scope.shopOrderSetAPI.doAction("getShopBrands", that.searchModel)
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
            return $scope.shopOrderSetAPI.getShops(that.currentShopBrand.id)
                .then(function (data) {
                    that.shops = data.content.items;
                    if (that.shops.length > 0) {
                        that.currentShop = that.shops[0];
                        that.load();
                    }
                });
        },
        loadCurrentOrderItems: function () {
            if (this.current == null) return;
            var that = this;
            return $scope.shopOrderSetAPI.getOrderItems(that.current.id)
                .then(function (data) {
                    that.currentOrderItems = data.content;
                });
        }
    };
});