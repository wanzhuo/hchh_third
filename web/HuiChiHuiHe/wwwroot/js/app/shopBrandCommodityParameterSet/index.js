// module
var app = angular.module('app');
// controller
app.controller("shopBrandCommodityParameterSetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.shopBrandCommodityParameterSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopBrandCommodityParameterSetAPI", action, model, category);
        }
    };

    $scope.shopBrandCommodityParameterValueSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopBrandCommodityParameterValueSetAPI", action, model, category);
        }
    };

    //初始化
    $scope.init = function (communityFlag, appFlag) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;
        $scope.shopBrandCommodityParameterSet.init(communityFlag, appFlag);
        $scope.shopBrandCommodityParameterValueSet.init(communityFlag, appFlag);
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.shopBrandCommodityParameterSet.current = item;
        $scope.shopBrandCommodityParameterValueSet.load();
    }

    $scope.selectShopBrand = function (item) {
        $scope.shopBrandCommodityParameterSet.currentShopBrand = item;
        $scope.shopBrandCommodityParameterSet.load();
    }

    $scope.shopBrandCommodityParameterValueSet = {
        getCurrentParameter: function () {
            return $scope.shopBrandCommodityParameterSet.current;
        },
        searchModel: {
            communityFlag: "",
            appFlag: ""
        },
        newModel: {
            communityFlag: "",
            appFlag: "",
            parameterId: null,
            value: ""
        },

        init: function (communityFlag, appFlag) {
            this.searchModel.communityFlag = communityFlag;
            this.searchModel.appFlag = appFlag;

            this.newModel.communityFlag = communityFlag;
            this.newModel.appFlag = appFlag;
        },
        load: function () {
            var that = this;
            var currentParameter = that.getCurrentParameter();
            console.log(currentParameter);
            this.getList(currentParameter.id)
                .then(function (data) {
                    currentParameter.values = data.content.items;
                });
        },
        getList: function (parameterId) {
            var that = this;
            if (parameterId != null) {
                that.searchModel.parameterId = parameterId;
            }
            return $scope.shopBrandCommodityParameterValueSetAPI.doAction("getList", that.searchModel);
        },
        add: function () {
            var that = this;
            var currentParameter = that.getCurrentParameter();

            if (currentParameter == null) {
                $scope.addError = "属性不能为空";
                return;
            }
            var value = that.newModel.value;
            if (value == null || value == undefined || value == "") {
                $scope.addError = "属性值不能为空";
                return;
            }
            that.newModel.parameterId = currentParameter.id;
            $scope.shopBrandCommodityParameterValueSetAPI.doAction("add", that.newModel)
                .then(function (data) {
                    that.newModel.value = "";
                    that.load(currentParameter);
                });
        },
        setIsDelete: function (item) {
            var that = this;
            var currentParameter = that.getCurrentParameter();

            $scope.shopBrandCommodityParameterValueSetAPI.doAction("setIsDelete", {
                communityFlag: $scope.communityFlag,
                appFlag: $scope.appFlag,
                id: item.id
            }).then(function (data) {
                that.load(currentParameter);
            });
        }
    };

    $scope.main =
        $scope.shopBrandCommodityParameterSet = {
            items: [],
            searchModel: {
                communityFlag: "",
                appFlag: ""
            },
            newModel: {
                communityFlag: "",
                appFlag: ""
            },
            pageIndex: 1,
            pageSize: 25,
            currentShopBrand: null,
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
                return $scope.shopBrandCommodityParameterSetAPI.doAction("getPagedList", that.searchModel)
                    .then(function (data) {
                        that.items = data.content.items;
                        that.pageIndex = data.content.pageIndex;
                        that.pageSize = data.content.pageSize;
                        that.totalCount = data.content.totalCount;
                    });
            },
            getSingle: function (id) {
                var that = this;
                return $scope.shopBrandCommodityParameterSetAPI.doAction("getSingle", {
                    communityFlag: $scope.communityFlag,
                    appFlag: $scope.appFlag,
                    id: id
                })
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
                $scope.shopBrandCommodityParameterSetAPI.doAction("add", that.newModel)
                    .then(function (data) {
                        that.load();
                        $scope.showView("index");
                    });
            },
            update: function () {
                var that = this;
                this.current.communityFlag = $scope.communityFlag;
                this.current.appFlag = $scope.appFlag;
                $scope.shopBrandCommodityParameterSetAPI.doAction("update", this.current)
                    .then(function (data) {
                        $scope.showView("index");
                    });
            },
            setIsDelete: function (item) {
                var that = this;
                $scope.shopBrandCommodityParameterSetAPI.doAction("setIsDelete", {
                    communityFlag: $scope.communityFlag,
                    appFlag: $scope.appFlag,
                    id: item.id
                })
                    .then(function (data) {
                        that.load();
                    });
            },
            loadShopBrands: function () {
                var that = this;
                return $scope.shopBrandCommodityParameterSetAPI.doAction("getShopBrands", that.searchModel)
                    .then(function (data) {
                        that.shopBrands = data.content.items;
                    });
            }
        };
});