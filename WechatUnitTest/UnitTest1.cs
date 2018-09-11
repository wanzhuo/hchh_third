using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using ZRui.Web;
using ZRui.Web.Common;
using ZRui.Web.Controllers;
using ZRui.Web.Core.Wechat;
using ZRui.Web.Core.Wechat.WechatAPIModels;

namespace WechatUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           var  wechatCoreDb = new WechatCoreDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<WechatCoreDbContext>());
            var memberDb = new MemberDbContext(new Microsoft.EntityFrameworkCore.DbContextOptions<MemberDbContext>());
            var argsModel = new BindCustomerPhoneByWxopenPhoneArgsModel() { EncryptedData= "Bbu3OZPldIdKPezC8druWzqZlIA5lhyDEFPpMR20vieOHUxIO+��xhv+IZaw1krTTqyXvikTO91cKGtfIPOBN0VbxIj2JshpsDQ==", Iv= "EPonbgT9+okCcL025u8boQ==" };
            var jwtFlag = "jwt";// User.Identity.Name;
            if (string.IsNullOrEmpty(argsModel.EncryptedData)) throw new ArgumentNullException("encryptedData");
            var jwt = wechatCoreDb.GetSingleMemberLogin(jwtFlag);
            if (jwt == null) throw new Exception("δ�ҵ���½��¼");
            var session_key = jwt.GetloginSettingValue<string>("sessionKey");
            var openId = jwt.GetloginSettingValue<string>("openId");

            var json = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecodeEncryptedData(session_key, argsModel.EncryptedData, argsModel.Iv);
           // _logger.LogInformation(json);

            var args = Newtonsoft.Json.JsonConvert.DeserializeObject<Senparc.Weixin.WxOpen.Entities.DecodedPhoneNumber>(json);

            var customerPhone = wechatCoreDb.Query<CustomerPhone>()
                    .Where(m => !m.IsDel)
                    .Where(m => m.Status == CustomerPhoneStatus.�Ѱ� && m.OpenId == openId)
                    .FirstOrDefault();
            if (customerPhone == null)
            {
                customerPhone = new CustomerPhone()
                {
                    OpenId = openId,
                    Phone = args.phoneNumber,
                    Status = CustomerPhoneStatus.�Ѱ�
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
            //        Status = MemberStatus.����,
            //        Truename = customerPhone.Phone,
            //        LastLoginTime = DateTime.Now,
            //        RegTime = DateTime.Now
            //    };

            //    memberDb.AddToMember(member);
            //    memberDb.SaveChanges();
            //}

            //��Ϊʹ��MemberPhone
            var memberId = memberDb.GetMemberIdByMemberPhone(customerPhone.Phone);
            if (memberId <= 0)//˵��û���ֻ��󶨵Ļ�Ա�����½�һ��
            {
                var member = new Member()
                {
                    Email = $"{customerPhone.Phone}@phone",
                    Password = CommonUtil.CreateNoncestr(8),
                    Status = MemberStatus.����,
                    Truename = customerPhone.Phone,
                    LastLoginTime = DateTime.Now,
                    RegTime = DateTime.Now
                };
                memberDb.AddToMember(member);
                var memberPhone = new MemberPhone()
                {
                    Member = member,
                    Phone = customerPhone.Phone,
                    State = MemberPhoneState.�Ѱ�
                };
                memberDb.Add<MemberPhone>(memberPhone);
                memberDb.SaveChanges();

                memberId = member.Id;
            }

            jwt.MemberId = memberId;
            wechatCoreDb.SaveChanges();
        }
    }
}
