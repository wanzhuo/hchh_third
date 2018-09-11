// module
var app = angular.module('app');

app.filter('articleStatus', function () {
    return function (status) {
        status = parseInt(status);
        var s = "未知(" + status + ")";
        var articleStatusList = [{ name: "正常", value: 0 }, { name: "正常并索引", value: 1 }, { name: "未审核", value: -1 }, { name: "不开放", value: -2 }];
        for (var i = 0; i < articleStatusList.length; i++) {
            if (articleStatusList[i].value == status) {
                s = articleStatusList[i].name;
                break;
            }
        }
        return s;
    }
});

// controller
app.controller("articleSetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx, Upload) {
    $scope.searchArticlStatusList = [{ name: "", value: '' }, { name: "正常", value: 0 }, { name: "正常并索引", value: 1 }, { name: "未审核", value: -1 }, { name: "不开放", value: -2 }];
    $scope.articleStatusList = [{ name: "正常", value: 0 }, { name: "正常并索引", value: 1 }, { name: "未审核", value: -1 }, { name: "不开放", value: -2 }];
    $scope.columnTypeList = [{ name: "文本", value: 0 }, { name: "上传", value: 100 }];
    ///列表显示方式
    $scope.listViewTypeList = [{ name: "默认视图", value: 0 }, { name: "图片视图", value: 1 }, { name: "链接视图", value: 2 }];

    $scope.uploader = {};

    $scope.addError = "";
    $scope.updateError = "";

    $scope.updateCategoryError = "";

    $scope.pageIndex = 1;
    $scope.pageSize = 25;

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';
    $scope.searchModel = { communityFlag: "", category: "", appFlag: "", orderType: "asc", orderName: "orderweight" };
    $scope.newModel = { communityFlag: "", category: 1, appFlag: "" };
    $scope.loadCategoryModel = {};

    $scope.items = [];
    $scope.current = null;

    $scope.categorys = [];

    $scope.articleSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("articleSetAPI", action, model, category);
        }
    };

    $scope.articleCategorySetAPI = {
        count: {},
        doAction: function (action, model, category) {
            if (this.count[action] == undefined) this.count[action] = 0;
            this.count[action] = this.count[action] + 1;
            return $httpEx.doAction("articleCategorySetAPI", action, model, category);
        }
    };

    $scope.articleColumnSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("articleColumnSetAPI", action, model, category);
        }
    };

    $scope.articleScheduledTaskSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("articleScheduledTaskSetAPI", action, model, category);
        }
    };

    $scope.spiderAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("spiderAPI", action, model, category);
        }
    };

    //初始化
    $scope.init = function (communityFlag, appFlag) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;

        $scope.searchModel.communityFlag = communityFlag;
        $scope.searchModel.appFlag = appFlag;

        $scope.newModel.communityFlag = communityFlag;
        $scope.newModel.appFlag = appFlag;

        $scope.loadCategoryModel.communityFlag = communityFlag;
        $scope.loadCategoryModel.appFlag = appFlag;

        $scope.upload = function (file, model, fieldName) {
            console.log(file);
            Upload.upload({
                url: '/articleSet/UploadIco?communityFlag=' + $scope.communityFlag + '&appFlag=' + $scope.appFlag,
                data: { image: file }
            }).then(function (resp) {
                model[fieldName] = resp.data;
                console.log('Success uploaded. Response: ' + resp.data);
            }, function (resp) {
                console.log('Error status: ' + resp.status);
            }, function (evt) {
                var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                console.log('progress: ' + progressPercentage + '% ');
            });
        };

        $scope.loadCategory();
        $scope.load();
    }

    $scope.changeListViewType = function () {
        console.log($scope.currentListViewType);
    };

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.removeArrayItem = function (item, arr) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] == item) {
                arr.splice(i, 1);
                break;
            }
        }

        $scope.$apply();
    }

    $scope.itemOpen = function (item) {
        abc.openPage("article" + item.flag, item.title, "/ArticleSet/open?id=" + item.id + "&communityFlag=" + $scope.communityFlag + "&appFlag=" + $scope.appFlag);
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        $scope.current = item;
        $scope.article.loadCurrentArticleColumns();
        $scope.article.loadCurrentArticleExtends();
        //console.log($scope.editEditor);
        //$scope.editEditor.setContent($scope.current.content);
        //$scope.getSingle(item);
    }

    $scope.editScheduled = function (item) {
        $scope.showView("editScheduled");
        $scope.current = item;
        $scope.articleScheduledTask.loadItems(item);
    }

    //添加
    $scope.add = function () {
        if (!$scope.currentCategory) {
            alert("没选择类别");
        }
        else {
            notify.info("正在提交添加...");
            $scope.newModel.categoryId = $scope.currentCategory.id;
            $scope.addError = "";
            $http.post("/api/articleSetAPI/add", $scope.newModel)
                .success(function (data, status, headers, config) {
                    $scope.newModel.info = "";
                    if (data.success) {
                        notify.success("添加完成");
                        $scope.showView("index");
                        $scope.reload();
                    }
                    else {
                        $scope.addError = data.message;
                    }
                })
                .error(function (data, status, headers, config) {
                    $scope.addError = status;
                });
        }
    }

    //添加
    $scope.addImages = function (files) {
        console.log(files);
        if (!$scope.currentCategory) {
            alert("没选择类别");
            return;
        }
        if (files && files.length) {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                Upload.upload({
                    url: '/articleSet/AddImage?communityFlag=' + $scope.communityFlag + '&appFlag=' + $scope.appFlag + '&categoryId=' + $scope.currentCategory.id,
                    data: { image: file }
                }).then(function (resp) {
                    $scope.reload();
                    console.log('Success uploaded. Response: ' + resp.data);
                }, function (resp) {
                    $scope.reload();
                    console.log('Error status: ' + resp.status);
                }, function (evt) {
                    var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                    console.log('progress: ' + progressPercentage + '% ');
                });
            }
        }
    }

    //修改
    $scope.update = function () {
        $scope.current.communityFlag = $scope.communityFlag;
        $scope.current.appFlag = $scope.appFlag;

        //更新不及时，所以这里加上这个。
        //$scope.current.content = $scope.editEditor.getContent();

        $scope.updateError = "";
        abc.showNotify("更新", "正在提交更新...", 'info');
        $http.post("/api/articleSetAPI/update", $scope.current)
            .success(function (data, status, headers, config) {
                $scope.newModel.info = "";
                if (data.success) {
                    abc.showNotify("更新", "成功更新", 'success');
                    $scope.showView("index");
                }
                else {
                    $scope.updateError = data.message;
                }
            })
            .error(function (data, status, headers, config) {
                $scope.updateError = status;
            });
    }

    //移除
    $scope.setIsDelete = function (item) {
        abc.showNotify("移除", "正在提交移除" + item.Name + "...", 'info');
        $http.post("/api/articleSetAPI/setIsDelete", { id: item.id, communityFlag: $scope.communityFlag, appFlag: $scope.appFlag })
            .success(function (data, status, headers, config) {
                $scope.newModel.info = "";
                if (data.success) {
                    abc.showNotify("移除", "成功移除" + item.Name, 'success');
                    $scope.reload();
                }
                else {
                    abc.showNotify("移除", "移除" + item.Name + "失败，原因：" + data.message, 'alert');
                }
            })
            .error(function (data, status, headers, config) {
                abc.showNotify("移除", "移除失败，原因：" + status, 'alert');
            });
    }

    $scope.reload = function () {
        $scope.searchModel.pageIndex = 0;
        $scope.items = [];
        $scope.load();
    }
    //加载
    $scope.load = function () {
        abc.showNotify("加载", "正在加载...", 'info');

        $scope.searchModel.pageIndex = $scope.pageIndex;
        $scope.searchModel.pageSize = $scope.pageSize;

        if ($scope.currentCategory) {
            $scope.searchModel.categoryId = $scope.currentCategory.id;
        }

        $http.post("/api/articleSetAPI/getpagedlist", $scope.searchModel)
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

    //选择文件后上传
    $scope.fileNameChanged = function (file, currentModel) {
        console.log("select file");
        jQuery.ajaxFileUpload(
            {
                url: '/articleSet/UploadIco?communityFlag=' + $scope.communityFlag + '&appFlag=' + $scope.appFlag,
                secureuri: false,
                fileElementId: $(file).attr("id"),
                dataType: 'text',
                success: function (data) {
                    if (data == "1") {
                        alert("您好，上传失败，不支持上传非jpg、gif、png格式图片！");
                    } else if (data == "2") {
                        alert("大于512K！");
                    } else if (data == "3") {
                        alert("您好，上传失败，请重试！");
                    }
                    else {
                        //载入图片
                        currentModel.ico = data;
                        $scope.$apply();
                    }
                },
                error: function (data, status, e) {
                    alert(status)
                }
            });
    };

    $scope.fileNameChangedForCategory = function (file, currentModel, filedName) {
        console.log("select file");
        jQuery.ajaxFileUpload(
            {
                url: '/articleCategorySet/UploadIco?communityFlag=' + $scope.communityFlag + '&appFlag=' + $scope.appFlag,
                secureuri: false,
                fileElementId: $(file).attr("id"),
                dataType: 'text',
                success: function (data) {
                    if (data == "1") {
                        alert("您好，上传失败，不支持上传非jpg、gif、png格式图片！");
                    } else if (data == "2") {
                        alert("大于512K！");
                    } else if (data == "3") {
                        alert("您好，上传失败，请重试！");
                    }
                    else {
                        //载入图片
                        currentModel[filedName] = data;
                        $scope.$apply();
                    }
                },
                error: function (data, status, e) {
                    alert(status)
                }
            });
    };

    $scope.fileNameChangedForColumn = function (file, abc, filedName) {
        console.log("select file");
        abc();
        jQuery.ajaxFileUpload(
            {
                url: '/articleCategorySet/UploadIco?communityFlag=' + $scope.communityFlag + '&appFlag=' + $scope.appFlag,
                secureuri: false,
                fileElementId: $(file).attr("id"),
                dataType: 'text',
                success: function (data) {
                    if (data == "1") {
                        alert("您好，上传失败，不支持上传非jpg、gif、png格式图片！");
                    } else if (data == "2") {
                        alert("大于512K！");
                    } else if (data == "3") {
                        alert("您好，上传失败，请重试！");
                    }
                    else {
                        //载入图片
                        currentModel[filedName] = data;
                        $scope.$apply();
                    }
                },
                error: function (data, status, e) {
                    alert(status)
                }
            });
    }

    $scope.editCategory = function (item) {
        $scope.showView("editCategory");
        $scope.currentEditCategory = item;
        $scope.articleColumn.loadItems(item);
    }

    $scope.selectCategory = function (item) {
        $scope.currentCategory = item;
        if ($scope.currentCategory != null) {
            $scope.currentListViewType = $scope.currentCategory.defaultListViewType;
        }
        $scope.reload();
    }

    //添加
    $scope.addCategory = function (node) {
        abc.showNotify("添加类别", "正在提交...", 'info');
        var args = {};
        args.communityFlag = $scope.communityFlag;
        args.appFlag = $scope.appFlag;
        args.name = "新建类别";
        if (node)//如果存在node
        {
            args.pid = node.id;
        }

        $http.post("/api/articleCategorySetAPI/add", args)
            .success(function (data, status, headers, config) {
                $scope.newModel.info = "";
                if (data.success) {
                    abc.showNotify("添加类别", "添加完成", 'success');
                    $scope.loadCategory();
                }
                else {
                    abc.showNotify("添加类别", "添加失败：" + data.message, 'error');
                }
            })
            .error(function (data, status, headers, config) {
                abc.showNotify("添加类别", "添加失败：" + status, 'error');
            });
    }

    //移除
    $scope.setIsDeleteCategory = function (item) {
        notify.info("正在提交移除" + item.title);
        $http.post("/api/articleCategorySetAPI/setIsDelete", { id: item.id, communityFlag: $scope.communityFlag, appFlag: $scope.appFlag })
            .success(function (data, status, headers, config) {
                $scope.newModel.info = "";
                if (data.success) {
                    notify.success("成功移除" + item.title);
                    $scope.loadCategory();
                }
                else {
                    notify.error("移除" + item.title + "失败，原因：" + data.message);
                }
            })
            .error(function (data, status, headers, config) {
                notify.error("移除" + item.title + "失败，原因：" + datastatusmessage);
            });
    }

    $scope.treeOptions = {
        dropped: function (event) {
            console.log(event);
        }
    };

    //加载
    $scope.loadCategory = function () {
        abc.showNotify("加载", "正在加载...", 'info');
        $scope.articleCategorySetAPI.doAction("gettree", $scope.loadCategoryModel)
            .then(function (data) {
                $scope.categorys = data.content.tree;
            });
    }
    //修改
    $scope.updateCategory = function () {
        var item = $scope.currentEditCategory;
        item.communityFlag = $scope.communityFlag;
        item.appFlag = $scope.appFlag;

        $scope.updateCategoryError = "";
        notify.info("正在提交更新" + item.name);
        $http.post("/api/articleCategorySetAPI/update", item)
            .success(function (data, status, headers, config) {
                $scope.newModel.info = "";
                if (data.success) {
                    item.title = item.name;
                    notify.success("成功更新");
                    $scope.loadCategory();
                    $scope.showView("index");
                }
                else {
                    $scope.updateCategoryError = data.message;
                }
            })
            .error(function (data, status, headers, config) {
                $scope.updateCategoryError = status;
            });
    }

    $scope.changeCategoryParentDialog;
    $scope.showChangeCategoryParentDialog = function (dialogId) {
        $scope.changeCategoryParentDialog = ngDialog.open({
            template: dialogId,
            scope: $scope
        });
    }

    $scope.changeCategoryParent = function (category) {
        $scope.currentEditCategory.pid = category.id;
        $scope.currentEditCategory.parentName = category.name;
        $scope.changeCategoryParentDialog.close();

    }


    $scope.moveToCategoryDialog;
    $scope.showMoveToCategoryDialog = function (dialogId) {
        $scope.moveToCategoryDialog = ngDialog.open({
            template: dialogId,
            scope: $scope
        });
    }

    //将选择的项，移动到指定的类别下
    $scope.moveToCategory = function (category) {
        var ids = [];
        for (var i = 0; i < $scope.items.length; i++) {
            var item = $scope.items[i];
            if (item.isSelected) {
                ids.push(item.id);
            }
        }
        console.log(ids);
        $scope.articleSetAPI.doAction("moveToCategory", { ids: ids, categoryId: category.id, communityFlag: $scope.communityFlag, appFlag: $scope.appFlag })
            .then(function (data) {
                $scope.load();
                $scope.moveToCategoryDialog.close();
            });
    };

    $scope.spider = {
        targetUrl: "",
        getContent: function () {
            if (this.targetUrl.length <= 0) return;
            var model = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, url: this.targetUrl };
            $scope.spiderAPI.doAction("getcontent", model)
                .then(function (data) {
                    console.log(data);
                    $scope.newModel.title = data.content.title;
                    $scope.newModel.content = data.content.content;
                });
        }
    };

    $scope.article = {
        loadCurrentArticleColumns: function () {
            var model = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: $scope.current.id };
            $scope.articleSetAPI.doAction("getArticleColumns", model)
                .then(function (data) {
                    $scope.current.columns = data.content;
                });
        },
        loadCurrentArticleExtends: function () {
            var model = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: $scope.current.id };
            $scope.articleSetAPI.doAction("getArticleExtends", model)
                .then(function (data) {
                    $scope.current.extends = data.content;
                });
        }
    };
    //加载
    $scope.articleColumn = {
        items: [],
        loadItems: function (category) {
            var model = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, categoryId: category.id };
            $scope.articleColumnSetAPI.doAction("getList", model)
                .then(function (data) {
                    $scope.articleColumn.items = data.content.items;
                });
        },
        add: function (category) {
            var model = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, categoryId: category.id, name: "无标题" };
            $scope.articleColumnSetAPI.doAction("add", model)
                .then(function (data) {
                    $scope.articleColumn.loadItems(category);
                });
        },
        update: function (category, item) {
            item.communityFlag = $scope.communityFlag;
            item.appFlag = $scope.appFlag;
            $scope.articleColumnSetAPI.doAction("update", item)
                .then(function (data) {
                    $scope.articleColumn.loadItems(category);
                });
        },
        setIsDelete: function (category, item) {
            var model = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id };
            $scope.articleColumnSetAPI.doAction("setIsDelete", model)
                .then(function (data) {
                    $scope.articleColumn.loadItems(category);
                });
        }
    };

    $scope.articleScheduledTask = {
        items: [],
        loadItems: function (article) {
            var model = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, articleId: article.id };
            $scope.articleScheduledTaskSetAPI.doAction("getList", model)
                .then(function (data) {
                    $scope.articleScheduledTask.items = data.content.items;
                    for (var i = 0; i < $scope.articleScheduledTask.items.length; i++) {
                        var item = $scope.articleScheduledTask.items[i];
                        item.actionTime = item.actionTime.replace('T', ' ');
                        item.actionArgsModel = {};
                        try {
                            item.actionArgsModel = JSON.parse(item.actionArgs);
                        } catch (e) {
                            console.log(e);
                        }
                        console.log(item.actionArgsModel);
                    }
                });
        },
        add: function (article) {
            var model = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, articleId: article.id, actionName: "无动作" };
            $scope.articleScheduledTaskSetAPI.doAction("add", model)
                .then(function (data) {
                    $scope.articleScheduledTask.loadItems(article);
                });
        },
        update: function (article, item) {
            item.communityFlag = $scope.communityFlag;
            item.appFlag = $scope.appFlag;
            item.actionArgs = JSON.stringify(item.actionArgsModel);
            $scope.articleScheduledTaskSetAPI.doAction("update", item)
                .then(function (data) {
                    $scope.articleScheduledTask.loadItems(article);
                });
        },
        setIsDelete: function (article, item) {
            var model = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id };
            $scope.articleScheduledTaskSetAPI.doAction("setIsDelete", model)
                .then(function (data) {
                    $scope.articleScheduledTask.loadItems(article);
                });
        }
    };
});
