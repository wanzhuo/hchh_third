// module
var app = angular.module('app');

// controller
app.controller("communitySetIndexController", function ($scope, $http, $location) {
    $scope.statusList = [{ name: "正常", value: 1 }, { name: "停用", value: 4 }];
    $scope.searchStatusList = [{ name: "", value: '' }, { name: "正常", value: 1 }, { name: "停用", value: 4 }];

    $scope.addError = "";
    $scope.addSuccess = "";
    $scope.updateError = "";
    $scope.updateSuccess = "";

    $scope.pageIndex = 1;
    $scope.pageSize = 100;

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';
    $scope.searchModel = { communityFlag: "", appFlag: "", category: "" };
    $scope.newModel = { communityFlag: "", appFlag: "", category: 1 };
    $scope.addMemberModel = { username: "", appFlag: "", communityFlag: "" };
    $scope.addAppModel = { name: "", url: "", communityFlag: "", appFlag: "" };
    $scope.addManagerModel = { username: "", communityFlag: "", appFlag: "" };

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

        $scope.addMemberModel.communityFlag = communityFlag;
        $scope.addMemberModel.appFlag = appFlag;

        $scope.addAppModel.communityFlag = communityFlag;
        $scope.addAppModel.appFlag = appFlag;

        $scope.addManagerModel.communityFlag = communityFlag;
        $scope.addManagerModel.appFlag = appFlag;

        $scope.load();
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    //如果列表内容不是整个内容，则需要启用这个来获取整个内容
    $scope.getSingle = function (item) {
        $http.post("/api/communitySetAPI/GetSingle", { communityFlag: $scope.communityFlag, currentCommunityFlag: item.flag, appFlag: $scope.appFlag })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.current = data.content;
                    for (var i = 0; i < $scope.current.apps.length; i++) {
                        var app = $scope.current.apps[i];
                        app.isEnabled = !app.isDisabled;
                    }
                }
                else {
                    alert(data.message);
                }
            })
            .error(function (data, status, headers, config) {
                alert(status);
            });
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        //$scope.current = item;
        $scope.getSingle(item);
    }

    //添加
    $scope.add = function () {
        abc.showNotify("添加", "正在提交...", 'info');
        $scope.addError = "";
        $http.post("/api/communitySetAPI/add", $scope.newModel)
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

    //加载
    $scope.load = function () {
        abc.showNotify("加载", "正在加载...", 'info');

        $scope.searchModel.pageIndex = $scope.pageIndex;
        $scope.searchModel.pageSize = $scope.pageSize;

        $http.post("/api/communitySetAPI/getpagedlist", $scope.searchModel)
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

    $scope.addMember = function () {
        $scope.addMemberModel.currentCommunityFlag = $scope.current.flag;
        $scope.clearUpdateMessage();
        $http.post("/api/communitySetAPI/addMember", $scope.addMemberModel)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("添加成员成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("添加成员失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("添加成员失败，原因：" + status);
            });
    }

    $scope.removeMember = function (username) {
        $scope.clearUpdateMessage();
        $http.post("/api/communitySetAPI/removeMember", { appFlag: $scope.appFlag, communityFlag: $scope.communityFlag, currentCommunityFlag: $scope.current.flag, username: username })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("移除成员成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("移除成员失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("移除成员失败，原因：" + status);
            });
    }

    $scope.addApp = function (appflag) {
        $scope.clearUpdateMessage();
        $http.post("/api/communitySetAPI/addApp", $scope.addAppModel)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("添加应用成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("添加应用失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("添加应用失败，原因：" + status);
            });
    }

    $scope.addManager = function () {
        $scope.addManagerModel.currentCommunityFlag = $scope.current.flag;
        $scope.clearUpdateMessage();
        $http.post("/api/communitySetAPI/addManager", $scope.addManagerModel)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("添加管理员成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("添加管理员失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("添加管理员失败，原因：" + status);
            });
    }

    $scope.removeManager = function (username) {
        $scope.clearUpdateMessage();
        $http.post("/api/communitySetAPI/removeManager", { appFlag: $scope.appFlag, communityFlag: $scope.communityFlag, currentCommunityFlag: $scope.current.flag, username: username })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("移除管理员成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("移除管理员失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("移除管理员失败，原因：" + status);
            });
    }

    $scope.clearUpdateMessage = function () {
        $scope.updateError = "";
        $scope.updateSuccess = "";
    }

    $scope.showUpdateErrorMessage = function (info) {
        $scope.updateError = info;
    }

    $scope.showUpdateSuccessMessage = function (info) {
        $scope.updateSuccess = info;
    }

    $scope.updateName = function () {
        try {
            $scope.clearUpdateMessage();
            if ($scope.current.name.length == 0) {
                throw new Error(0, "群组名称不能为空")
            }

            $http.post("/api/communitySetAPI/SetCommunityName", { appFlag: $scope.appFlag, communityFlag: $scope.communityFlag, currentCommunityFlag: $scope.current.flag, name: $scope.current.name })
                .success(function (data, status, headers, config) {
                    if (data.success) {
                        $scope.showUpdateSuccessMessage("修改群组名称成功");
                        $scope.getSingle($scope.current);
                    }
                    else {
                        $scope.showUpdateErrorMessage("修改群组名称失败，原因：" + data.message);
                    }
                })
                .error(function (data, status, headers, config) {
                    $scope.showUpdateErrorMessage("修改群组名称失败，原因：" + status);
                });
        } catch (e) {
            $scope.showUpdateErrorMessage(e.message);
        }
    }

    $scope.updateAppName = function (item) {
        $scope.clearUpdateMessage();
        if (item.name.length == 0) {
            $scope.showUpdateErrorMessage("应用名称不能为空");
            return;
        }
        $http.post("/api/communitySetAPI/SetAppName", { appFlag: $scope.appFlag, communityFlag: $scope.communityFlag, currentCommunityFlag: $scope.current.flag, name: item.name, currentAppFlag: item.flag })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("修改应用名称成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("修改应用名称失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("修改应用名称失败，原因：" + status);
            });
    }

    $scope.setAppIsDisabled = function (item) {
        $scope.clearUpdateMessage();
        $http.post("/api/communitySetAPI/SetAppIsDisabled", { appFlag: $scope.appFlag, communityFlag: $scope.communityFlag, currentCommunityFlag: $scope.current.flag, currentAppFlag: item.flag, IsDisabled: !item.isEnabled })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("修改应用可用状态成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("修改应用可用状态失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("修改应用名修改应用可用状态失败称失败，原因：" + status);
            });
    }

    $scope.setAppIsDefaultOpen = function (item) {
        console.log(item.isDefaultOpen);
        $scope.clearUpdateMessage();
        $http.post("/api/communitySetAPI/SetAppIsDefaultOpen", { appFlag: $scope.appFlag, communityFlag: $scope.communityFlag, currentCommunityFlag: $scope.current.flag, currentAppFlag: item.flag, IsDefaultOpen: item.isDefaultOpen })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("修改应用默认开启状态成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("修改应用默认开启状态失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("修改应用名默认开启状态失败，原因：" + status);
            });
    }

    $scope.addApp = function () {
        $scope.addAppModel.currentCommunityFlag = $scope.current.flag;
        $scope.clearUpdateMessage();
        $http.post("/api/communitySetAPI/addApp", $scope.addAppModel)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("添加应用成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("添加应用失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("添加应用失败，原因：" + status);
            });
    }

    $scope.removeApp = function (item) {
        $scope.clearUpdateMessage();
        $http.post("/api/communitySetAPI/removeApp", { appFlag: $scope.appFlag, communityFlag: $scope.communityFlag, currentCommunityFlag: $scope.current.flag, currentAppFlag: item.flag })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.showUpdateSuccessMessage("移除应用成功");
                    $scope.getSingle($scope.current);
                }
                else {
                    $scope.showUpdateErrorMessage("移除应用失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.showUpdateErrorMessage("移除应用失败，原因：" + status);
            });
    }

    $scope.openSettingPage = function (item) {
        //console.log(abc);
        abc.openPage("communitysetappsetting" + $scope.current.flag + item.flag, item.name + "配置", "/communityset/appSetting?communityFlag=" + $scope.communityFlag + "&appflag=" + $scope.appFlag + "&currentCommunityFlag=" + $scope.current.flag + "&currentAppFlag=" + item.flag);
    }

});
