// module
var app = angular.module('app');
app.filter('shopBrandActorType', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case 10000:
                s = "超级管理员";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});
app.filter('shopBrandStatus', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case 0:
                s = "正常";
                break;
            case -1:
                s = "停用";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});
// controller
app.controller("shopBrandSetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {
    $scope.shopBrandActorTypeList = [{ name: "超级管理员", value: 10000 }];

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.shopBrandActorSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopBrandActorSetAPI", action, model, category);
        },
        getList: function (shopBrandId) {
            return this.doAction("getList", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopBrandId: shopBrandId });
        },
        add: function (newModel) {
            return this.doAction("add", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopBrandId: newModel.shopBrandId, memberFlag: newModel.memberFlag, actorType: newModel.actorType });
        },
        setIsDelete: function (id) {
            return this.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: id });
        }
    };

    $scope.shopBrandSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopBrandSetAPI", action, model, category);
        }
    };

    //初始化
    $scope.init = function (communityFlag, appFlag) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;
        $scope.shopBrandSet.init(communityFlag, appFlag);
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.shopBrandSet.current = item;
        $scope.shopBrandActorSet.load();
    }

    $scope.shopBrandActorSet = {
        items: [],
        newModel: { actorType: 10000 },
        load: function () {
            var that = this;
            return $scope.shopBrandActorSetAPI.getList($scope.shopBrandSet.current.id)
                .then(function (data) {
                    that.items = data.content.items;
                });
        },
        add: function () {
            var that = this;
            this.newModel.shopBrandId = $scope.shopBrandSet.current.id;
            return $scope.shopBrandActorSetAPI.add(this.newModel)
                .then(function (data) {
                    that.load();
                });
        },
        setIsDelete: function (item) {
            var that = this;
            return $scope.shopBrandActorSetAPI.setIsDelete(item.id)
                .then(function (data) {
                    that.load();
                });
        }
    };

    $scope.shopBrandSet = {
        items: [],
        searchModel: { communityFlag: "", appFlag: "" },
        newModel: { communityFlag: "", appFlag: "" },
        statusList: [{ name: "正常", value: 0 }, { name: "停用", value: -1 }],
        searchStatusList: [{ name: "", value: '' }, { name: "正常", value: 0 }, { name: "停用", value: -1 }],
        pageIndex: 1,
        pageSize: 25,
        init: function (communityFlag, appFlag) {
            this.searchModel.communityFlag = communityFlag;
            this.searchModel.appFlag = appFlag;

            this.newModel.communityFlag = communityFlag;
            this.newModel.appFlag = appFlag;

            this.load();
        },
        load: function () {
            var that = this;
            return $scope.shopBrandSetAPI.doAction("getPagedList", that.searchModel)
                .then(function (data) {
                    that.items = data.content.items;
                    that.pageIndex = data.content.pageIndex;
                    that.pageSize = data.content.pageSize;
                    that.totalCount = data.content.totalCount;
                });
        },
        getSingle: function (id) {
            var that = this;
            return $scope.shopBrandSetAPI.doAction("getSingle", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: id })
                .then(function (data) {
                    that.current = data.content;
                });
        },
        add: function (item) {
            var that = this;
            $scope.shopBrandSetAPI.doAction("add", that.newModel)
                .then(function (data) {
                    that.load();
                    $scope.showView("index");
                });
        },
        update: function () {
            var that = this;
            this.current.communityFlag = $scope.communityFlag;
            this.current.appFlag = $scope.appFlag;
            $scope.shopBrandSetAPI.doAction("update", this.current)
                .then(function (data) {
                    $scope.showView("index");
                });
        },
        setIsDelete: function (item) {
            var that = this;
            $scope.shopBrandSetAPI.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id })
                .then(function (data) {
                    that.load();
                });
        }
    };
});
