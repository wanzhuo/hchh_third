// module
var app = angular.module('app');
app.controller("memberMainController", function ($scope, $http, $location) {
    $scope.viewName = 'index';
    $scope.baseError = ''
    $scope.widgets = {};
    $scope.init = function () {
        $scope.getDashBoardWidgets();
    };
    //获取用户个性配置
    $scope.getDashBoardWidgets = function () {
        abc.showNotify("个性配置", "正在加载", 'info');
        $scope.baseError = ''
        $http.post("/api/memberapi/getDashBoardWidgets/", {})
            .success(function (data, status, headers, config) {
                if (data.success) {
                    abc.showNotify("个性配置", "获取完成", 'success', false);
                    $scope.widgets = data.content;
                    for (var i = 0; i < $scope.widgets.one.length; i++) {
                        $scope.getWidgetContent($scope.widgets.one[i]);
                    }
                    for (var i = 0; i < $scope.widgets.two.length; i++) {
                        $scope.getWidgetContent($scope.widgets.two[i]);
                    }
                    for (var i = 0; i < $scope.widgets.three.length; i++) {
                        $scope.getWidgetContent($scope.widgets.three[i]);
                    }
                }
                else {
                    $scope.baseError = data.message;
                }
            })
            .error(function (data, status, headers, config) {
                $scope.baseError = status;
            });
    }

    $scope.getWidgetContent = function (widget) {
        console.log(widget);
        var url = widget.contentUrl || '';
        console.log(url);
        if (url == "") return;
        widget.content = "正在加载...";
        $http.get(url)
            .success(function (data, status, headers, config) {
                widget.content = data;
            })
            .error(function (data, status, headers, config) {
                widget.content = "有错误发生：" + status;
            });
    }

    $scope.wxCreateMenu = function () {
        $http
            .post("/api/memberwechatapi/createMenu", { clientId: $scope.clientId })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    alert("添加菜单成功");
                }
                else {
                    alert(data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.errorInfo = status;
                alert($scope.errorInfo);

            });
    }

    $scope.jobSyncApplicants = function () {
        $http
            .post("/api/xiechengJobapi/SyncApplicants", { date: $scope.syncDate })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    alert("操作完成,共条" + data.content + "纪录");
                }
                else {
                    alert(data.message);
                }
            })
            .error(function (data, status, headers, config) {
                $scope.errorInfo = status;
                alert($scope.errorInfo);

            });
    }
});
