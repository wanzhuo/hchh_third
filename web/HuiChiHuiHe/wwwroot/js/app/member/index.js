// module
var app = angular.module('app');
app.controller("memberIndexController", function ($scope, $http, $location) {      

    $scope.newFolder = { parentFlag: '' };
    $scope.newFile = { ext: '.txt' };
    $scope.myDocumentData = [];
    $scope.currentFolder = null;
    $scope.currentFile = null;
    $scope.currentSettingItem = null;
    $scope.mydocumentIsEdit = false;
    $scope.mainUrl = "/member/main";
    $scope.tabsData = [];
    $scope.isShowBianqian = true;
    $scope.options = {
        UserName: $scope.UserName,
        UseEmail: "",
        Email: "",
    }
    $scope.isLoading = false;
    $scope.loadingCount = 0;

    $scope.notifyList = [];
    $scope.showMain = function () {
        $scope.selectTab($scope.tabsData[0]);
    }

    //获取用户个性配置
    $scope.getOptions = function () {
        abc.showNotify("个性配置", "正在加载", 'info');
        $http.post("/api/MsgNotifyAPI/GetOptions/", {
            UserName: $scope.UserName,
        })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("个性配置", "获取完成", 'success', false);
                $scope.options = data.content;
            }
            else {
                abc.showNotify("个性配置", "获取失败:" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("个性配置", "获取失败，原因：" + status, 'alert');
        });
    }

    //上传用户个性配置
    $scope.saveOptions = function () {
        $scope.options.UserName = $scope.UserName;
        var data = $scope.options;
        console.log(data);
        abc.showNotify("个性配置", "正在上传", 'info');
        $http.post("/api/MsgNotifyAPI/SaveOptions/", data)
            .success(function (data, status, headers, config) {
                if (data.success) {
                    abc.showNotify("个性配置", "上传完成", 'success');
                }
                else {
                    abc.showNotify("个性配置", "上传失败:" + data.message, 'alert');
                }
            })
            .error(function (data, status, headers, config) {
                abc.showNotify("个性配置", "上传失败，原因：" + status, 'alert');
            });
    }

    //初始化
    $scope.init = function (username) {
        $scope.UserName = username;
        $scope.getOptions();
        $('#pastePanel').pastableContenteditable();
        $('#pastePanel').on("pasteImage", function (ev, data) {
            abc.showNotify("截图上传", "正在上传...", 'info');
            var blobUrl = URL.createObjectURL(data.blob);
            var parentFlag = "";
            if ($scope.currentFolder != null) {
                parentFlag = $scope.currentFolder.Flag;
            }
            $http.post("/mydocument/PasteImageForBase64/", {
                data: data.dataURL,
                parentFlag: parentFlag
            })
            .success(function (data, status, headers, config) {
                if (data.success) {
                    abc.showNotify("截图上传", "上传完成", 'success');
                    $scope.loadChildren($scope.currentFolder);
                }
                else {
                    abc.showNotify("截图上传", "上传失败:" + data.message, 'alert');
                }
            })
            .error(function (data, status, headers, config) {
                abc.showNotify("截图上传", "上传失败，原因：" + status, 'alert');
            });
        });

        //加载根目录（为null代表加载根目录）
        $scope.loadChildren(null);
    }
    //文件的设置按钮被点击
    $scope.mydocumentItemSettingClicked = function (item) {
        $scope.currentSettingItem = item;
        abc.showMydocumentSettingBox();
    }
    //文件被点击
    $scope.mydocumentItemClicked = function (item) {
        //下面的代码实现选中后，再次点击，则取消选择。
        if (item == $scope.currentFolder) {
            $scope.currentFolder = null;
            item.isSelected = false;
            return;
        }

        //下面的代码用于选中没有选中的。
        if ($scope.currentFolder != null) {
            $scope.currentFolder.isSelected = false;
            $scope.currentFolder = null;
        }
        if ($scope.currentFile != null) {
            $scope.currentFile.isSelected = false;
            $scope.currentFile = null
        }
        item.isSelected = true;
        if (item.IsFolder) {
            $scope.currentFolder = item;
        }
        else {
            $scope.currentFile = item;
            $scope.openPage(item.Flag, item.Name, "/mydocument/open?flag=" + item.Flag);
        }
    }

    $scope.openPage = function (flag, name, url) {
        var hasTab = false;
        for (var i = 0; i < $scope.tabsData.length; i++) {
            var t = $scope.tabsData[i];
            t.active = false;
            if (t.id == flag) {
                t.active = true;
                hasTab = true;
            }
        }
        if (!hasTab) {
            $scope.tabsData.push({
                title: name,
                url: url,
                id: flag,
                active: true,
                canClose: true
            });
        }
    }

    $scope.reloadCurrentFrame = function () {
        for (var i = 0; i < $scope.tabsData.length; i++) {
            var t = $scope.tabsData[i];
            if (t.active) {
                if (t.url.indexOf('?') > 0) {
                    t.url = t.url + "&";
                }
                else {
                    t.url = t.url + "?";
                }

            }
        }
    }

    //主选项卡相关----------------
    //选中一个选项卡
    $scope.selectTab = function (item) {
        for (var i = 0; i < $scope.tabsData.length; i++) {
            var t = $scope.tabsData[i];
            t.active = false;
            if (t.id == item.id) {
                t.active = true;
            }
        }
    }
    //移除一个选项卡
    $scope.removeTab = function (item) {
        for (var i = 0; i < $scope.tabsData.length; i++) {
            var t = $scope.tabsData[i];
            if (t.id == item.id) {
                if (t.active) {
                    $scope.tabsData[i - 1].active = true;
                }
                $scope.tabsData.splice(i, 1);
                break;
            }
        }
    }
    //end主选项卡相关---结束------

    //添加一个文件夹
    $scope.addFolder = function () {
        abc.showNotify("添加文件夹", "正在提交...", 'info');
        if ($scope.currentFolder != null) {
            $scope.newFolder.parentFlag = $scope.currentFolder.Flag;
        }
        $http.post("/api/mydocumentapi/addfolder", $scope.newFolder)
        .success(function (data, status, headers, config) {
            $scope.newFolder.info = "";
            if (data.success) {
                $scope.newFolder.info = "添加完成";
                abc.showNotify("添加文件夹", "添加完成", 'success');
                $scope.loadChildren($scope.currentFolder);
            }
            else {
                abc.showNotify("添加文件夹", "添加失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {

            abc.showNotify("添加文件夹", "添加失败，原因：" + status, 'alert');
        });
    }
    //添加一个文件
    $scope.addFile = function () {
        abc.showNotify("添加文件", "正在提交...", 'info');
        if ($scope.currentFolder != null) {
            $scope.newFile.parentFlag = $scope.currentFolder.Flag;
        }
        $http.post("/api/mydocumentapi/add", $scope.newFile)
        .success(function (data, status, headers, config) {
            $scope.newFile.info = "";
            if (data.success) {
                $scope.newFile.info = "添加完成";
                abc.showNotify("添加文件", "添加完成", 'success');
                $scope.loadChildren($scope.currentFolder);
            }
            else {
                abc.showNotify("添加文件", "添加失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {

            abc.showNotify("添加文件", "添加失败，原因：" + status, 'alert');
        });
    }
    //选择文件后上传
    $scope.uploadFile = function () {
        abc.showNotify("上传", "正在上传...", 'info');
        var data = {};
        if ($scope.currentFolder != null) {
            data.parentFlag = $scope.currentFolder.Flag;
        }
        jQuery.ajaxFileUpload(
        {
            url: '/mydocument/uploadFile',
            type: 'post',
            data: data,
            secureuri: false,
            fileElementId: 'updateFile',
            dataType: 'json',
            success: function (data) {
                if (!data.issuccess) {
                    abc.showNotify("上传", "上传失败，原因：" + data.error, 'alert');
                }
                else {
                    abc.showNotify("上传", "上传成功", 'success');
                    $scope.$apply();
                    $scope.loadChildren($scope.currentFolder);
                }
            },
            error: function (data, status, e) {
                abc.showNotify("上传", "上传失败，原因：" + status, 'alert');
            },
            complete: function (xml, status) {
                console.log(xml + ":" + status);
            }
        });
    };
    //修改文件或者文件夹的名字
    $scope.updateName = function (item) {
        abc.showNotify("重命名", "正在提交重命名...", 'info');
        $http.post("/api/mydocumentapi/updateName", { flag: item.Flag, name: item.Name })
        .success(function (data, status, headers, config) {
            $scope.newFile.info = "";
            if (data.success) {
                abc.showNotify("重命名", "成功重命名", 'success');
            }
            else {
                abc.showNotify("重命名", "重命名失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("重命名", "重命名失败，原因：" + status, 'alert');
        });
    }
    //移动到当前选定的文件夹
    $scope.move = function (item) {
        $scope.moveTo(item, $scope.currentFolder);
    }
    //移动到指定的文件夹
    $scope.moveTo = function (item, folder) {
        if (item === folder || item.Flag == folder.Flag) return;
        if (folder == null || folder == undefined || folder.IsFolder) {
            abc.showNotify("移动", "正在提交...", 'info');
            var parentFlag = "";
            if (folder != null) {
                parentFlag = folder.Flag;
            }
            $http.post("/api/mydocumentapi/move", { flag: item.Flag, parentFlag: parentFlag })
            .success(function (data, status, headers, config) {
                $scope.newFile.info = "";
                if (data.success) {
                    abc.showNotify("移动", "成功移动", 'success');
                    moveChildren(item, folder);
                }
                else {
                    abc.showNotify("移动", "移动失败，原因：" + data.message, 'alert');
                }
            })
            .error(function (data, status, headers, config) {
                abc.showNotify("移动", "移动失败，原因：" + status, 'alert');
            });
        }
    }
    //移除一个文档
    $scope.setIsDelete = function (item) {
        abc.showNotify("移除", "正在提交移除" + item.Name + "...", 'info');
        //alert(item.parent);
        $http.post("/api/mydocumentapi/setIsDelete", { flag: item.Flag })
        .success(function (data, status, headers, config) {
            $scope.newFile.info = "";
            if (data.success) {
                abc.showNotify("移除", "成功移除" + item.Name, 'success');
                removeChildren(item.parent, item);
                //$scope.loadChildren(item.parent);
            }
            else {
                abc.showNotify("移除", "移除" + item.Name + "失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("移除", "移除失败，原因：" + status, 'alert');
        });
    }
    //加载下级文档
    $scope.loadChildren = function (parentItem) {
        var parentFlag = "";
        var notify = "加载根目录子节点";
        if (parentItem != null && parentItem != undefined) {
            parentFlag = parentItem.Flag;
            notify = "加载" + parentItem.Name + "子节点";
        }
        abc.showNotify("加载", notify, 'info');
        $http.post("/api/mydocumentapi/getlist", { parentFlag: parentFlag })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("加载", notify + "完成", 'success', false);
                if (parentItem == null || parentItem == undefined) {
                    $scope.myDocumentData = data.content.Items;
                }
                else {
                    parentItem.children = data.content.Items;
                    for (var i = 0; i < parentItem.children.length; i++) {
                        parentItem.children[i].parent = parentItem;
                    }
                }
            }
            else {
                abc.showNotify("加载", "加载失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("加载", "加载失败，原因：" + status, 'alert');
        });
    }

    //拖动相关
    $scope.dropObject = {
        objType: "",
        obj: null
    }

    $scope.mydocumentItemDrag = function (item) {
        $scope.dropObject.objType = "mydocumentItem";
        $scope.dropObject.obj = item;
    }

    $scope.mydocumentItemDrop = function (item) {
        if ($scope.dropObject.objType == "mydocumentItem") {
            var folder = item;
            if (folder.IsFolder) {
                $scope.moveTo($scope.dropObject.obj, folder);
            }
        }
    }

    $scope.binDrop = function () {
        if ($scope.dropObject.objType == "mydocumentItem") {
            $scope.setIsDelete($scope.dropObject.obj);
        }
    }



    //私有方法
    function removeChildren(folder, item) {
        var items = $scope.myDocumentData;
        if (folder != null) {
            items = folder.children;
        }

        for (var i = 0; i < items.length; i++) {
            var t = items[i];
            if (t.Flag == item.Flag) {
                items.splice(i, 1);
                break;
            }
        }
    }

    function insertChildren(folder, item) {
        if (folder == null) {
            $scope.myDocumentData.push(item);
        }
        else {
            if (folder.children != null && folder.children != undefined) {
                folder.children.push(item);
                item.parent = folder;
            }
        }
    }

    function moveChildren(item, targetFolder) {
        var currentFolder = item.parent;
        removeChildren(currentFolder, item);
        insertChildren(targetFolder, item);
    }
});

app.controller("mycommunityController", function ($scope, $http, $location, alertbox, $modal, hotkeys) {
    $scope.myCommunitys = [];
    $scope.newMyCommunity = { name: "" };

    //初始化
    $scope.init = function () {
        //加载群组
        $scope.loadMyCommutitys();
    }

    //文件被点击
    $scope.mycommunityItemClicked = function (item) {
        item.isSelected = true;
        var hasTab = false;
        console.log(item);

        for (var i = 0; i < $scope.tabsData.length; i++) {
            var t = $scope.tabsData[i];
            t.active = false;
            if (t.id == item.Flag) {
                t.active = true;
                hasTab = true;
            }
        }
        if (!hasTab) {
            var url = "/mycommunity/open?flag=" + item.Flag;
            if (!item.IsCommunity) {
                url = item.Url + "?communityFlag=" + item.communityFlag;
            }
            console.log(item);
            console.log(url);
            $scope.openPage(item.Flag, item.Name, url);
            //$scope.tabsData.push({
            //    title: item.Name,
            //    url: url,
            //    id: item.Flag,
            //    active: true,
            //    canClose: true
            //});
        }
    }

    //下面是群组相关


    $scope.loadMyCommutitys = function () {
        abc.showNotify("群组", "正在加载...", 'info');
        $http.post("/api/mycommunityapi/getlist", {})
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "加载成功", 'success', false);
                $scope.myCommunitys = data.content.Items;
                for (var i = 0; i < $scope.myCommunitys.length; i++) {
                    $scope.myCommunitys[i].IsCommunity = true;
                }
            }
            else {
                abc.showNotify("群组", "加载失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "加载失败，原因：" + status, 'alert');
        });
    }

    $scope.loadChildren = function (item) {
        console.log("loadChildren:" + item.Name);
        if (item.IsCommunity) {
            $scope.loadAppNavigates(item);
        }
        else {
            if (item.GetChildrenUrl != null && item.GetChildrenUrl.length > 0) {

            }
        }
    }

    $scope.loadAppNavigates = function (item) {
        abc.showNotify("群组", "正在加载应用...", 'info');
        $http.post("/api/mycommunityapi/GetAppNavigates", { flag: item.Flag })
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "加载应用成功", 'success', false);
                item.children = data.content.Navs;
                for (var i = 0; i < item.children.length; i++) {
                    item.children[i].communityFlag = item.Flag;
                    item.children[i].Flag = item.Flag + item.children[i].Url;
                    item.children[i].Ext = item.children[i].ICO;
                }

            }
            else {
                abc.showNotify("群组", "加载应用失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "加载应用失败，原因：" + status, 'alert');
        });
    }

    $scope.addMyCommunity = function () {
        abc.showNotify("群组", "正在添加...", 'info');
        $http.post("/api/mycommunityapi/add", $scope.newMyCommunity)
        .success(function (data, status, headers, config) {
            if (data.success) {
                abc.showNotify("群组", "添加成功", 'success');
                $scope.refreshMyCommunity();
            }
            else {
                abc.showNotify("群组", "添加失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "添加失败，原因：" + status, 'alert');
        });
    }

    $scope.refreshMyCommunity = function () {
        abc.showNotify("群组", "正在刷新...", 'info');
        $http.post("/api/mycommunityapi/refresh", {})
        .success(function (data, status, headers, config) {
            if (data.success) {
                //刷新完后就加载
                abc.showNotify("群组", "刷新成功", 'success');
                $scope.loadMyCommutitys();
            }
            else {
                abc.showNotify("群组", "刷新失败，原因：" + data.message, 'alert');
            }
        })
        .error(function (data, status, headers, config) {
            abc.showNotify("群组", "刷新失败，原因：" + status, 'alert');
        });
    }
});
