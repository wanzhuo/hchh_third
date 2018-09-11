using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.MemberSetAPIModels
{

    public class GetListArgsModel:CommunityArgsModel
    {
        public string Truename { get; set; }
        public string Email { get; set; }
        public MemberStatus? Status { get; set; }
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }

    public class GetPagedListArgsModel : GetListArgsModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string OrderName { get; set; }
        public string OrderType { get; set; }
    }

    public class GetPagedListModel : GetListModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }


    public class RowItem
    {
        public virtual int Id { get; set; }
        public virtual string Email { get; set; }
        public virtual bool? EmailIsValid { get; set; }
        public virtual DateTime RegTime { get; set; }
        public virtual string RegIP { get; set; }
        public virtual DateTime LastLoginTime { get; set; }
        public virtual string LastLoginIP { get; set; }
        public virtual int LoginCount { get; set; }
        public virtual string Truename { get; set; }
        public virtual MemberStatus Status { get; set; }
    }

    public class AddArgsModel : CommunityArgsModel
    {
        public virtual String Email { get; set; }
        public virtual String Truename { get; set; }
        public virtual String Password { get; set; }
    }



    public class UpdateArgsModel : CommunityArgsModel
    {
        public virtual Int32 Id { get; set; }
        public virtual String Truename { get; set; }
        public virtual MemberStatus Status { get; set; }
        public virtual String Email { get; set; }
        public bool EmailIsValid { get; internal set; }
    }

    public class GetSingleArgsModel : CommunityArgsModel
    {
        public int ID { get; set; }
    }

    public class GetSingleModel
    {
        public virtual int Id { get; set; }
        public virtual string Email { get; set; }
        public virtual bool? EmailIsValid { get; set; }
        public virtual DateTime RegTime { get; set; }
        public virtual string RegIP { get; set; }
        public virtual DateTime LastLoginTime { get; set; }
        public virtual string LastLoginIP { get; set; }
        public virtual int LoginCount { get; set; }
        public virtual string Truename { get; set; }
        public virtual MemberStatus Status { get; set; }
    }

    public class DeleteArgsModel : CommunityArgsModel
    {
        public int Id { get; set; }
    }


    public class ChangePasswordForMeArgsModel : CommunityArgsModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordArgsModel : CommunityArgsModel
    {
        public int MemberId { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class SetIsDeleteArgsModel : CommunityArgsModel
    {
        public int Id { get; set; }
    }

    public class ResetPasswoordArgsModel : CommunityArgsModel
    {
        public int Id { get; set; }
    }
}