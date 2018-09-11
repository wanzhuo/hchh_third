// module
var app = angular.module('app');
// controller
app.controller("shopBrandCommodityCategorySetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {

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

    //初始化
    $scope.init = function (communityFlag, appFlag) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;
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
        $scope.shopBrandCommodityCategorySet.current = item;
    }

    $scope.selectShopBrand = function (item) {
        $scope.shopBrandCommodityCategorySet.currentShopBrand = item;
        $scope.shopBrandCommodityCategorySet.load();
    }

    $scope.changeCategoryParentDialog;
    $scope.showChangeCategoryParentDialog = function (dialogId) {
        $scope.changeCategoryParentDialog = ngDialog.open({
            template: dialogId,
            scope: $scope
        });
    }

    $scope.changeCategoryParent = function (category) {
        $scope.shopBrandCommodityCategorySet.current.pid = category.id;
        $scope.shopBrandCommodityCategorySet.current.parentName = category.name;
        $scope.changeCategoryParentDialog.close();

    }

    $scope.shopBrandCommodityCategorySet = {
        cagetorys: [],
        searchModel: {
            communityFlag: "",
            appFlag: ""
        },
        currentShopBrand: null,
        init: function (communityFlag, appFlag) {
            this.searchModel.communityFlag = communityFlag;
            this.searchModel.appFlag = appFlag;

            this.loadShopBrands();
        },
        load: function () {
            var that = this;
            if (this.currentShopBrand != null) {
                that.searchModel.shopBrandId = this.currentShopBrand.id;
            }
            $scope.shopBrandCommodityCategorySetAPI.doAction("gettree", that.searchModel)
                .then(function (data) {
                    that.categorys = data.content.tree;
                });
        },
        getSingle: function (id) {
            var that = this;
            return $scope.shopBrandCommodityCategorySetAPI.doAction("getSingle", {
                communityFlag: $scope.communityFlag,
                appFlag: $scope.appFlag,
                id: id
            })
                .then(function (data) {
                    that.current = data.content;
                });
        },
        add: function (node) {
            var that = this;
            if (this.currentShopBrand == null) {
                $scope.addError = "必须先选择一个商铺品牌";
                return;
            }
            var args = {};
            args.shopBrandId = this.currentShopBrand.id;
            args.communityFlag = $scope.communityFlag;
            args.appFlag = $scope.appFlag;
            args.name = "新建类别";
            if (node)//如果存在node
            {
                args.pid = node.id;
            }
            
            $scope.shopBrandCommodityCategorySetAPI.doAction("add", args)
                .then(function (data) {
                    that.load();
                });
        },
        update: function () {
            var that = this;
            that.current.communityFlag = $scope.communityFlag;
            that.current.appFlag = $scope.appFlag;
            $scope.shopBrandCommodityCategorySetAPI.doAction("update", this.current)
                .then(function (data) {
                    that.current.title = that.current.name;
                    that.load();
                    $scope.showView("index");
                });
        },
        setIsDelete: function (item) {
            var that = this;
            $scope.shopBrandCommodityCategorySetAPI.doAction("setIsDelete", {
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
            return $scope.shopBrandCommodityCategorySetAPI.doAction("getShopBrands", that.searchModel)
                .then(function (data) {
                    that.shopBrands = data.content.items;
                });
        }
    };
});