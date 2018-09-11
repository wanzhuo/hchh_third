# 后端
### 携程设置
- 地址：后端-->系统管理-->招聘设置
- 页面：/Views/XiechengJobSet/Index
- 可用接口：
  - 地区管理：查看XiechengAreaSetAPI 文档
  - 权限管理：查看XiechengMemberSetAPI 文档

# 微信端

### 登陆
- 地址：/Wechat/LoginStart
- 跳转地址：/Wechat/Login?code=test&state=zhegewojuedemeishenmyong
  - 上面的code为test的时候表示测试，登陆后的openId=string

- 返回结果
```{"access_token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoic3RyaW5nIiwianRpIjoiZjlkYTUxMWEtMTkyYS00MDQ1LTk4OTQtMzRlZWNkMWIzNDVhIiwiaWF0IjoxNDkyMzgwMTMxLCJuYmYiOjE0OTI0MDg5MzEsImV4cCI6MTQ5MjQwOTIzMSwiaXNzIjoieGllY2hlbmciLCJhdWQiOiJhcHBsaWNhbnQifQ.NO_RfQsOhWg3EGol3ClhzXaUWexiRnYlpbP7oqeqRac","expires_in":300}
```

### 手机绑定
- 地址： /wechat/bindPhone
- 页面： /Views/Wechat/BindPhone.cshtml
- 可用接口：
  - 发送短信验证接口：/api/WechatAPI/SendSmsForBindCustomerPhone
  - 执行绑定手机接口：/api/WechatAPI/BindCustomerPhone
- 注意：由于没有短信接口，实际发送短信，可以进入数据库查看验证码，表 CustomerSmsValiCodeTask

### 面试登记表
- 地址： /xiechengJob/editApply
- 页面： /Views/XiechengJob/EditApply.cshtml
- 可用接口：
  - 获取当前OpenId关联的求职信息/api/XiechengJobAPI/GetApplies
  - 更新某个Id的求职信息：/api/XiechengJobAPI/UpdateApply

### 我的进度
- 地址： /xiechengJob/showProgress
- 页面： /Views/XiechengJob/ShowProgress.cshtml
- 可用接口：
  - 获取当前OpenId关联的求职信息/api/XiechengJobAPI/GetApplies
- 注意：进度是递增的，例如已经到了初试，那么初筛和电话就是绿色的，而这个页面的所有参数，再GetApplies中已经包含，那么当前直接使用GetApplies这个接口。

### 资料提交
- 地址： /xiechengJob/eitApplicant
- 页面： /Views/XiechengJob/EditApplicant.cshtml
- 可用接口：
  - 获取当前OpenId关联的应聘者资料/api/XiechengJobAPI/GetApplicant
  - 更新当前OpenId关联的应聘者资料/api/XiechengJobAPI/UpdateApplicant


