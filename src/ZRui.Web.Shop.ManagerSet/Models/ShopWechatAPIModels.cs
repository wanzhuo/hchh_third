using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Models
{

    public class ShopWechatAPIModels
    {

    }
    /// <summary>
    /// 获取微信基本信息返回
    /// </summary>
    public class GetWechatUserInfoByCodeResultModel
    {

        //
        // 摘要:
        //     用户的唯一标识
        public string openid { get; set; }
        //
        // 摘要:
        //     用户昵称
        public string nickname { get; set; }
 
        //
        // 摘要:
        //     用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        public string headimgurl { get; set; }
    }

}
