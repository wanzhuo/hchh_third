var app = angular.module('app', ['ui.bootstrap', 'ui.select', 'cfp.hotkeys', 'angular-loading-bar', 'ngDialog', 'validation', 'validation.rule', 'ui.tree', 'ngFileUpload', angularDragula(angular)]);

Date.prototype.format = function (fmt) { //author: meizz   
    var o = {
        "M+": this.getMonth() + 1,                 //月份   
        "d+": this.getDate(),                    //日   
        "h+": this.getHours(),                   //小时   
        "m+": this.getMinutes(),                 //分   
        "s+": this.getSeconds(),                 //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

function MathRand(count) {
    var Num = "";
    for (var i = 0; i < count; i++) {
        Num += Math.floor(Math.random() * 10);
    }
    return Num;
}

app.service('$httpEx', function ($http, $q, notify) {
    var me = this;
    this.post = function (url, model, category) {
        var deferred = $q.defer();
        var promise = deferred.promise;
        //notify.info("正在执行"+category+"...");
        $http.post(url, model)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    //如果是加载，则不提示
                    if (category != "加载") notify.success(category + "成功...");
                    deferred.resolve(data);
                }
                else {
                    notify.error(category + "失败，原因：" + data.message);
                    deferred.reject(data);
                }
            })
            .error(function (data, status, headers, config) {
                notify.error(category + "失败，原因：" + data.message);
                deferred.reject(null, status);
            });
        return promise;
    };

    this.doAction = function (controller, action, model, category) {
        if (category == null || category == undefined) {
            if (action.indexOf("set") == 0) {
                category = "设置";
            }
            else if (action.indexOf("get") == 0) {
                category = "加载";
            }
            else if (action.indexOf("remove") == 0) {
                category = "移除";
            }
            else if (action.indexOf("save") == 0) {
                category = "保存";
            }
            else {
                category = "操作";
            }
        }
        return me.post("/api/" + controller + "/" + action, model, category);
    }
});


//网上的例子有问题，factory不能注入$scope，只能注入$rootScope，可以理解为factory为一个全局变量
app.factory('notify', ['$rootScope', function ($rootScope) {
    return {
        show: function (message, type, delay) {
            if (!type) type = "info";
            if (!delay) delay = 2000;
            var icon = 'fa  fa-info-circle';

            $.notify({
                // options
                icon: icon,
                message: " " + message
            }, {
                    // settings
                    type: type,
                    animate: {
                        enter: 'animated fadeInDown',
                        exit: 'animated fadeOutUp'
                    },
                    delay: delay
                });
        },
        error: function (message) {
            this.show(message, 'danger', 5000);
        },
        info: function (message) {
            this.show(message, 'info');
        },
        success: function (message) {
            this.show(message, 'success');
        }
    };
}]);



app.filter('memberStatus', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case 1:
                s = "正常";
                break;
            case 4:
                s = "停用";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});
app.filter('currency', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case 0:
                s = "人民币";
                break;
            case 1:
                s = "美元";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});

app.filter('financeType', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case 0:
                s = "收入";
                break;
            case 1:
                s = "支出";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});

app.filter('diningType', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case 0:
                s = "早餐";
                break;
            case 1:
                s = "午餐";
                break;
            case 2:
                s = "晚餐";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});

app.filter('toDecimal2', function () {
    return function (x) {
        var f = parseFloat(x);
        if (isNaN(f)) {
            return x;
        }
        f = Math.round(x * 100) / 100;
        return f;
    }
});

app.filter('toDecimal4', function () {
    return function (x) {
        var f = parseFloat(x);
        if (isNaN(f)) {
            return x;
        }
        f = Math.round(x * 10000) / 10000;
        return f;
    }
});

app.filter('to_trusted', ['$sce', function ($sce) {
    return function (text) {
        return $sce.trustAsHtml(text);
    }
}]);


app.factory('tools', function ($http) {
    return {
        getUSDCNYRate: function (f) {
            $http.get("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.xchange%20where%20pair%20in%20(%22USDCNY%22)&format=json&diagnostics=true&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=")
                .success(function (data, status, headers, config) {
                    var rate = data.query.results.rate.Rate * 1.0095;
                    rate = Math.round(rate * 10000) / 10000;
                    f(rate);
                })
                .error(function (data, status, headers, config) {
                    console.log(status);
                });
        }
    };
});

