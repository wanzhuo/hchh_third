using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.MemberAPIModels
{
    public class LoginForEmailArgsModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginForSmsArgsModel
    {
        public string Phone { get; set; }
        public string Code { get; set; }
    }

    public class RegisterBySmsArgsModel
    {
        public string Phone { get; set; }
        public string Code { get; set; }
    }

    public class GetListArgsModel
    {

    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    public class GetPagedListArgsModel
    {
        public string Username { get; set; }
        public string MemberName { get; set; }
        public DateTime? RegTime1 { get; set; }
        public DateTime? RegTime2 { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string OrderName { get; set; }
        public string OrderType { get; set; }
    }

    public class GetPagedListModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }


    public class RowItem
    {
        [JsonProperty("id")]
        public virtual Int32 Id { get; set; }
        [JsonProperty("username")]
        public virtual String Username { get; set; }
        [JsonProperty("regTime")]
        public virtual DateTime RegTime { get; set; }
        [JsonProperty("regIP")]
        public virtual String RegIP { get; set; }
        [JsonProperty("lastLoginTime")]
        public virtual DateTime LastLoginTime { get; set; }
        [JsonProperty("lastLoginIP")]
        public virtual String LastLoginIP { get; set; }
        [JsonProperty("loginNum")]
        public virtual Int32 LoginNum { get; set; }
        [JsonProperty("memberName")]
        public virtual String MemberName { get; set; }
        [JsonProperty("status")]
        public virtual MemberStatus Status { get; set; }
        [JsonProperty("email")]
        public virtual String Email { get; set; }
        [JsonProperty("emailIsValid")]
        public virtual bool? EmailIsValid { get; set; }
    }

    public class AddArgsModel
    {
        public virtual String Username { get; set; }
        public virtual String MemberName { get; set; }
        public virtual String Password { get; set; }
    }

    public class UpdateArgsModel
    {
        public virtual Int32 Id { get; set; }
        public virtual String MemberName { get; set; }
        public virtual MemberStatus Status { get; set; }
        public virtual String Email { get; set; }
    }

    public class GetSingleArgsModel
    {
        public int ID { get; set; }
    }

    public class GetSingleModel
    {
        [JsonProperty("id")]
        public virtual Int32 Id { get; set; }
        [JsonProperty("username")]
        public virtual String Username { get; set; }
        [JsonProperty("regTime")]
        public virtual DateTime RegTime { get; set; }
        [JsonProperty("regIP")]
        public virtual String RegIP { get; set; }
        [JsonProperty("lastLoginTime")]
        public virtual DateTime LastLoginTime { get; set; }
        [JsonProperty("lastLoginIP")]
        public virtual String LastLoginIP { get; set; }
        [JsonProperty("loginNum")]
        public virtual Int32 LoginNum { get; set; }
        [JsonProperty("memberName")]
        public virtual String MemberName { get; set; }
        [JsonProperty("status")]
        public virtual MemberStatus Status { get; set; }
        [JsonProperty("email")]
        public virtual String Email { get; set; }
        [JsonProperty("emailIsValid")]
        public virtual Object EmailIsValid { get; set; }
    }

    public class DeleteArgsModel
    {
        public int Id { get; set; }
    }


    public class ChangePasswordArgsModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class GetDashBoardWidgetsArgsModel
    {

    }

    public class GetDashBoardWidgetsModel
    {
        public List<DashBoadWidgetInfo> One { get; set; }
        public List<DashBoadWidgetInfo> Two { get; set; }
        public List<DashBoadWidgetInfo> Three { get; set; }

        public GetDashBoardWidgetsModel()
        {
            One = new List<DashBoadWidgetInfo>();
            Two = new List<DashBoadWidgetInfo>();
            Three = new List<DashBoadWidgetInfo>();
        }
    }

    public class DashBoadWidgetInfo
    {
        public string Title { get; set; }
        public string ContentUrl { get; set; }
    }

    public class GetMemberInfoModel
    {
        public string Avatar { get; set; }
        public string Truename { get; set; }
        public string LastLoginIP { get; set; }
        public DateTime LastLoginTime { get; set; }
        public int LoginCount { get; set; }
    }

    public class UpdateMemberInfoArgsModel
    {
        public string Avatar { get; set; }
        public string Truename { get; set; }
    }

    public class UploadImageByBase64ArgsModel
    {
        public string Data { get; set; }
    }
}