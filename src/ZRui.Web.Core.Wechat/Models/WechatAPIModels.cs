using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Wechat.WechatAPIModels
{
    public class SendSmsForBindCustomerPhoneArgsModel
    {
        public string Phone { get; set; }
    }

    public class IsBindCustomerPhoneArgsModel
    {
    }
    public class SetIsDeleteBindCustomerPhoneArgsModel
    {
    }
    public class BindCustomerPhoneArgsModel
    {
        /// <summary>
        ///  手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 验证证
        /// </summary>
        public string Code { get; set; }

    }

    public class GetHeadImageArgsModel
    {
        /// <summary>
        /// OpenId,当jwt登陆的OpenId为“string”时才生效，用于测试用，正常不用传。
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像）
        /// </summary>
        public int? Size { get; set; }
    }

    public class BindCustomerPhoneByWxopenPhoneArgsModel
    {
        /// <summary>
        /// 加密数据
        /// </summary>
        public string EncryptedData { get; set; }
        /// <summary>
        /// 向量
        /// </summary>
        public string Iv { get; set; }
    }

    public class SaveWxUserInfoArgsModel
    {
        public string avatarUrl { get; set; }
        public string nickName { get; set; }
        public string province { get; set; }
        public string city { get; set; }
    }

}
