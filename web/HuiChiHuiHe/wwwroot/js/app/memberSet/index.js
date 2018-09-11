// module
var app = angular.module('app');

// controller
app.controller("memberSetIndexController", function ($scope, $http, $location) {
    $scope.statusList = [{ name: "正常", value: 1 }, { name: "停用", value: 4 }];
    $scope.searchStatusList = [{ name: "", value: '' }, { name: "正常", value: 1 }, { name: "停用", value: 4 }];

    $scope.addError = "";
    $scope.updateError = "";

    $scope.pageIndex = 1;
    $scope.pageSize = 100;

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';
    $scope.searchModel = { communityFlag: "", appFlag: "", category: "" };
    $scope.newModel = { communityFlag: "", appFlag: "", category: 1 };
    $scope.items = [];
    $scope.current = null;

    //初始化
    $scope.init = function (communityFlag, appFlag) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;
        $scope.searchModel.communityFlag = communityFlag;
        $scope.searchModel.appFlag = appFlag;
        $scope.newModel.communityFlag = communityFlag;
        $scope.newModel.appFlag = appFlag;
        $scope.load();
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    //如果列表内容不是整个内容，则需要启用这个来获取整个内容
    //$scope.getSingle = function (id) {
    //    $http.post("/api/memberSetAPI/GetSingle", { Id: id })
    //    .success(function (data, status, headers, config) {
    //        if (data.success) {
    //            $scope.current = data.content;
    //        }
    //        else {
    //            alert(data.message);
    //        }
    //    })
    //    .error(function (data, status, headers, config) {
    //        alert(status);
    //    });
    //}

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.current = item;
        //$scope.getSingle(item);
    }

    //添加
    $scope.add = function () {
        abc.showNotify("添加", "正在提交...", 'info');
        $scope.addError = "";
        $http.post("/api/memberSetAPI/add", $scope.newModel)
            .success(function (data, status, headers, config) {
                $scope.newModel.info = "";
                if (data.success) {
                    abc.showNotify("添加", "添加完成", 'success');
                    $scope.showView("index");
                    $scope.load();
                }
                else {
                    $scope.addError = data.message;
                }
            })
            .error(function (data, status, headers, config) {
                $scope.addError = status;
            });
    }

    //修改
    $scope.update = function () {
        $scope.current.communityFlag = $scope.communityFlag;
        $scope.current.appFlag = $scope.appFlag;

        $scope.updateError = "";
        abc.showNotify("更新", "正在提交更新...", 'info');
        $http.post("/api/memberSetAPI/update", $scope.current)
            .success(function (data, status, headers, config) {
                $scope.newModel.info = "";
                if (data.success) {
                    abc.showNotify("更新", "成功更新", 'success');
                    $scope.showView("index");
                }
                else {
                    $scope.updateError = data.message;
                }
            })
            .error(function (data, status, headers, config) {
                $scope.updateError = status;
            });
    }
    $scope.resetPassword = function () {
        $scope.updateError = "";
        $http.post("/api/memberSetAPI/resetPassword", { id: $scope.current.id, communityFlag: $scope.communityFlag, appFlag: $scope.appFlag })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.current.newPassword = data.content;
                }
                else {
                    $scope.updateError = data.message;
                }
            })
            .error(function (data, status, headers, config) {
                $scope.updateError = status;
            });
    }
    //移除
    $scope.setIsDelete = function (item) {
        abc.showNotify("移除", "正在提交移除" + item.Name + "...", 'info');
        $http.post("/api/memberSetAPI/setIsDelete", { id: item.id, communityFlag: $scope.communityFlag, appFlag: $scope.appFlag  })
            .success(function (data, status, headers, config) {
                $scope.newModel.info = "";
                if (data.success) {
                    abc.showNotify("移除", "成功移除" + item.Name, 'success');
                    $scope.reload();
                }
                else {
                    abc.showNotify("移除", "移除" + item.Name + "失败，原因：" + data.message, 'alert');
                }
            })
            .error(function (data, status, headers, config) {
                abc.showNotify("移除", "移除失败，原因：" + status, 'alert');
            });
    }

    $scope.reload = function () {
        $scope.searchModel.pageIndex = 0;
        $scope.items = [];
        $scope.load();
    }
    //加载
    $scope.load = function () {
        abc.showNotify("加载", "正在加载...", 'info');

        $scope.searchModel.pageIndex = $scope.pageIndex;
        $scope.searchModel.pageSize = $scope.pageSize;

        $http.post("/api/memberSetAPI/getpagedlist", $scope.searchModel)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    abc.showNotify("加载", "成功加载...", 'success');
                    $scope.totalCount = data.content.totalCount;
                    $scope.items = data.content.items;
                }
                else {
                    abc.showNotify("加载", "加载失败，原因：" + data.message, 'alert');
                }
            })
            .error(function (data, status, headers, config) {
                abc.showNotify("加载", "加载失败，原因：" + status, 'alert');
            });
    }

});
