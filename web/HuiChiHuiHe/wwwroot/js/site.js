app.module('starter.controllers', [])
  //注册
  .controller('BindPhoneCtrl', function ($rootScope, $scope, $http,GetCode) {
    $scope.forms = {};
    $scope.save = function () {
      $http({
        url: _preurl + 'api/WechatAPI/BindCustomerPhone',
        method: 'POST',
        data: {
          'Phone': $scope.forms.Phone,
          'Code': $scope.forms.Code,
          'Pwd': $scope.forms.Pwd
        }
      }).success(function (data, header, config, status) {
        if (data.Code != 1000) {
          $ionicPopup.alert({
            title: '<i class="iconfont icon-icon text-xxxl mc"></i>',
            template: '<div class="pd15 pdb-lg">' + data.Msg + '</div>',
            okText: "<i class='ion-close'></i>",
            okType: 'button-light'
          });
        } else {
          $cordovaToast.showShortCenter('密码重置成功，请登录！').then(function (success) {
            $state.go('login');
          }, function (error) {
          });
        }
      }).error(function (data, header, config, status) {
        $cordovaToast.showShortCenter("网络请求出错");
      });
    }

    $rootScope.paracont = "获取验证码";
    $rootScope.codehassend = false;
    $scope.getcode = function () {
      if ($scope.forms.Phone) {
        $rootScope.codehassend = true;
        GetCode($scope.forms.Phone, "api/WechatAPI/SendSmsForBindCustomerPhone");
      } else {
        alert('请输入手机号码！');
      }
    }
  })