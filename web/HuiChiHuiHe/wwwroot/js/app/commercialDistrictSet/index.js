// module
var app = angular.module('app');
app.filter('commercialDistrictStatus', function () {
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
app.controller("commercialDistrictSetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.commercialDistrictSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("commercialDistrictSetAPI", action, model, category);
        }
    };

    //初始化
    $scope.init = function (communityFlag, appFlag) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;
        $scope.commercialDistrictSet.init(communityFlag, appFlag);
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.commercialDistrictSet.current = item;
    }

    $scope.commercialDistrictSet = {
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
            return $scope.commercialDistrictSetAPI.doAction("getPagedList", that.searchModel)
                .then(function (data) {
                    that.items = data.content.items;
                    that.pageIndex = data.content.pageIndex;
                    that.pageSize = data.content.pageSize;
                    that.totalCount = data.content.totalCount;
                });
        },
        getSingle: function (id) {
            var that = this;
            return $scope.commercialDistrictSetAPI.doAction("getSingle", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: id })
                .then(function (data) {
                    that.current = data.content;
                });
        },
        add: function (item) {
            var that = this;
            $scope.commercialDistrictSetAPI.doAction("add", that.newModel)
                .then(function (data) {
                    that.load();
                    $scope.showView("index");
                });
        },
        update: function () {
            var that = this;
            this.current.communityFlag = $scope.communityFlag;
            this.current.appFlag = $scope.appFlag;
            $scope.commercialDistrictSetAPI.doAction("update", this.current)
                .then(function (data) {
                    $scope.showView("index");
                });
        },
        setIsDelete: function (item) {
            var that = this;
            $scope.commercialDistrictSetAPI.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id })
                .then(function (data) {
                    that.load();
                });
        }
    };
});
