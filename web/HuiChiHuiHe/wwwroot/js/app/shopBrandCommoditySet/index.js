// module
var app = angular.module('app');
// controller
app.controller("shopBrandCommoditySetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.shopBrandCommodityCategorySetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopBrandCommodityCategorySetAPI", action, model, category);
        }
    };

    $scope.shopBrandCommoditySetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopBrandCommoditySetAPI", action, model, category);
        }
    };

    $scope.shopBrandCommoditySkuSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopBrandCommoditySkuSetAPI", action, model, category);
        },
        getParameters: function (shopBrandId) {
            return this.doAction("getParameters", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopBrandId: shopBrandId })
        },
        getList: function (commodityId) {
            return this.doAction("getList", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, commodityId: commodityId })
        },
        update: function (commodityId, parameterIds) {
            return this.doAction("update", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, commodityId: commodityId, parameterIds: parameterIds })
        }
    };

    //初始化
    $scope.init = function (communityFlag, appFlag) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;
        $scope.shopBrandCommoditySet.init(communityFlag, appFlag);
        $scope.shopBrandCommodityCategorySet.init(communityFlag, appFlag);
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.shopBrandCommoditySet.current = item;
        $scope.shopBrandCommoditySkuSet.load();
    }

    $scope.selectShopBrand = function (item) {
        $scope.shopBrandCommoditySet.currentShopBrand = item;
        $scope.shopBrandCommoditySet.load();
        $scope.shopBrandCommoditySkuSet.loadParameters();
        $scope.shopBrandCommodityCategorySet.load();
    }

    $scope.changeCategoryDialog;
    $scope.showChangeCategoryDialog = function (dialogId, action) {
        $scope.changeCategoryDialog = ngDialog.open({
            template: dialogId,
            scope: $scope
        });
        if (action == "update"){
            $scope.changeCategoryDialog.doCustomAction = function (category) {
                $scope.shopBrandCommoditySet.current.categoryId = category.id;
                $scope.shopBrandCommoditySet.current.categoryName = category.name;
            }
        }
        else if (action == "add"){
            $scope.changeCategoryDialog.doCustomAction = function (category) {
                $scope.shopBrandCommoditySet.newModel.categoryId = category.id;
                $scope.shopBrandCommoditySet.newModel.categoryName = category.name;
            }
        }
    }

    $scope.changeCategory = function (category) {
        if ($scope.changeCategoryDialog.doCustomAction) {
            $scope.changeCategoryDialog.doCustomAction(category);
        }
        $scope.changeCategoryDialog.close();
    }

    $scope.shopBrandCommodityCategorySet = {
        cagetorys: [],
        searchModel: {
            communityFlag: "",
            appFlag: ""
        },
        getCurrentShopBrand: function () {
            return $scope.shopBrandCommoditySet.currentShopBrand;
        },
        init: function (communityFlag, appFlag) {
            this.searchModel.communityFlag = communityFlag;
            this.searchModel.appFlag = appFlag;
        },
        load: function () {
            var that = this;
            var shopBrand = that.getCurrentShopBrand();
            if (shopBrand != null) {
                that.searchModel.shopBrandId = shopBrand.id;
            }
            $scope.shopBrandCommodityCategorySetAPI.doAction("gettree", that.searchModel)
                .then(function (data) {
                    that.categorys = data.content.tree;
                });
        }
    }

    $scope.shopBrandCommoditySkuSet = {
        currentParameters: [],
        items: [],
        skuParameters: [],
        addModel: { parameterId: null },
        getCurrentShopBrand: function () {
            return $scope.shopBrandCommoditySet.currentShopBrand;
        },
        getCurrentShopBrandCommodity: function () {
            return $scope.shopBrandCommoditySet.current;
        },
        load: function () {
            var that = this;
            var commodity = that.getCurrentShopBrandCommodity();
            return $scope.shopBrandCommoditySkuSetAPI.getList(commodity.id)
                .then(function (data) {
                    that.items = data.content.items;
                    that.skuParameters = data.content.skuParameters;
                });
        },
        loadParameters: function () {
            var that = this;
            var shopBrand = that.getCurrentShopBrand();
            return $scope.shopBrandCommoditySkuSetAPI.getParameters(shopBrand.id)
                .then(function (data) {
                    that.currentParameters = data.content;
                });
        },
        add: function () {
            var that = this;
            var parameterIds = [];
            for (var i = 0; i < this.skuParameters.length; i++) {
                var p = this.skuParameters[i];
                if (p.id == this.addModel.parameterId) {
                    alert("已经存在该参数");
                    return;
                }
                parameterIds.push(p.id);
            }
            parameterIds.push(this.addModel.parameterId);
            var commodity = that.getCurrentShopBrandCommodity();
            return $scope.shopBrandCommoditySkuSetAPI.update(commodity.id, parameterIds)
                .then(function (data) {
                    that.load();
                });
        },
        remove: function (parameterId) {
            var that = this;
            var parameterIds = [];
            for (var i = 0; i < this.skuParameters.length; i++) {
                var p = this.skuParameters[i];
                if (p.id != parameterId) {
                    parameterIds.push(p.id);
                }
            }
            var commodity = that.getCurrentShopBrandCommodity();
            return $scope.shopBrandCommoditySkuSetAPI.update(commodity.id, parameterIds)
                .then(function (data) {
                    that.load();
                });
        }
    }

    $scope.shopBrandCommoditySet = {
        items: [],
        searchModel: { communityFlag: "", appFlag: "" },
        newModel: { communityFlag: "", appFlag: "" },
        pageIndex: 1,
        pageSize: 25,
        currentShopBrand: null,
        current: null,
        init: function (communityFlag, appFlag) {
            this.searchModel.communityFlag = communityFlag;
            this.searchModel.appFlag = appFlag;

            this.newModel.communityFlag = communityFlag;
            this.newModel.appFlag = appFlag;
            this.loadShopBrands();
        },
        load: function () {
            var that = this;
            if (this.currentShopBrand != null) {
                that.searchModel.shopBrandId = this.currentShopBrand.id;
            }
            return $scope.shopBrandCommoditySetAPI.doAction("getPagedList", that.searchModel)
                .then(function (data) {
                    that.items = data.content.items;
                    that.pageIndex = data.content.pageIndex;
                    that.pageSize = data.content.pageSize;
                    that.totalCount = data.content.totalCount;
                });
        },
        getSingle: function (id) {
            var that = this;
            return $scope.shopBrandCommoditySetAPI.doAction("getSingle", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: id })
                .then(function (data) {
                    that.current = data.content;
                });
        },
        add: function () {
            var that = this;
            if (this.currentShopBrand == null) {
                $scope.addError = "必须先选择一个商铺品牌";
                return;
            }

            that.newModel.shopBrandId = this.currentShopBrand.id;
            $scope.shopBrandCommoditySetAPI.doAction("add", that.newModel)
                .then(function (data) {
                    that.load();
                    $scope.showView("index");
                });
        },
        update: function () {
            var that = this;
            this.current.communityFlag = $scope.communityFlag;
            this.current.appFlag = $scope.appFlag;
            $scope.shopBrandCommoditySetAPI.doAction("update", this.current)
                .then(function (data) {
                    $scope.showView("index");
                });
        },
        setIsDelete: function (item) {
            var that = this;
            $scope.shopBrandCommoditySetAPI.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id })
                .then(function (data) {
                    that.load();
                });
        },
        loadShopBrands: function () {
            var that = this;
            return $scope.shopBrandCommoditySetAPI.doAction("getShopBrands", that.searchModel)
                .then(function (data) {
                    that.shopBrands = data.content.items;
                });
        }
    };
});
