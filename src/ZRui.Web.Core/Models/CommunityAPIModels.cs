using System.Collections.Generic;
using System.Linq;

namespace ZRui.Web.CommunityAPIModels
{

    public class GetListArgsModel
    {
        public string ParentFlag { get; set; }
    }

    public class GetListModel
    {
        public IList<CommunityBase> Items { get; set; }
    }

    public class GetListForVueRouteModel
    {
        public IList<VueRouteItem> Items { get; set; }
        public GetListForVueRouteModel(IList<Community> communitys)
        {
            Items = new List<VueRouteItem>();
            foreach (var community in communitys)
            {
                var model = new VueRouteItem()
                {
                    Name = community.Name,
                    Path = $"/{community.Flag}",
                    Children = new List<VueRouteItem>(),
                    Meta = new Dictionary<string, object>() {
                        {"title",community.Name},
                        { "icon",community.Ico}
                    }
                };
                foreach (var app in community.Apps.Where(m => !m.IsDisabled))
                {
                    //url格式形如 /memberSet/index
                    //Name形如 memberSet_index
                    var name = app.Url.TrimStart('/').Replace('/', '_');
                    model.Children.Add(new VueRouteItem()
                    {
                        Name = name,
                        Path = $"{app.Flag}",
                        Component = name,
                        Meta = new Dictionary<string, object>() {
                            { "title",app.Name},
                            { "communityFlag",community.Flag},
                            { "appFlag",app.Flag},
                            { "icon",app.Ico},
                            { "defaultOpen",app.IsDefaultOpen}
                        }
                    });
                }

                Items.Add(model);
            }
        }
    }

    public class VueRouteItem
    {
        public string Path { get; set; }
        public string Component { get; set; }
        public string Redirect { get; set; }
        public string Name { get; set; }
        public Dictionary<string, object> Meta { get; set; }
        public IList<VueRouteItem> Children { get; set; }
    }

    public class GetAppNavigatesArgsModel
    {
        public string Flag { get; set; }
    }

    public class GetSingleArgsModel
    {
        public string CommunityFlag { get; set; }
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

    public class AddMemberArgsModel
    {
        public string CommunityFlag { get; set; }
        public string Username { get; set; }
    }

    public class RemoveMemberArgsModel
    {
        public string CommunityFlag { get; set; }
        public string Username { get; set; }
    }

    public class AddManagerArgsModel
    {
        public string CommunityFlag { get; set; }
        public string Username { get; set; }
    }

    public class RemoveManagerArgsModel
    {
        public string CommunityFlag { get; set; }
        public string Username { get; set; }
    }

    public class AddAppArgsModel
    {
        public string CommunityFlag { get; set; }
        public string MyAppFlag { get; set; }
    }

    public class GetMembersArgsModel
    {
        public string CommunityFlag { get; set; }
    }

    public class GetMembersModel
    {
        public List<CommunityMember> Members { get; set; }
    }

    public class SetCommunityNameArgsModel
    {
        public string CommunityFlag { get; set; }
        public string Name { get; set; }
    }

    public class SetCommunityBaseArgsModel
    {
        public string CommunityFlag { get; set; }
        public string Name { get; set; }
        public string Ico { get; set; }
    }

    public class SetAppNameArgsModel
    {
        public string CommunityFlag { get; set; }
        public string AppFlag { get; set; }
        public string Name { get; set; }
    }
    public class SetAppBaseArgsModel
    {
        public string CommunityFlag { get; set; }
        public string AppFlag { get; set; }
        public string Name { get; set; }
        public string Ico { get; set; }
    }
    public class SetAppIsDisabledArgsModel
    {
        public string CommunityFlag { get; set; }
        public string AppFlag { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class GetAppSettingsArgsModel
    {
        public string CommunityFlag { get; set; }
        public string AppFlag { get; set; }
    }

    public class SetAppSettingsArgsModel
    {
        public string CommunityFlag { get; set; }
        public string AppFlag { get; set; }
        public CommunityAppSettings Settings { get; set; }
    }

}