// module
var app = angular.module('app');
// controller
app.controller("shopCommodityStockSetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.shopCommodityStockSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopCommodityStockSetAPI", action, model, category);
        },
        getList: function (shopId, commodityId) {
            return this.doAction("getList", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopId: shopId, commodityId: commodityId });
        },
        getShops: function (shopBrandId) {
            return this.doAction("getShops", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopBrandId: shopBrandId });
        },
        getSkuItems: function (commodityId) {
            return this.doAction("getSkuItems", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, commodityId: commodityId });
        },
        add: function (shopId, commodityId, skuId) {
            return this.doAction("add", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopId: shopId, commodityId: commodityId, skuId: skuId });
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
        $scope.shopCommodityStockSet.init(communityFlag, appFlag);
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.shopCommodityStockSet.current = item;
        $scope.shopCommodityStockSet.loadSkuItems();
    }

    $scope.selectShopBrand = function (item) {
        $scope.shopCommodityStockSet.currentShopBrand = item;
        $scope.shopCommodityStockSet.loadShops();
        $scope.shopCommodityStockSet.loadShopBrandCommoditys();
    }

    $scope.selectShop = function (item) {
        $scope.shopCommodityStockSet.currentShop = item;
    }

    $scope.removeUpSkuFilter = function (item) {
        for (var i = 0; i < $scope.shopCommodityStockSet.currentStocks.length; i++) {
            if ($scope.shopCommodityStockSet.currentStocks[i].skuId == item.id) {
                return false;
            }
        }
        return true;
    }

    $scope.shopCommodityStockSet = {
        items: [],//这个是商铺品牌的商品
        shopBrands: [],
        shops: [],
        skuItems: [],
        skus: [],
        searchModel: { communityFlag: "", appFlag: "" },
        newModel: { communityFlag: "", appFlag: "" },
        currentShopBrand: null,
        currentShop: null,
        current: null,//当前选择的商品
        currentStocks: [],
        pageIndex: 0,
        pageSize: 20,
        init: function (communityFlag, appFlag) {
            this.searchModel.communityFlag = communityFlag;
            this.searchModel.appFlag = appFlag;

            this.newModel.communityFlag = communityFlag;
            this.newModel.appFlag = appFlag;
            this.loadShopBrands();
        },
        getSingleSku: function (skuId) {
            for (var i = 0; i < this.skus.length; i++) {
                var sku = this.skus[i];
                if (sku.id == skuId) return sku;
            }
        },
        load: function () {
            var that = this;
            if (this.currentShop == null) return;
            if (this.current == null) return;

            return $scope.shopCommodityStockSetAPI.getList(this.currentShop.id, this.current.id)
                .then(function (data) {
                    that.currentStocks = data.content.items;
                    for (var i = 0; i < that.currentStocks.length; i++) {
                        var stock = that.currentStocks[i];
                        stock.values = that.getSingleSku(stock.skuId).values;
                    }
                });
        },
        add: function (skuId) {
            var that = this;
            if (this.currentShop == null) return;
            if (this.current == null) return;

            return $scope.shopCommodityStockSetAPI.add(this.currentShop.id, this.current.id, skuId)
                .then(function (data) {
                    that.load();
                });
        },
        update: function (item) {
            var that = this;
            item.communityFlag = $scope.communityFlag;
            item.appFlag = $scope.appFlag;
            $scope.shopCommodityStockSetAPI.doAction("update", item)
                .then(function (data) {
                    that.load();
                });
        },
        setIsDelete: function (item) {
            var that = this;
            $scope.shopCommodityStockSetAPI.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id })
                .then(function (data) {
                    that.load();
                });
        },
        loadShopBrands: function () {
            var that = this;
            return $scope.shopCommodityStockSetAPI.doAction("getShopBrands", that.searchModel)
                .then(function (data) {
                    that.shopBrands = data.content.items;
                    if (that.shopBrands.length > 0) {
                        that.currentShopBrand = that.shopBrands[0];
                        that.loadShops();
                        that.loadShopBrandCommoditys();
                    }
                });
        },
        loadShopBrandCommoditys: function () {
            var that = this;
            if (this.currentShopBrand == null) return;
            that.searchModel.shopBrandId = this.currentShopBrand.id;
            return $scope.shopBrandCommoditySetAPI.doAction("getPagedList", that.searchModel)
                .then(function (data) {
                    that.items = data.content.items;
                    that.pageIndex = data.content.pageIndex;
                    that.pageSize = data.content.pageSize;
                    that.totalCount = data.content.totalCount;
                });
        },
        loadShops: function () {
            if (this.currentShopBrand == null) return;
            var that = this;
            return $scope.shopCommodityStockSetAPI.getShops(that.currentShopBrand.id)
                .then(function (data) {
                    that.shops = data.content.items;
                    if (that.shops.length > 0) {
                        that.currentShop = that.shops[0];
                    }
                });
        },
        loadSkuItems: function () {
            if (this.current == null) return;
            var that = this;
            return $scope.shopCommodityStockSetAPI.getSkuItems(this.current.id)
                .then(function (data) {
                    that.skuItems = data.content.items;
                    that.skus = data.content.skus;
                    that.load();
                });
        }
    };
});
