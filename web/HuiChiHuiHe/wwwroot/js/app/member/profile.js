// module
var app = angular.module('app', []);
// controller
app.controller('memberProfileController', function ($scope, $http,$interval) {
    $scope.changePasswordModel = { password: "", newPassword: "",confirmPassword:"" };
    $scope.changePasswordError = "";
    $scope.changePasswordSuccess = "";
    $scope.errorInfo = "";
    $scope.viewName = "index";
    $scope.isBindWx=false;

    $scope.checkHasBindWx=function(){
        //检测是否已经绑定微信
        $http.post("/api/MemberWechatAPI/HasMemberWechat", $scope.changePasswordModel)
        .success(function (data, status, headers, config) {
            if (data.success) {
                if(data.content){
                    $scope.isBindWx=true;
                }else{
                    $scope.isBindWx=false;
                }
            }
            else {
                $scope.changePasswordError = data.message;

            }
        })
        .error(function (data, status, headers, config) {
            $scope.changePasswordError = status;
        });
    }

    $scope.checkHasBindWx();
    $interval(function() {
        if(!$scope.isBindWx){
            $scope.checkHasBindWx();
        }
    },2000)

    $scope.changePassword = function () {
        
        $scope.changePasswordError = "";
        $scope.changePasswordSuccess = "";

        var model = $scope.changePasswordModel;
        if (model.newPassword != model.confirmPassword) {
            $scope.changePasswordError = "两次密码不一致";
            return;
        }

        $http
            .post("/api/memberapi/changePassword", $scope.changePasswordModel)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    $scope.changePasswordSuccess = "success";
                }
                else {
                    $scope.changePasswordError = data.message;

                }
            })
            .error(function (data, status, headers, config) {
                $scope.changePasswordError = status;
            });
    }

    //解绑微信
    $scope.unBindWx=function(){
        $http
        .post("/api/MemberWechatAPI/SetIsDeleteForMemberWechat", {})
        .success(function (data, status, headers, config) {
            if (data.success) {
                alert("解绑成功!");
                window.location.reload();
            }
            else {
                $scope.changePasswordError = data.message;

            }
        })
        .error(function (data, status, headers, config) {
            $scope.changePasswordError = status;
        });
    }
});
