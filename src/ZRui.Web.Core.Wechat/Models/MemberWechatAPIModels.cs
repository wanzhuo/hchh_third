using Senparc.Weixin.MP;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Wechat.MemberWechatAPIModels
{
    public class GetLoginQRCodeUrlArgsModel
    {
        public string ClientId { get; set; }
    }

    public class TryLoginArgsModel
    {
        public string ClientId { get; set; }
    }

    public class CreateMenuArgsModel
    {
        public List<MenuItemInfo> Items { get; set; }
    }
    public class MenuItemInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// 如 click 或 view ，请参照微信
        /// </summary>
        public string Type { get; set; }
        public List<MenuItemInfo> Items { get; set; }
        public MenuItemInfo()
        {
            Items = new List<MenuItemInfo>();
        }
    }



    public class GetOpenIdByCustomerPhoneArgsModel
    {
        public string Phone { get; set; }
    }

    public class SendCustomerTextArgsModel
    {
        public string OpenId { get; set; }
        public string Text { get; set; }
    }

    public class SendCustomerImageArgsModel
    {
        public string OpenId { get; set; }
        public string Data { get; set; }
    }

    public class SendCustomerNewsArgsModel
    {
        public string OpenId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }

    public class SendCustomerTemplateMessageArgsModel
    {
        public string OpenId { get; set; }
        public string TemplateId { get; set; }
        public string Url { get; set; }
        public object Data { get; set; }
    }

    public class ConnectCustomerSessionArgsModel
    {
        public string OpenId { get; set; }
    }

    public class DisConnectCustomerSessionArgsModel
    {
        public string OpenId { get; set; }
        public bool? MustDo { get; set; }
    }

    public class CloseCustomerSessionArgsModel
    {
        public string OpenId { get; set; }
    }

    public class GetCustomerSessionOpenIdListForNobodyArgsModel
    {

    }

    public class GetCustomerSessionOpenIdListArgsModel
    {
    }

    public class GetCustomerMessageCountForUnreadArgsModel
    {
        public string OpenId { get; set; }
    }

    public class GetCustomerMessageCountForUnreadModel
    {
        public int Count { get; set; }
        public string FromUser { get; set; }
        public long StartMsgId { get; set; }
    }

    public class GetCustomerMessageCountForAllUnreadArgsModel
    {
    }

    public class GetCustomerMessageListArgsModel
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }

        public string ChatFlag { get; set; }
        public int? BeginMsgId { get; set; }
    }

    public class GetHeadImageArgsModel
    {
        /// <summary>
        /// OpenId。
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像）
        /// </summary>
        public int? Size { get; set; }
    }

    public class GetUserInfoArgsModel
    {
        /// <summary>
        /// OpenId。
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像）
        /// </summary>
        public int? Size { get; set; }
    }

    public class GetUserInfoModel
    {
        public string Nickname { get; set; }
        public string HeadImageUrl { get; set; }
        public string City { get; internal set; }
        public string Country { get; internal set; }
        public string Province { get; internal set; }
        public int Sex { get; internal set; }
        public string Language { get; internal set; }
    }

    public class GetNewsMediaListArgsModel : CommunityArgsModel
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }

    public class GetNewsMediaListModel
    {
        public int TotalCount { get; set; }
        public List<Senparc.Weixin.MP.AdvancedAPIs.Media.MediaList_News_Item> Items { get; set; }

    }

    public class GetOtherMediaListArgsModel : CommunityArgsModel
    {
        public UploadMediaFileType? MediaFileType { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }

    public class GetOtherMediaListModel
    {
        public int TotalCount { get; set; }
        public List<Senparc.Weixin.MP.AdvancedAPIs.Media.MediaList_Others_Item> Items { get; set; }

    }

    public class UploadForeverMediaArgsModel : CommunityArgsModel
    {
        public UploadMediaFileType? MediaFileType { get; set; }
        public string Data { get; set; }
    }

    public class UploadForeverMediaModel
    {
        public string MediaId { get; set; }
        public string Url { get; set; }
    }

    public class DeleteForeverMediaArgsModel : CommunityArgsModel
    {
        public string MediaId { get; set; }
    }

    public class DeleteForeverMediaModel
    {
        public string MediaId { get; set; }
        public string Url { get; set; }
    }

    public class UploadNewsArgsModel : CommunityArgsModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ThumbMediaId { get; set; }
    }

    public class UploadNewsModel
    {
        public string MediaId { get; set; }
    }
}
