using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using System.Linq;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using System.Threading.Tasks;
using System.Web;
using Senparc.Weixin.CommonAPIs;

namespace ZRui.Web.Core.Wechat
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class WechatCoreMessageHandler : MessageHandler<WechatCoreMessageContext>
    {
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */
        private string appId;
        private string appSecret;

        string GetAccessToken()
        {
            return AccessTokenContainer.TryGetAccessToken(appId, appSecret);
        }
        /// <summary>
        /// 模板消息集合（Key：checkCode，Value：OpenId）
        /// </summary>
        public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();
        ILogger _logger;
        DbContext db;

        public WechatCoreMessageHandler(DbContext db, string appId, string appSecret, Stream inputStream, PostModel postModel, ILogger _logger, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            this.db = db;
            this.appId = appId;
            this.appSecret = appSecret;
            this._logger = _logger;
            _logger.LogTrace(string.Format("post:init handler"));

            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;

            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            }

            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }



        public WechatCoreMessageHandler(DbContext db, string appId, string appSecret, XDocument xdoc, PostModel postModel, ILogger _logger, int maxRecordCount = 0)
            : base(xdoc, postModel, maxRecordCount)
        {
            this.db = db;
            this.appId = appId;
            this.appSecret = appSecret;
            this._logger = _logger;
            _logger.LogTrace(string.Format("post:init handler"));

            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;

            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            }

            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            _logger.LogTrace(string.Format("post:进入handler"));
            try
            {
                var defaultResponseMessage = base.CreateResponseMessage<ResponseMessageText>();

                var xml = RequestDocument.ToString();
                _logger.LogTrace(string.Format("post:{0}", xml));

                var data = new RequestMsgData();
                data.MsgId = requestMessage.MsgId;
                data.Content = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    msgType = "text",
                    text = requestMessage.Content
                });
                data.CreateTime = requestMessage.CreateTime;
                data.FromUserName = requestMessage.FromUserName;
                data.ToUserName = requestMessage.ToUserName;
                data.MsgType = requestMessage.MsgType;
                db.AddToRequestMsgData(data);
                db.SaveChanges();

                _logger.LogTrace(string.Format("post:原始数据保存成功"));


                //绑定会员功能，此处用输入账号密码来实现
                if (!string.IsNullOrWhiteSpace(requestMessage.Content) && requestMessage.Content.StartsWith("bd#"))
                {
                    string emailPwd = requestMessage.Content.Substring(2);
                    var member = db.MemberDbSet().FirstOrDefault(p => p.Email != null && p.Email.Trim() != string.Empty && (p.Email + p.Password) == emailPwd);
                    if (member != null)
                    {
                        var memberWechat = db.QueryMemberWechat().FirstOrDefault(p => p.MemberId == member.Id);
                        if (memberWechat != null)
                        {
                            memberWechat.OpenId = requestMessage.FromUserName;
                        }
                        else
                        {
                            memberWechat = new MemberWechat() { OpenId = requestMessage.FromUserName, MemberId = member.Id };
                            db.Add(memberWechat);
                        }
                        db.SaveChanges();
                    }
                }

                var robotMessages = db.QueryRobotMessage()
                     .Where(m => !m.IsDel)
                     .Where(m => m.Status == RobotMessageStatus.正常)
                     .Select(m => new
                     {
                         Question = m.Question,
                         Answer = m.Answer,
                         QuestionType = m.QuestionType
                     })
                     .ToList();
                _logger.LogTrace($"post:尝试自动回复,共条{robotMessages.Count}自动回复内容");

                var question = requestMessage.Content;
                var textResponse = robotMessages.Where(m => m.QuestionType == RobotMessageQuestionType.文本)
                    .Where(m => m.Question == question)
                    .FirstOrDefault();
                if (textResponse != null)
                {
                    if (textResponse.Answer.StartsWith("image:"))
                    {
                        var tmpMessage = CreateResponseMessage<ResponseMessageImage>();
                        tmpMessage.Image.MediaId = textResponse.Answer.Replace("images:", "");
                        return tmpMessage;
                    }
                    else if (textResponse.Answer.StartsWith("video:"))
                    {
                        var tmpMessage = CreateResponseMessage<ResponseMessageVideo>();
                        tmpMessage.Video.MediaId = textResponse.Answer.Replace("video:", "");
                        return tmpMessage;
                    }
                    else if (textResponse.Answer.StartsWith("voice:"))
                    {
                        var tmpMessage = CreateResponseMessage<ResponseMessageVoice>();
                        tmpMessage.Voice.MediaId = textResponse.Answer.Replace("voice:", "");
                        return tmpMessage;
                    }
                    else if (textResponse.Answer.StartsWith("music:"))
                    {
                        var tmpMessage = CreateResponseMessage<ResponseMessageMusic>();
                        tmpMessage.Music.ThumbMediaId = textResponse.Answer.Replace("music:", "");
                        return tmpMessage;
                    }
                    else if (textResponse.Answer.StartsWith("news:"))
                    {
                        var mediaId = textResponse.Answer.Replace("news:", "");
                        Task.Factory.StartNew(() =>
                        {
                            Senparc.Weixin.MP.AdvancedAPIs.CustomApi.SendMpNews(GetAccessToken(), base.WeixinOpenId, mediaId);
                        });
                        return null;
                    }
                    else
                    {
                        defaultResponseMessage.Content = textResponse.Answer;
                        return defaultResponseMessage;
                    }
                }

                var regexResponse = robotMessages.Where(m => m.QuestionType == RobotMessageQuestionType.正则)
                    .Where(m => System.Text.RegularExpressions.Regex.IsMatch(question, m.Question, System.Text.RegularExpressions.RegexOptions.None))
                    .FirstOrDefault();

                if (regexResponse != null)
                {
                    defaultResponseMessage.Content = regexResponse.Answer;
                    return defaultResponseMessage;
                }

                //这个没效果，仍然会弹出“该公众号暂时无法提供服务，请稍后再试”
                //return base.CreateResponseMessage<ResponseMessageNoResponse>();
                return new SuccessResponseMessage();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(-100), ex, "OnTextRequest Error");
                return new SuccessResponseMessage();
            }
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            var content = new
            {
                msgType = "image",
                mediaId = requestMessage.MediaId
            };

            var data = new RequestMsgData();
            data.MsgId = requestMessage.MsgId;
            data.Content = Newtonsoft.Json.JsonConvert.SerializeObject(content);
            data.CreateTime = requestMessage.CreateTime;
            data.FromUserName = requestMessage.FromUserName;
            data.ToUserName = requestMessage.ToUserName;
            data.MsgType = requestMessage.MsgType;
            db.AddToRequestMsgData(data);
            db.SaveChanges();

            var accessToken = Senparc.Weixin.MP.Containers.AccessTokenContainer.TryGetAccessToken(appId, appSecret);
            var fileName = CustomerMessage.GetCachePathForTemporaryMedia(requestMessage.MediaId);
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                MediaApi.Get(accessToken, requestMessage.MediaId, fs);
            }

            return null;
        }
        /// <summary>
        /// 通过二维码扫描关注扫描事件
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            _logger.LogTrace(Newtonsoft.Json.JsonConvert.SerializeObject(requestMessage));

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();

            var sceneId = int.Parse(requestMessage.EventKey);
            var eventTask = db.QueryWechatQRScene()
                .Where(m => m.SceneId == sceneId && m.QrCodeTicket == requestMessage.Ticket)
                .OrderByDescending(m => m.Id)
                .FirstOrDefault();
            if (eventTask == null || eventTask.Status == WechatQRSceneStatus.已处理)
            {
                responseMessage.Content = "您刚才扫的二维码已经失效";
            }

            if (eventTask.Category == "BindMember")
            {
                var bindMemberTask = db.QueryMemberWeChatBindTask()
                    .Where(m => m.Code == sceneId.ToString())
                    .FirstOrDefault();
                if (bindMemberTask == null || bindMemberTask.Status == MemberWeChatBindTaskStatus.已使用)
                {
                    responseMessage.Content = "您进行绑定的任务不存在或者已经完成";
                    return responseMessage;
                }

                var memberWechat = db.QueryMemberWechat().Where(m => m.MemberId == bindMemberTask.MemberId)
                    .FirstOrDefault();
                if (memberWechat != null && !memberWechat.IsDel) throw new Exception("您已经绑定了微信，如需要重新绑定，请先取消");

                bindMemberTask.OpenId = requestMessage.FromUserName;
                bindMemberTask.Status = MemberWeChatBindTaskStatus.已使用;

                memberWechat = new MemberWechat()
                {
                    IsDel = false,
                    MemberId = bindMemberTask.MemberId,
                    OpenId = requestMessage.FromUserName
                };
                db.AddToMemberWechat(memberWechat);

                db.SaveChanges();

                responseMessage.Content = "绑定成功！";

            }
            else if (eventTask.Category == "Login")
            {
                var bindMemberTask = db.QueryMemberWeChatLoginTask()
                    .Where(m => m.Code == sceneId.ToString())
                    .FirstOrDefault();
                if (bindMemberTask == null || bindMemberTask.Status != MemberWeChatLoginTaskStatus.扫二维码进行中)
                {
                    responseMessage.Content = "您进行登陆的任务不存在或者已经完成";
                    return responseMessage;
                }

                var memberWechat = db.QueryMemberWechat()
                    .Where(m => m.OpenId == requestMessage.FromUserName)
                    .Where(m => !m.IsDel)
                    .FirstOrDefault();
                if (memberWechat == null)
                {
                    responseMessage.Content = "您未绑定微信，登陆失败！";
                    return responseMessage;
                }
                bindMemberTask.OpenId = requestMessage.FromUserName;
                bindMemberTask.Status = MemberWeChatLoginTaskStatus.扫二维码完成;
                db.SaveChanges();

                responseMessage.Content = "您进行了扫码登陆，登陆时间" + DateTime.Now.ToString() + ".";


            }
            else
            {
                return base.OnEvent_ScanRequest(requestMessage);

            }

            return responseMessage;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "欢迎欢迎";
            var message = db.GetSingleRobotMessageForWelcome();
            if (message != null)
            {
                responseMessage.Content = message.Answer;
            }

            if (responseMessage.Content.Contains("[nickname]"))
            {
                var accessToken = GetAccessToken();
                var userInfo = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetUserInfo(accessToken, requestMessage.FromUserName);
                responseMessage.Content = responseMessage.Content.Replace("[nickname]", userInfo.nickname);
            }

            //if (!string.IsNullOrEmpty(requestMessage.EventKey))
            //{
            //    responseMessage.Content += "\r\n============\r\n场景值：" + requestMessage.EventKey;
            //}

            return responseMessage;
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            return responseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */

            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            //return responseMessage;

            //return null;

            return new SuccessResponseMessage();
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            string openid = requestMessage.FromUserName;
            var memberWechat = db.MemberWechatDbSet().FirstOrDefault(p => !p.IsDel && p.OpenId == openid);
            if (memberWechat != null)
            {
                var memberShop = db.DbSet<ShopMember>().FirstOrDefault(p => p.MemberId == memberWechat.MemberId);
                if (memberShop != null)
                {
                    var shopId = memberShop.ShopId;
                    string url = requestMessage.ScanCodeInfo.ScanResult.ToLower();
                    if (url.Contains("api/shopconglomerationorderapi/manager/taketheirfinishforscan")) //核销团购自提单
                    {
                        var args = HttpUtility.ParseQueryString(url);
                        string orderId = args["orderId"];
                        string pickupCode = args["pickupCode"];
                        if (!string.IsNullOrWhiteSpace(orderId) && !string.IsNullOrWhiteSpace(pickupCode))
                        {
                            var order = db.DbSet<ConglomerationOrder>().FirstOrDefault(p => p.Id.ToString() == orderId && p.PickupCode == pickupCode && p.ShopId == shopId);
                            if (order != null && order.Status == ShopOrderStatus.待自提)
                            {
                                order.Status = ShopOrderStatus.已完成;
                                db.SaveChanges();
                            }

                        }
                    }
                }
            }
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "订单" + requestMessage.ScanCodeInfo.ScanResult + "已核销";

            return responseMessage;
        }
    }
}