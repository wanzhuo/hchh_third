// module
var app = angular.module('app', []);
// controller
app.controller('loginController', function ($scope, $http) {
    $scope.loginModel = { email: "", password: "" };
    $scope.errorInfo = "";

    $scope.onEnter=function(e){
        var keyCode=window.event?e.keyCode:e.which;
        if(keyCode==13){
            $scope.login();
        }
    }

    $scope.login = function () {
        $http
            .post("/api/memberapi/loginforemail", $scope.loginModel)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    if (returnUrl == null || returnUrl == undefined || returnUrl == ""){
                        window.location = "/";
                    }
                    else {
                        window.location = returnUrl;
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
