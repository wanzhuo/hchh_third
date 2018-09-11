using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopCommodityStockSetAPIModels
{
    /// <summary>
    /// ��ȡ�б������
    /// </summary>
    public class GetListArgsModel 
    {
        public int? ShopId { get; set; }
        public int CommodityId { get; set; }
    }

    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<RowItem> Items { get; set; }
    }


    /// <summary>
    /// ��
    /// </summary>
    public class RowItem : ShopCommodityStock
    {

    }

    /// <summary>
    /// ����ǰ��Ʒ�����еĿ������Ϊɾ��״̬
    /// </summary>
    public class SetAllIsDeleteArgsModel
    {
        public int? shopId { get; set; }
        public int? commodityId { get; set; }
    }
    /// <summary>
    /// ��Ӳ�����
    /// </summary>
    public class AddArgsModel 
    {
        /// <summary>
        /// ����Id
        /// </summary>
        public int? ShopId { get; set; }
        /// <summary>
        /// ��ƷId
        /// </summary>
        public int CommodityId { get; set; }
        /// <summary>
        /// SkuId
        /// </summary>
        public int SkuId { get; set; }
    }

    public class UpdateArgsModel 
    {
        /// <summary>
        /// ���
        /// </summary>
        public int Id { get; set; }
        // <summary>
        /// ���
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// �ɱ��ۣ���λ�Ƿ�
        /// </summary>
        public int CostPrice { get; set; }
        /// <summary>
        /// ���ۼۣ���λ�Ƿ�
        /// </summary>
        public int SalePrice { get; set; }
        /// <summary>
        /// �г��ۣ���λ�Ƿ�
        /// </summary>
        public int MarketPrice { get; set; }
    }

    public class GetSkuItemsArgsModel 
    {
        public int? CommodityId { get; set; }
    }

    public class GetSkuItemsModel
    {
        public IList<SkuItem> Items { get; set; }
        public IList<Sku> Skus
        {
            get
            {
                return Items.OrderBy(m => m.SkuId).GroupBy(m => m.SkuId).Select(g => new Sku
                {
                    Id = g.Key,
                    Values = g.OrderBy(m => m.SkuId).Select(m => m.Value).ToList()
                }).ToList();
            }
        }
    }

    public class Sku
    {
        public int Id { get; set; }
        public IList<string> Values { get; set; }
    }

    public class SkuItem
    {
        public int SkuId { get; set; }
        public int Id { get; set; }
        public string Value { get; set; }
    }
}