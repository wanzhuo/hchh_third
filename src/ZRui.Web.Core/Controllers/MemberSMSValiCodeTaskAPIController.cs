using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZRui.Web.MemberSMSValiCodeTaskAPIModels;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Text.RegularExpressions;
using ZRui.Web.Sms;
// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ZRui.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MemberSMSValiCodeTaskAPIController : CommunityApiControllerBase
    {
        private readonly ILogger _logger;
        ISmsHandler smsHandler;
        SmsOptions smsOptions;
        public MemberSMSValiCodeTaskAPIController(ICommunityService communityService
            , MemberDbContext memberDb
            , ISmsHandler smsHandler
            , IOptions<MemberAPIOptions> options
            , IOptions<SmsOptions> smsOptions
            , ILoggerFactory loggerFactory)
            : base(communityService, options, memberDb)
        {
            this.smsHandler = smsHandler;
            this.smsOptions = smsOptions.Value;
            _logger = loggerFactory.CreateLogger<MemberAPIController>();
        }

        /// <summary>
        /// 发送手机短信
        /// </summary>
        /// <param name="args">手机短信参数类</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> Add([FromBody]AddArgs args)
        {
            if (string.IsNullOrEmpty(args.Phone)) throw new ArgumentNullException("Phone");
            if (!smsOptions.SmsTemplates.ContainsKey(args.TaskType)) throw new Exception($"{args.TaskType}短信模板未配置");

            var phone = args.Phone.Trim();
            var operationIP = GetIp();

            var isPhone = Regex.IsMatch(phone, @"^1[23456789]\d{9}$", RegexOptions.IgnoreCase);
            if (!isPhone)
                throw new Exception("手机号码格式不正确，请正确输入手机号码！");

            var isBindMember = memberDb.IsBindMemberPhone(phone);
            if (!isBindMember)
                throw new Exception(string.Format("手机号码:{0}没有绑定会员", phone));

            memberDb.CheckMemberSMSValiCodeTaskLimitRule(phone, operationIP, args.TaskType);

            var code = CommonUtil.CreateIntNoncestr(6);
            var content = string.Format(smsOptions.SmsTemplates[args.TaskType], code);
            await smsHandler.SendAsync(phone, content);

            MemberSMSValiCodeTask task = new MemberSMSValiCodeTask()
            {
                Phone = phone,
                IP = operationIP,
                TaskState = MemberSMSValiCodeTaskState.未使用,
                TaskTime = DateTime.Now,
                TaskEndTime = DateTime.Now.AddMinutes(3),
                TaskType = args.TaskType,
                Code = code,
            };
            memberDb.AddToMemberSMSValiCodeTask(task);
            memberDb.SaveChanges();

            return Success();
        }
    }
}
