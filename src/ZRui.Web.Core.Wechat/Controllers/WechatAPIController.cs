using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ZRui.Web.Core.Wechat;
using Senparc.Weixin.MP.Containers;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
using ZRui.Web.Core.Wechat.WechatAPIModels;
using System.Text.RegularExpressions;
using ZRui.Web.Sms;
using Microsoft.AspNetCore.Cors;
using System.Text;

namespace ZRui.Web.Controllers
{
    [EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/[action]")]
    public class WechatAPIController : WechatApiControllerBase
    {
        ISmsHandler smsHandler;
        SmsOptions smsOptions;
        WechatOptions wechatOptions;
        private readonly ILogger _logger;
        string GetAccessToken()
        {
            return AccessTokenContainer.TryGetAccessToken(wechatOptions.AppId, wechatOptions.AppSecret);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="wechatOptions"></param>
        /// <param name="smsOptions"></param>
        /// <param name="smsHandler"></param>
        /// <param name="wechatCoreDb"></param>
        /// <param name="communityService"></param>
        /// <param name="memberOptions"></param>
        /// <param name="memberDb"></param>
        /// <param name="loggerFactory"></param>
        public WechatAPIController(
            IOptions<WechatOptions> wechatOptions,
            IOptions<SmsOptions> smsOptions,
            ISmsHandler smsHandler,
            WechatCoreDbContext wechatCoreDb,
            ICommunityService communityService,
            IOptions<MemberAPIOptions> memberOptions,
            MemberDbContext memberDb,
            ILoggerFactory loggerFactory)
            : base(memberOptions, memberDb, wechatCoreDb)
        {
            this.smsHandler = smsHandler;
            this.smsOptions = smsOptions.Value;
            this.wechatOptions = wechatOptions.Value;
            _logger = loggerFactory.CreateLogger<WechatAPIController>();
        }

        /// <summary>
        /// 是否已经绑定手机
        /// </summary>
        /// <param name="args">是否已经绑定手机参数类</param>
        /// <returns>成功content为true，否则为false</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult<bool> IsBindCustomerPhone([FromBody]IsBindCustomerPhoneArgsModel args)
        {
            var openId = GetOpenId();
            var count = wechatCoreDb.QueryCustomerPhone()
                   .Where(m => m.OpenId == openId)
                   .Where(m => !m.IsDel)
                   .Count();

            return Success<bool>(count > 0);
        }

        /// <summary>
        /// 发送手机绑定验证短信
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns>成功发送success为true，否则为false</returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public async Task<APIResult> SendSmsForBindCustomerPhone([FromBody]SendSmsForBindCustomerPhoneArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Phone))
                throw new ArgumentNullException("Phone");

            var phone = args.Phone.Trim();
            //var openId = args.OpenId.Trim();
            var openId = GetOpenId();
            var operationIP = GetIp();

            var isPhone = Regex.IsMatch(phone, @"^1[3578]\d{9}$", RegexOptions.IgnoreCase);
            if (!isPhone)
                throw new Exception("手机号码格式不正确，请正确输入手机号码！");

            if (wechatCoreDb.HasCustomerPhone(openId, phone))
                throw new Exception(string.Format("手机号码:{0}已绑定会员", phone));

            wechatCoreDb.CheckCustomerSmsValiCodeTask(phone, operationIP, CustomerSmsValiCodeTaskType.手机绑定);
            var code = CommonUtil.CreateIntNoncestr(6);
            var messageTemplate = "绑定手机验证码：{0}";
            if (this.smsOptions.SmsTemplates.ContainsKey("WechatBindCustomerPhone"))
            {
                messageTemplate = this.smsOptions.SmsTemplates["WechatBindCustomerPhone"];
            }
            var message = string.Format(messageTemplate, code);
            var sendIsSuccess = await sendPhoneSmsAsync(phone, message);
            if (!sendIsSuccess)
            {
                throw new Exception("发送失败");
            }
            CustomerSmsValiCodeTask task = new CustomerSmsValiCodeTask()
            {
                Phone = phone,
                IP = operationIP,
                TaskState = CustomerSmsValiCodeTaskState.未使用,
                TaskTime = DateTime.UtcNow,
                TaskEndTime = DateTime.Now.AddMinutes(5),
                TaskType = CustomerSmsValiCodeTaskType.手机绑定,
                Code = code,
            };
            wechatCoreDb.AddToCustomerSmsValiCodeTask(task);
            wechatCoreDb.SaveChanges();

            return Success();
        }

        /// <summary>
        /// 绑定手机
        /// </summary>
        /// <param name="args">绑定手机参数类</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult BindCustomerPhone([FromBody]BindCustomerPhoneArgsModel args)
        {
            if (string.IsNullOrEmpty(args.Phone))
                throw new Exception("手机号码不能为空");

            if (string.IsNullOrEmpty(args.Code))
                throw new Exception("验证码不能为空");

            var phone = args.Phone.Trim();
            var code = args.Code.Trim();
            //var openId = args.OpenId.Trim();
            var openId = GetOpenId();

            var isPhone = Regex.IsMatch(phone, @"^1[3578]\d{9}$", RegexOptions.IgnoreCase);
            if (!isPhone)
                throw new Exception("手机号码格式不正确，请正确输入手机号码！");

            //var member = db.GetSingleCustomerForUserNum(args.UserNum);
            //if (member == null) throw new Exception("指定的用户不存在");

            if (wechatCoreDb.HasCustomerPhone(openId, phone))
                throw new Exception(string.Format("手机号码:{0}已绑定会员", phone));

            var memberSmsValiCodeTask = wechatCoreDb.GetLastestCustomerSmsValiCodeTask(phone);

            if (memberSmsValiCodeTask == null)
                throw new Exception("请获取短信验证码");

            if (memberSmsValiCodeTask.Code != args.Code)
                throw new Exception("短信验证码不正确");

            var isExpiredTime = DateTime.Now > memberSmsValiCodeTask.TaskEndTime;
            if (isExpiredTime)
            {
                memberSmsValiCodeTask.TaskState = CustomerSmsValiCodeTaskState.已取消;
                wechatCoreDb.SaveChanges();
                throw new Exception("验证码已过期，请重新获取");
            }

            memberSmsValiCodeTask.TaskState = CustomerSmsValiCodeTaskState.已使用;

            var memberPhone = wechatCoreDb.QueryCustomerPhone()
                           .Where(m => m.OpenId == openId)
                           .Where(m => m.Phone == phone)
                           .Where(m => !m.IsDel)
                           .FirstOrDefault();

            if (memberPhone != null)
            {
                memberPhone.Status = CustomerPhoneStatus.已绑定;
            }
            else
            {
                memberPhone = new CustomerPhone()
                {
                    OpenId = openId,
                    Phone = phone,
                    Status = CustomerPhoneStatus.已绑定
                };
                wechatCoreDb.AddToCustomerPhone(memberPhone);
            }

            //var message = string.Format("【唯一网络】{0},{1}绑定手机:{2}", DateTime.Now, member.UserNum, phone);
            //SendPhoneSms(args.Phone, message);

            //CustomerSms memberSms = new CustomerSms()
            //{
            //    Phone = phone,
            //    SendTime = DateTime.Now,
            //    SmsType = CustomerSmsType.手机绑定成功,
            //    Message = message
            //};
            //db.AddToCustomerSms(memberSms);


            wechatCoreDb.SaveChanges();

            return Success();
        }

        /// <summary>
        /// 解除绑定手机
        /// </summary>
        /// <param name="args">解除绑定手机参数类</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SetIsDeleteBindCustomerPhone([FromBody]SetIsDeleteBindCustomerPhoneArgsModel args)
        {
            var openId = GetOpenId();
            var items = wechatCoreDb.QueryCustomerPhone()
                   .Where(m => m.OpenId == openId)
                   .Where(m => !m.IsDel)
                   .ToList();

            foreach (var item in items)
            {
                item.IsDel = true;
            }

            wechatCoreDb.SaveChanges();
            return Success();
        }

        /// <summary>
        /// 发送手机短信
        /// </summary>
        /// <param name="phone">手机</param>
        /// <param name="message">短信</param>
        private async Task<bool> sendPhoneSmsAsync(string phone, string message)
        {
            return await this.smsHandler.SendAsync(phone, message);
            //暂无功能
        }

        /// <summary>
        /// 获取头像的Url地址
        /// </summary>
        /// <param name="args">参数类</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult GetHeadImageUrl([FromBody]Core.Wechat.WechatAPIModels.GetHeadImageArgsModel args)
        {
            args.Size = args.Size ?? 0;
            var openId = GetOpenId();
            if (openId == "string")
            {
                openId = args.OpenId;
            }
            var accessToken = GetAccessToken();
            var userInfo = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetUserInfo(accessToken, openId);
            var url = userInfo.GetHeadImageUrl(args.Size.Value);
            return Success(url);
        }

        /// <summary>
        /// 通过小程序中获取到的手机来绑定用户手机
        /// </summary>
        /// <param name="argsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult BindCustomerPhoneByWxopenPhone([FromBody]BindCustomerPhoneByWxopenPhoneArgsModel argsModel)
        {
            var jwtFlag = User.Identity.Name;
            if (string.IsNullOrEmpty(argsModel.EncryptedData)) throw new ArgumentNullException("encryptedData");
            var jwt = wechatCoreDb.GetSingleMemberLogin(jwtFlag);
            if (jwt == null) throw new Exception("未找到登陆纪录");
            var session_key = jwt.GetloginSettingValue<string>("sessionKey");
            var openId = jwt.GetloginSettingValue<string>("openId");

            var json = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecodeEncryptedData(session_key, argsModel.EncryptedData, argsModel.Iv);
            //_logger.LogInformation(json);

            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<Senparc.Weixin.WxOpen.Entities.DecodedPhoneNumber>(json);

            var customerPhone = wechatCoreDb.Query<CustomerPhone>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.Status == CustomerPhoneStatus.已绑定 && m.OpenId == openId)
                    .FirstOrDefault();
            if (customerPhone == null)
            {
                customerPhone = new CustomerPhone()
                {
                    OpenId = openId,
                    Phone = args.phoneNumber,
                    Status = CustomerPhoneStatus.已绑定
                };
                wechatCoreDb.Add<CustomerPhone>(customerPhone);
                wechatCoreDb.SaveChanges();
            }

            //var email = $"{customerPhone.Phone}@phone";
            //var member = memberDb.Query<Member>()
            //    .Where(m => !m.IsDel)
            //    .Where(m => m.Email == email)
            //    .FirstOrDefault();

            //if (member == null)
            //{
            //    member = new Member()
            //    {
            //        Email = email,
            //        Password = CommonUtil.CreateNoncestr(8),
            //        Status = MemberStatus.正常,
            //        Truename = customerPhone.Phone,
            //        LastLoginTime = DateTime.Now,
            //        RegTime = DateTime.Now
            //    };

            //    memberDb.AddToMember(member);
            //    memberDb.SaveChanges();
            //}

            //改为使用MemberPhone
            var memberId = memberDb.GetMemberIdByMemberPhone(customerPhone.Phone);
            if (memberId <= 0)//说明没有手机绑定的会员，则新建一个
            {
                var member = new Member()
                {
                    Email = $"{customerPhone.Phone}@phone",
                    Password = CommonUtil.CreateNoncestr(8),
                    Status = MemberStatus.正常,
                    Truename = customerPhone.Phone,
                    LastLoginTime = DateTime.Now,
                    RegTime = DateTime.Now
                };
                memberDb.AddToMember(member);
                var memberPhone = new MemberPhone()
                {
                    Member = member,
                    Phone = customerPhone.Phone,
                    State = MemberPhoneState.已绑定
                };
                memberDb.Add<MemberPhone>(memberPhone);
                memberDb.SaveChanges();

                memberId = member.Id;
            }

            jwt.MemberId = memberId;
            wechatCoreDb.SaveChanges();

            return Success();
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "jwt")]
        public APIResult SaveWxUserInfo([FromBody]SaveWxUserInfoArgsModel args)
        {
            var jwtFlag = User.Identity.Name;
            var jwt = wechatCoreDb.GetSingleMemberLogin(jwtFlag);
            if (jwt == null) throw new Exception("未找到登陆纪录");
            var openId = jwt.GetloginSettingValue<string>("openId");
            var customerPhone = wechatCoreDb.Query<CustomerPhone>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.Status == CustomerPhoneStatus.已绑定 && m.OpenId == openId)
                    .FirstOrDefault();
            var memberId = memberDb.GetMemberIdByMemberPhone(customerPhone.Phone);
            var member = memberDb.MemberDbSet().Find(memberId);
            if (member == null) throw new Exception("用户信息不存在");
            member.Avatar = args.avatarUrl;
            member.NickName = args.nickName;
            try
            {
                memberDb.SaveChanges();
            }
            catch (Exception)
            {
                member.NickName = UniCodeToUTF8(args.nickName);
                memberDb.SaveChanges();
            }
            return Success();
        }


        string UniCodeToUTF8(string source)
        {
            byte[] uniByte = Encoding.Unicode.GetBytes(source);
            return Encoding.UTF8.GetString(uniByte);
        }

    }
}
