using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZRui.Web.ShopManager.ShopBrandCommoditySkuSetAPIModels
{
    /// <summary>
    /// 获取列表参数类
    /// </summary>
    public class GetListArgsModel 
    {
        public int? CommodityId { get; set; }
    }

    public class GetListModel
    {
        public IList<RowItem> Items { get; set; }
        private IList<SkuParameter> _skuParameters;
        public IList<SkuParameter> SkuParameters
        {
            get
            {
                if (_skuParameters == null)
                {
                    _skuParameters = new List<SkuParameter>();
                    foreach (var parameterId in Items.Select(m => m.ParameterId).Distinct())
                    {
                        _skuParameters.Add(new SkuParameter()
                        {
                            Id = parameterId,
                            Name = Items.Where(m => m.ParameterId == parameterId).First().ParameterName,
                            Values = Items.Where(m => m.ParameterId == parameterId).GroupBy(m=>m.ParameterValueId).Select(m => new SkuParameterValue()
                            {
                                Id = m.First().ParameterValueId,
                                Value = m.First().ParameterValue
                            }).ToList()
                        });
                    }
                }
                return _skuParameters;
            }
        }
    }


    public class SkuParameter
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<SkuParameterValue> Values { get; set; }
    }
    public class SkuParameterValue
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// 行
    /// </summary>
    public class RowItem
    {
        public string ParameterName { get; internal set; }
        public int ParameterId { get; internal set; }
        public string ParameterValue { get; internal set; }
        public int ParameterValueId { get; internal set; }
    }

    public class UpdateArgsModel 
    {
        /// <summary>
        /// 商品Id
        /// </summary>
        public int? CommodityId { get; set; }
        /// <summary>
        /// 参数Id
        /// </summary>
        public List<int> ParameterIds { get; set; }
    }

    public class GetParameterItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}