// module
var app = angular.module('app');

// controller
app.controller("articleSetOpenController", function ($scope, $http, $location, notify, ngDialog, $httpEx, Upload, hotkeys) {

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.current = null;
    $scope.getContentModel = { communityFlag: "", id: 0, appFlag: "" };

    $scope.articleSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("articleSetAPI", action, model, category);
        }
    };

    //初始化
    $scope.init = function (communityFlag, appFlag, id) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;

        $scope.getContentModel.communityFlag = communityFlag;
        $scope.getContentModel.appFlag = appFlag;
        $scope.getContentModel.id = id;

        $scope.getContent();
    }



    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    //修改
    $scope.save = function () {
        $scope.current.communityFlag = $scope.communityFlag;
        $scope.current.appFlag = $scope.appFlag;

        //更新不及时，所以这里加上这个。
        $scope.current.content = $scope.editEditor.getContent();

        $scope.articleSetAPI.doAction("setContent", $scope.current)
            .then(function (data) {
            });
    }

    //加载
    $scope.getContent = function () {
        $scope.articleSetAPI.doAction("getContent", $scope.getContentModel)
            .then(function (data) {
                $scope.current = data.content;
                $scope.editEditor.setContent($scope.current.content);
            });
    }

    hotkeys.add({
        combo: 'alt+`',
        description: '保存',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function () {
            console.log("保存");
            $scope.save();
        }
    });
});
