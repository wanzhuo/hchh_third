webpackJsonp([7],[,function(e,t,n){"use strict";function a(e){return e&&e.__esModule?e:{default:e}}Object.defineProperty(t,"__esModule",{value:!0});var o=n(0),i=a(o),u=n(12),c=a(u);i.default.use(c.default),t.default=new c.default({routes:[{path:"/bindmobile",component:function(e){n.e(4).then(function(){var t=[n(18)];e.apply(null,t)}.bind(this)).catch(n.oe)}},{path:"/unbindmobile",component:function(e){n.e(3).then(function(){var t=[n(21)];e.apply(null,t)}.bind(this)).catch(n.oe)}},{path:"/info",component:function(e){n.e(1).then(function(){var t=[n(20)];e.apply(null,t)}.bind(this)).catch(n.oe)}},{path:"/describe/:id",component:function(e){n.e(2).then(function(){var t=[n(19)];e.apply(null,t)}.bind(this)).catch(n.oe)}},{path:"/writeinfo",component:function(e){n.e(0).then(function(){var t=[n(22)];e.apply(null,t)}.bind(this)).catch(n.oe)}},{path:"/:code/:page/:from",component:function(e){n.e(5).then(function(){var t=[n(23)];e.apply(null,t)}.bind(this)).catch(n.oe)}}]})},function(e,t,n){"use strict";function a(e){return e&&e.__esModule?e:{default:e}}Object.defineProperty(t,"__esModule",{value:!0});var o=n(0),i=a(o),u=n(14),c=a(u);i.default.use(c.default),t.default=new c.default.Store({state:{domain:"http://xcwzp.site/",appid:"wxbb8540e4892ce063",jwt:{},describe:{},applies:[],jobIdx:0}})},,,function(e,t,n){n(10);var a=n(6)(n(9),n(11),null,null);e.exports=a.exports},,,function(e,t,n){"use strict";function a(e){return e&&e.__esModule?e:{default:e}}var o=n(0),i=a(o),u=n(3),c=a(u),d=n(5),r=a(d),l=n(4),s=a(l),p=n(1),f=a(p),h=n(2),m=a(h);i.default.use(s.default),c.default.attach(document.body),i.default.config.productionTip=!1,f.default.beforeEach(function(e,t,n){if("oauth"===e.params.from)n();else{var a=(new Date).getTime();if(!m.default.state.jwt.token){var o=encodeURIComponent(e.path);return window.location.replace("https://open.weixin.qq.com/connect/oauth2/authorize?appid="+m.default.state.appid+"&redirect_uri="+m.default.state.domain+"dist/getcode.html&response_type=code&scope=snsapi_base&state="+o+"#wechat_redirect"),!1}(a-m.default.state.jwt.createTime)/1e3>m.default.state.jwt.expires_in-1200&&i.default.http({method:"Get",url:m.default.state.domain+"wechat/login",params:{code:m.default.state.jwt.code,state:m.default.state.jwt.state}}).then(function(e){m.default.state.jwt={token:"Bearer "+e.data.access_token,expires_in:e.data.expires_in,createTime:a}},function(e){alert("登录异常!"),window.location.href="https://open.weixin.qq.com/connect/oauth2/authorize?appid="+m.default.state.appid+"&redirect_uri="+m.default.state.domain+"dist/getcode.html&response_type=code&scope=snsapi_base&state="+o+"#wechat_redirect"}),n()}}),new i.default({router:f.default,store:m.default,render:function(e){return e(r.default)}}).$mount("#app-box")},function(e,t,n){"use strict";Object.defineProperty(t,"__esModule",{value:!0}),t.default={name:"app"}},function(e,t){},function(e,t){e.exports={render:function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{attrs:{id:"app"}},[n("router-view")],1)},staticRenderFns:[]}},,,,function(e,t){}],[8]);
//# sourceMappingURL=app.8d090d2c07be6493b9ae.js.map