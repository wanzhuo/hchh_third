// module
var app = angular.module('app');

// controller
app.controller("communityDetailController", function ($scope, $http, $location) {
    //内容开始
    $scope.viewName = 'index';
    $scope.loadError = "";

    $scope.addMemberModel = { username: "", communityflag: "" };
    $scope.addAppModel = { username: "", communityflag: "" };
    $scope.addManagerModel = { username: "", communityflag: "" };
    $scope.flag;
    $scope.isManager = false;

    $scope.init = function (flag, isManager) {
        $scope.flag = flag;
        $scope.isManager = isManager;
        $scope.addMemberModel.communityflag = flag;
        $scope.addAppModel.communityflag = flag;
        $scope.addManagerModel.communityflag = flag;

        $scope.loadDetail();
    }

    $scope.loadDetail = function () {
        abc.showNotify("群组", "正在加载...", 'info');
        $http.post("/api/communityapi/GetSingle", { communityFlag: $scope.flag })
        .success(function (data, status, headers, config) {
            if (data.success) {
                $scope.model = data.content;
                for (var i = 0; i < $scope.model.apps.length; i++) {
                    var app = $scope.model.apps[i];
                    app.isEnabled = !app.IsDisabled;
                }
            }
            else {
                $scope.loadError = data.message;
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "加载失败，原因：" + status, 'alert');
        });
    }
    $scope.refreshMembers = function () {
        abc.showNotify("群组", "正在刷新页面...", 'info');
        $http.post("/api/communityapi/refreshMembers", { flag: $scope.flag })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "刷新成功..", 'success');
                $scope.loadDetail();
            }
            else {
                abc.showNotify("群组", "刷新失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "刷新失败，原因：" + status, 'alert');
        });
    }

    $scope.addMember = function () {
        abc.showNotify("群组", "正在添加成员...", 'info');
        $http.post("/api/communityapi/addMember", $scope.addMemberModel)
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "添加成员成功..", 'success');
                $scope.loadDetail();
            }
            else {
                abc.showNotify("群组", "添加成员失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "添加成员失败，原因：" + status, 'alert');
        });
    }

    $scope.removeMember = function (username) {
        abc.showNotify("群组", "正在移除成员...", 'info');
        $http.post("/api/communityapi/removeMember", { communityFlag: $scope.flag, username: username })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "移除成员成功..", 'success');
                $scope.loadDetail();
            }
            else {
                abc.showNotify("群组", "移除成员失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "移除成员失败，原因：" + status, 'alert');
        });
    }

    $scope.addApp = function (appflag) {
        abc.showNotify("群组", "正在添加应用...", 'info');
        $http.post("/api/communityapi/addApp", $scope.addAppModel)
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "添加应用成功..", 'success');
                $scope.loadDetail();
            }
            else {
                abc.showNotify("群组", "添加应用失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "添加应用失败，原因：" + status, 'alert');
        });
    }

    $scope.addManager = function () {
        abc.showNotify("群组", "正在添加管理员...", 'info');
        $http.post("/api/communityapi/addManager", $scope.addManagerModel)
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "添加管理员成功..", 'success');
                $scope.loadDetail();
            }
            else {
                abc.showNotify("群组", "添加管理员失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "添加管理员失败，原因：" + status, 'alert');
        });
    }

    $scope.removeManager = function (username) {
        abc.showNotify("群组", "正在移除管理员...", 'info');
        $http.post("/api/communityapi/removeManager", { communityFlag: $scope.flag, username: username })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "移除管理员成功..", 'success');
                $scope.loadDetail();
            }
            else {
                abc.showNotify("群组", "移除管理员失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "移除管理员失败，原因：" + status, 'alert');
        });
    }

    $scope.updateName = function () {
        abc.showNotify("群组", "正在修改群组名称...", 'info');
        if ($scope.model.name.length == 0) {
            alert("群组名称不能为空");
            return;
        }
        $http.post("/api/communityapi/SetCommunityName", { communityFlag: $scope.flag, name: $scope.model.name })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "修改群组名称成功..", 'success');
                $scope.loadDetail();
            }
            else {
                abc.showNotify("群组", "修改群组名称失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "修改群组名称失败，原因：" + status, 'alert');
        });
    }

    $scope.updateAppName = function (item) {
        abc.showNotify("应用", "正在修改应用名称...", 'info');
        if (item.name.length == 0) {
            alert("应用名称不能为空");
            return;
        }
        $http.post("/api/communityapi/SetAppName", { communityFlag: $scope.flag, name: item.name, appflag: item.flag })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("应用", "修改应用名称成功..", 'success');
                $scope.loadDetail();
            }
            else {
                abc.showNotify("应用", "修改应用名称失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("应用", "修改应用名称失败，原因：" + status, 'alert');
        });
    }

    $scope.setAppIsDisabled = function (item) {
        abc.showNotify("应用", "正在修改应用可用状态...", 'info');
        $http.post("/api/communityapi/SetAppIsDisabled", { communityFlag: $scope.flag, appflag: item.Flag, IsDisabled: !item.isEnabled })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("应用", "修改应用可用状态成功..", 'success');
                $scope.loadDetail();
            }
            else {
                abc.showNotify("应用", "修改应用可用状态失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("应用", "修改应用可用状态失败，原因：" + status, 'alert');
        });
    }

    $scope.openSettingPage = function (item) {
        //console.log(abc);
        abc.openPage("appsetting" + $scope.flag + item.flag, item.name + "配置", "/community/appSetting?appflag=" + item.flag + "&communityFlag=" + $scope.flag);
    }

    //内容结束
});