// module
var app = angular.module('app');

// controller
app.controller("articleSetOpenTxtController", function ($scope, $http, $location, notify, ngDialog, $httpEx, Upload, hotkeys, $window, $document) {
    $scope.isEdit = false;
    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';

    $scope.current = {content:""};
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

    //文件被点击
    $scope.openHistory = function () {
        abc.openPage("fileHistory" + $scope.model.communityFlag + $scope.model.flag, "文件历史", "/CommunityDocument/fileHistory?flag=" + $scope.model.flag + "&communityFlag=" + $scope.model.communityFlag);
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

        $scope.articleSetAPI.doAction("setContent", $scope.current)
            .then(function (data) {
            });
    }

    //加载
    $scope.getContent = function () {
        $scope.articleSetAPI.doAction("getContent", $scope.getContentModel)
            .then(function (data) {
                $scope.current = data.content;
                $window.abc.showContent($scope.current.content);
            });
    }


    $scope.startPos = 0;
    $scope.endPos = 0;
    $scope.caretPos = 0;
    $scope.textareaId = "editPanel";
    $scope.recordCaretPos = function () {
        var el = $document[0].getElementById($scope.textareaId);
        $scope.startPos = el.selectionStart;
        $scope.endPos = el.selectionEnd;
    }
    $scope.insertContent = function (content) {
        var startPos = $scope.startPos;
        var endPos = $scope.endPos;
        $scope.caretPos = startPos + content.length;
        var el = $document[0].getElementById($scope.textareaId);
        var tmpStr = el.value;
        $scope.current.content = tmpStr.substring(0, startPos) + content + tmpStr.substring(endPos, tmpStr.length);
        //var newContent = tmpStr.substring(0, startPos) + content + tmpStr.substring(endPos, tmpStr.length);

        el.value = $scope.current.content;
        setCaretPosition(el, $scope.caretPos);
    }

    function setCaretPosition(elem, caretPos) {
        if (elem !== null) {
            if (elem.createTextRange) {
                var range = elem.createTextRange();
                range.move('character', caretPos);
                range.select();
            } else {
                if (elem.selectionStart) {
                    elem.focus();
                    elem.setSelectionRange(caretPos, caretPos);
                } else
                    elem.focus();
            }
        }
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

    hotkeys.add({
        combo: 'alt+2',
        description: '插入日期时间',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function () {
            $scope.insertContent(new Date().format("yyyy-MM-dd hh:mm:ss"));
        }
    });

    hotkeys.add({
        combo: 'alt+3',
        description: '插入时间',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function () {
            $scope.insertContent(new Date().format("hh:mm:ss"));
        }
    });

    hotkeys.add({
        combo: 'alt+4',
        description: '插入日期',
        allowIn: ['INPUT', 'SELECT', 'TEXTAREA'],
        callback: function () {
            $scope.insertContent(new Date().format("yyyy-MM-dd"));
        }
    });
});
