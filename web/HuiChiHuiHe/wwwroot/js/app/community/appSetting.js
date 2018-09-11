// module
var app = angular.module('app');

// controller
app.controller("communityAppSettingController", function ($scope, $http, $location) {
    //内容开始
    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.settingsContent = "{}";

    $scope.viewName = "index";
    $scope.loadError = "";
    $scope.updateError = "";

    //初始化
    $scope.init = function (appFlag, communityFlag) {
        $scope.appFlag = appFlag;
        $scope.communityFlag = communityFlag;

        $scope.initSetting();
    }


    $scope.initSetting = function () {
        abc.showNotify("加载设置", "正在加载设置...", 'info');
        $http.post("/api/communityapi/getappsettings", { appFlag: $scope.appFlag, communityFlag: $scope.communityFlag })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("加载设置", "加载完成...", 'success', true);
                $scope.settings = data.content;
                $scope.settingsContent = JSON.stringify(data.content, null, 4);
            }
            else {
                $scope.loadError = "加载设置失败，原因：" + data.message;

                abc.showNotify("加载设置", $scope.loadErrorInfo, 'alert');
            }
        })
        .error(function (data, status, headers, config) {

            $scope.loadError = "加载设置失败，原因：" + status;
            abc.showNotify("加载设置", $scope.loadError, 'alert');
        });
    }

    $scope.updateSettings = function () {
        $scope.updateError = "";
        $scope.updating = true;
        abc.showNotify("更新设置", "正在更新设置...", 'info');
        var settings;
        try {
            settings = jsonlint.parse($scope.settingsContent);
        } catch (e) {
            alert(e);
        }

        $http.post("/api/communityapi/SetAppSettings", { appFlag: $scope.appFlag, communityFlag: $scope.communityFlag, settings: settings })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("更新设置", "更新设置成功", 'success');
            }
            else {
                $scope.updateError =  data.message;
            }
            $scope.updating = false;

        })
        .error(function (data, status, headers, config) {
            $scope.updateError =  status;
            $scope.updating = false;
        });
    }

    //内容结束
});