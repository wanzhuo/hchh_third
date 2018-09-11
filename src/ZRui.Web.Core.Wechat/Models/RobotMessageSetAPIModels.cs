using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZRui.Web.Core.Wechat;

namespace ZRui.Web.RobotMessageSetAPIModels
{

    public class GetListArgsModel:CommunityArgsModel
    {
        public string Question { get; set; }
        public RobotMessageQuestionType? QuestionType { get; set; }
        public RobotMessageStatus? Status { get; set; }
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
        public long Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public RobotMessageQuestionType QuestionType { get; set; }
        public RobotMessageStatus Status { get; set; }
    }

    public class AddArgsModel : CommunityArgsModel
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public RobotMessageQuestionType QuestionType { get; set; }
    }



    public class UpdateArgsModel : CommunityArgsModel
    {
        public virtual Int32 Id { get; set; }
        public string Answer { get; set; }
        public RobotMessageQuestionType QuestionType { get; set; }
    }

    public class GetSingleArgsModel : CommunityArgsModel
    {
        public int Id { get; set; }
    }

    public class GetSingleModel
    {
        public long Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public RobotMessageQuestionType QuestionType { get; set; }
        public RobotMessageStatus Status { get; set; }
    }

    public class SetIsDeleteArgsModel : CommunityArgsModel
    {
        public int Id { get; set; }
    }

    public class SetStatusArgsModel : CommunityArgsModel
    {
        public int Id { get; set; }
        public RobotMessageStatus Status { get; set; }
    }

    public class SetAnswerArgsModel : CommunityArgsModel
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}