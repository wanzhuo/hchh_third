// module
var app = angular.module('app');
app.filter('shopActorType', function () {
    return function (status) {
        status = parseInt(status);
        var s = "";
        switch (status) {
            case 10000:
                s = "超级管理员";
                break;
            default:
                s = "未知(" + status + ")"
        }
        return s;
    }
});
// controller
app.controller("shopSetIndexController", function ($scope, $http, $location, notify, ngDialog, $httpEx) {
    $scope.shopActorTypeList = [{ name: "超级管理员", value: 10000 }];

    $scope.addError = "";
    $scope.updateError = "";

    $scope.communityFlag = "";
    $scope.appFlag = "";
    $scope.viewName = 'index';
    $scope.shopActorSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopActorSetAPI", action, model, category);
        },
        getList: function (shopId) {
            return this.doAction("getList", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopId: shopId });
        },
        add: function (newModel) {
            return this.doAction("add", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopId: newModel.shopId, memberFlag: newModel.memberFlag, actorType: newModel.actorType });
        },
        setIsDelete: function (id) {
            return this.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: id });
        }
    };

    $scope.shopSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("shopSetAPI", action, model, category);
        }
    };

    $scope.ShopPayInfoSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("ShopPayInfoSetAPI", action, model, category);
        }
    };

    $scope.shopBrandSetAPI = {
        doAction: function (action, model, category) {
            return $httpEx.doAction("ShopBrandSetAPI", action, model, category);
        },
        getList: function () {
            return this.doAction("getList", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag })
        }
    };

    //初始化
    $scope.init = function (communityFlag, appFlag) {
        $scope.communityFlag = communityFlag;
        $scope.appFlag = appFlag;
        $scope.shopSet.init(communityFlag, appFlag);
        $scope.shopBrandSet.load();
    }

    $scope.showView = function (viewName) {
        $scope.viewName = viewName;
    }

    $scope.initAdd = function () {
        $scope.showView("add");
    }

    $scope.edit = function (item) {
        $scope.showView("edit");
        if (item.isShowApplets == '显示') {
            item.isShowApplets = true;
        } else {
            item.isShowApplets = false;
        }
        $scope.shopSet.current = item;
        $scope.shopActorSet.load();
    }

    $scope.shopActorSet = {
        items: [],
        newModel: { actorType: 10000 },
        load: function () {
            var that = this;
            return $scope.shopActorSetAPI.getList($scope.shopSet.current.id)
                .then(function (data) {
                    that.items = $scope.shopSet.exisShowApplets(data.content.items);
                });
        },
        add: function () {
            var that = this;
            this.newModel.shopId = $scope.shopSet.current.id;
            return $scope.shopActorSetAPI.add(this.newModel)
                .then(function (data) {
                    that.load();
                });
        },
        setIsDelete: function (item) {
            var that = this;
            return $scope.shopActorSetAPI.setIsDelete(item.id)
                .then(function (data) {
                    that.load();
                });
        }
    };

    $scope.shopBrandSet = {
        items: [],
        load: function () {
            var that = this;
            return $scope.shopBrandSetAPI.getList()
                .then(function (data) {
                    that.items = $scope.shopSet.exisShowApplets(data.content.items);
                });
        }
    };

    $scope.shopSet = {
        items: [],
        PayWays: [{ id: "0", name: "微信支付" }, { id: "1", name: "中信支付" }],
        searchModel: { communityFlag: "", appFlag: "" },
        newModel: { communityFlag: "", appFlag: "" },
        pageIndex: 1,
        pageSize: 25,
        init: function (communityFlag, appFlag) {
            this.searchModel.communityFlag = communityFlag;
            this.searchModel.appFlag = appFlag;

            this.newModel.communityFlag = communityFlag;
            this.newModel.appFlag = appFlag;

            this.load();
        },
        load: function () {
            var that = this;
            return $scope.shopSetAPI.doAction("getPagedList", that.searchModel)
                .then(function (data) {

                    that.items = $scope.shopSet.exisShowApplets(data.content.items);
                    that.pageIndex = data.content.pageIndex;
                    that.pageSize = data.content.pageSize;
                    that.totalCount = data.content.totalCount;
                });
        },
        exisShowApplets: function (items) {
            var showitems = items;
            for (var i = 0; i < showitems.length; i++) {
                showitems[i].abc = 'abc';
                if (showitems[i].isShowApplets === false) {
                    showitems[i].isShowApplets = '不显示';
                } else {
                    showitems[i].isShowApplets = '显示';
                }
                // showitems[i].isShowApplets === 'false' ? showitems[i].isShowApplets = '不显示' : '显示';


            }
            return showitems
        }
        ,
        getSingle: function (id) {
            var that = this;
            return $scope.shopSetAPI.doAction("getSingle", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: id })
                .then(function (data) {
                    that.current = data.content;
                });
        },
        add: function () {
            var that = this;
            $scope.shopSetAPI.doAction("add", that.newModel)
                .then(function (data) {
                    that.load();
                    $scope.showView("index");
                });
        },
        update: function () {
            var that = this;
            this.current.communityFlag = $scope.communityFlag;
            this.current.appFlag = $scope.appFlag;
            $scope.shopSetAPI.doAction("update", this.current)
                .then(function (data) {
                    $scope.showView("index");
                });
        },
        setIsDelete: function (item) {
            var that = this;
            $scope.shopSetAPI.doAction("setIsDelete", { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: item.id })
                .then(function (data) {
                    that.load();
                });
        },
        getPayInfo: function (pShop) {

        },
        savePayInfo: function (pShop) {
            if (!confirm("是否确定修改？（支付信息错误会导致不能正常支付）")) return;
            var parms = {
                communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, shopid: pShop.id, mchid: pShop.mchId, payway: Number(pShop.payWay), secretkey: pShop.secretKey, isenable: pShop.isEnable
                , publicKey: pShop.publicKey, prviateKey: pShop.prviateKey, reqUrl: pShop.reqUrl, notify: pShop.notify
            };
            $scope.ShopPayInfoSetAPI.doAction("SetPayInfo", parms)
                .then(function () { });
        },
        selectpayway: function (pShop) {
            if (Number(pShop.payWay) == 1) {
                $("#swiftpass").show();
                $("#wechat").hide();
            } else {
                $("#swiftpass").hide();
                $("#wechat").show();
            }
            var parms = { communityFlag: $scope.communityFlag, appFlag: $scope.appFlag, id: pShop.id, mchid: pShop.mchId, payway: Number(pShop.payWay), secretkey: pShop.secretKey };
            $scope.ShopPayInfoSetAPI.doAction("SelectShopPayInfoGetSingleByPayWay", parms)
                .then(function (data) {
                    console.log(data)
                    if (data.content.flage =='wechat') {
                        $scope.shopSet.current.mchId = data.content.mchid;
                        $scope.shopSet.current.secretKey = data.content.secretkey;
                        $scope.shopSet.current.isEnable = data.content.isenable;
                    } else {
                        $scope.shopSet.current.mchId = data.content.mchid;
                        $scope.shopSet.current.notify = data.content.notify;
                        $scope.shopSet.current.prviateKey = data.content.prviateKey;
                        $scope.shopSet.current.publicKey = data.content.publicKey;
                        $scope.shopSet.current.reqUrl = data.content.reqUrl;
                        $scope.shopSet.current.isEnable = data.content.isenable;
                    }

                });
        }
    };
});
