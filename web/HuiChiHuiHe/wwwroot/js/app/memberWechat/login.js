// module
var app = angular.module('app', []);
// controller
app.controller('loginController', function ($scope, $http, $timeout) {
    $scope.loginModel = { email: "", password: "" };
    $scope.errorInfo = "";
    $scope.clientId = "";
    $scope.qrCodeUrl = "";
    $scope.init = function (clientId) {
        $scope.clientId = clientId;
        $http
            .post("/api/memberwechatapi/GetLoginQRCodeUrl", { clientId: $scope.clientId })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.qrCodeUrl = data.content;
                    $scope.tryLogin();
                }
                else {
                    $scope.errorInfo = data.message;
                }
            })
            .error(function (data, status, headers, config) {
                $scope.errorInfo = status;
            });
    }

    $scope.tryLogin = function () {
        $http
            .post("/api/memberwechatapi/tryLoginAsync", { clientId: $scope.clientId })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    if (data.content == true)
                    {
                        if (returnUrl == null || returnUrl == undefined || returnUrl == "") {
                            window.location = "/";
                        }
                        else {
                            window.location = returnUrl;
                        }
                    }
                    else {
                        $timeout(function () {
                            $scope.tryLogin();
                        }, 5000)
                    }
                }
                else {
                    $scope.errorInfo = data.message;
                    $scope.isLoading = false;
                }
            })
            .error(function (data, status, headers, config) {
                $scope.errorInfo = status;
            });
    }
});