app.directive('ueditor', function ($timeout, $http, $timeout) {
    return {
        restrict: 'A',
        scope: {
            content: "=",
            editor: "=",
            toolbar: "@",
            height: "@",
            serverUrl: "@"
        },
        link: function (scope, element, attrs, ctrl) {
            var id = 'ueditor_' + Date.now() + MathRand(6);
            element[0].id = id;
            var h = '350';
            if (scope.height != null && scope.height != undefined) {
                h = scope.height;
            }
            console.log(h);
            var config = {
                initialFrameWidth: '100%',
                initialFrameHeight: h,
                //autoHeightEnabled: false,
                //scaleEnabled: true,
                serverUrl: scope.serverUrl
            };
            if (scope.toolbar != null && scope.toolbar != undefined) {
                config.toolbar = [scope.toolbar];
                //alert(config.toolbars);
            }

            console.log(config.serverUrl);
            var ue = UE.getEditor(id, config);
            scope.editor = ue;
            ue.ready(function () {
                //alert(scope.content);
                if (scope.content) {
                    ue.setContent(scope.content);
                }

                if (isFirefox = navigator.userAgent.indexOf("Firefox") > 0 || navigator.userAgent.indexOf("MSIE") > 0) {
                    $('#' + id).pastableContenteditable();
                    $('#' + id).on("pasteImage", function (ev, data) {
                        var blobUrl = URL.createObjectURL(data.blob);
                        $http.post("/home/UploadImageForBase64/", {
                            data: data.dataURL
                        })
                            .success(function (data, status, headers, config) {
                                if (data.success) {
                                    scope.editor.execCommand('insertHtml', "<img src='" + data.url + "' />");
                                }
                                else {
                                    alert("粘贴失败：" + data.state);
                                }
                            })
                            .error(function (data, status, headers, config) {
                                alert(status);
                            });
                    })
                }
                ue.addListener('contentChange', function () {
                    var newContent = ue.getContent();
                    if (newContent != scope.content) {
                        //alert(rootScope);
                        console.log(scope.content);
                        $timeout(function () {
                            //scope.$apply(function () {
                            //if (!scope.$$phase) {
                            scope.content = newContent;
                            //}
                            // })
                        }, 0);
                    }
                });
            });
        }
    };
});



app.directive('deleteConfirm', ['$http', function ($http) {
    return {
        restrict: 'A',
        compile: function (tElem, attrs) {
            return function (scope, elem, attrs) {
                //alert("a");
                elem.click(function () {
                    content = elem.attr("delete-confirm");
                    if (content.length <= 0) {
                        content = "确定要删除？";
                    }
                    var d = dialog({
                        align: 'left',
                        content: content,
                        quickClose: true,
                        okValue: '确定',
                        ok: function () {
                            var dataAttr = elem.data();
                            if (dataAttr.fnDelete) {
                                scope.$eval(dataAttr.fnDelete);
                            }
                            //return false;
                        },
                        cancelValue: '取消',
                        cancel: function () { }
                    });
                    d.show(elem[0]);
                });
            }
        }
    };
}]);


app.directive('datePicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        scope: {
            minDate: '@'
        },
        link: function (scope, element, attr, ngModel) {

            element.val(ngModel.$viewValue);

            function onpicking(dp) {
                var date = dp.cal.getNewDateStr();
                scope.$apply(function () {
                    ngModel.$setViewValue(date);

                    //alert(ngModel.$viewValue);
                });
            }

            element.bind('click', function () {
                WdatePicker({
                    onpicking: onpicking,
                    dateFmt: 'yyyy-MM-dd'
                    //minDate: (scope.minDate || '%y-%M-%d')
                })
            });
        }
    };
});

app.directive('datetimePicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        scope: {
            minDate: '@'
        },
        link: function (scope, element, attr, ngModel) {

            element.val(ngModel.$viewValue);

            function onpicking(dp) {
                var date = dp.cal.getNewDateStr();
                scope.$apply(function () {
                    ngModel.$setViewValue(date);

                    //alert(ngModel.$viewValue);
                });
            }

            element.bind('click', function () {
                WdatePicker({
                    onpicking: onpicking,
                    dateFmt: 'yyyy-MM-dd HH:mm'
                    //minDate: (scope.minDate || '%y-%M-%d')
                })
            });
        }
    };
});