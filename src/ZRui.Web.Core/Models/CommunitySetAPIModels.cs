using System.Collections.Generic;

namespace ZRui.Web.CommunitySetAPIModels
{

    public class GetListArgsModel : CommunityArgsModel
    {
    }

    public class GetListModel
    {
        public IList<CommunityBase> Items { get; set; }
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
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class AddArgsModel : CommunityArgsModel
    {
        public string Name { get; set; }
    }

    public class AddModel
    {
        public string Flag { get; set; }
    }

    public class GetSingleArgsModel : CommunityArgsModel
    {
        public string CurrentCommunityFlag { get; set; }
    }

    public class GetSingleModel
    {
        public string Flag { get; set; }
        public string Name { get; set; }
        public string Ico { get; set; }
        public List<CommunityMember> Members { get; set; }
        public List<CommunityApp> Apps { get; set; }

        public List<string> Managers { get; set; }
    }

    public class AddMemberArgsModel : CommunityArgsModel
    {
        public string Username { get; set; }
        public string CurrentCommunityFlag { get; set; }
    }

    public class RemoveMemberArgsModel : CommunityArgsModel
    {
        public string Username { get; set; }
        public string CurrentCommunityFlag { get; set; }
    }

    public class AddManagerArgsModel : CommunityArgsModel
    {
        public string Username { get; set; }
        public string CurrentCommunityFlag { get; set; }
    }

    public class RemoveManagerArgsModel : CommunityArgsModel
    {
        public string Username { get; set; }
        public string CurrentCommunityFlag { get; set; }
    }

    public class RemoveAppArgsModel : CommunityArgsModel
    {
        public string CurrentCommunityFlag { get; set; }
        public string CurrentAppFlag { get; set; }
    }

    public class AddAppArgsModel : CommunityArgsModel
    {
        public string Name { get; set; }
        public string Ico { get; set; }
        public string Url { get; set; }
        public string CurrentCommunityFlag { get; set; }
    }

    public class GetMembersArgsModel : CommunityArgsModel
    {
        public string CurrentCommunityFlag { get; set; }
    }

    public class GetMembersModel
    {
        public List<CommunityMember> Members { get; set; }
    }

    public class SetCommunityNameArgsModel : CommunityArgsModel
    {
        public string Name { get; set; }
        public string CurrentCommunityFlag { get; set; }
    }

    public class SetCommunityBaseArgsModel : CommunityArgsModel
    {
        public string Name { get; set; }
        public string Ico { get; set; }
        public string CurrentCommunityFlag { get; set; }
    }

    public class SetAppNameArgsModel : CommunityArgsModel
    {
        public string Name { get; set; }
        public string CurrentCommunityFlag { get; set; }
        public string CurrentAppFlag { get; set; }
    }

    public class SetAppBaseArgsModel : CommunityArgsModel
    {
        public string Name { get; set; }
        public string Ico { get; set; }
        public string CurrentCommunityFlag { get; set; }
        public string CurrentAppFlag { get; set; }
    }

    public class SetAppIsDisabledArgsModel : CommunityArgsModel
    {
        public bool IsDisabled { get; set; }
        public string CurrentCommunityFlag { get; set; }
        public string CurrentAppFlag { get; set; }
    }

    public class SetAppIsDefaultOpenArgsModel : CommunityArgsModel
    {
        public bool IsDefaultOpen { get; set; }
        public string CurrentCommunityFlag { get; set; }
        public string CurrentAppFlag { get; set; }
    }

    public class GetAppSettingsArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// CommunityFlag
        /// </summary>
        public string CurrentCommunityFlag { get; set; }
        public string CurrentAppFlag { get; set; }
    }

    public class SetAppSettingsArgsModel : CommunityArgsModel
    {
        /// <summary>
        /// CommunityFlag
        /// </summary>
        public string CurrentCommunityFlag { get; set; }
        public string CurrentAppFlag { get; set; }
        public CommunityAppSettings Settings { get; set; }
    }

}