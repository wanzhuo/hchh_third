using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopPartSetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class ShopIdArgsModel
    {
        public int? ShopId { get; set; }
    }

    public class GetListArgsModel: ShopIdArgsModel
    {
    }

    public class GetListModel
    {
        public IList<RowItem> Items { get; set; }
    }

    public class GetPagedListArgsModel : GetListArgsModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string OrderName { get; set; }
        public string OrderType { get; set; }
    }
    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : ShopPart
    {

    }

    public class GetPagedListModel : GetListModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }

    public class AddArgsModel
    {
        public int? ShopId { get; set; }
        public String Title { get; set; }
        public String Detail { get; set; }
    }

    public class UpdateArgsModel
    {
        public Int32 Id { get; set; }
        public String Title { get; set; }
        public String Detail { get; set; }
    }

    public class InitQRCodeJumpArgsModel : ShopIdArgsModel
    {
        /// <summary>
        /// ���Է�Χ��1Ϊ�����棨����ֻ�Կ�������Ч��2Ϊ����棨���öԹ���Ա����������Ч��3Ϊ���ϰ汾�����öԹ���Ա�������ߺ���������Ч��
        /// </summary>
        public string OpenVersion { get; set; }
        public string Path { get; set; }
        public List<string> DebugUrl { get; set; }
    }
}