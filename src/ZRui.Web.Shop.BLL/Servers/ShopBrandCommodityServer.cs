using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZRui.Web.BLL.Servers
{
    public class ShopBrandCommodityServer
    {
        private ShopDbContext db;
        private Shop mShop;
        private DiningWay diningWay;
        private int shopId;
        private double memberDiscount;
        private bool hasDiscount = false;

        public ShopBrandCommodityServer(ShopDbContext db, DiningWay diningWay)
        {
            this.db = db;
            this.diningWay = diningWay;
        }



        public ShopBrandCommodityServer(ShopDbContext db, string shopFlag, DiningWay diningWay)
        {
            this.db = db;
            mShop = db.Query<Shop>()
                .Where(m => !m.IsDel)
                .Where(m => m.Flag == shopFlag)
                .FirstOrDefault();
            this.diningWay = diningWay;
        }


        public ShopBrandCommodityServer(ShopDbContext db, Shop shop, DiningWay diningWay)
        {
            this.db = db;
            mShop = shop;
            this.diningWay = diningWay;
        }

        /// <summary>
        /// 获取类别树
        /// </summary>
        /// <returns></returns>
        public GetCategoryTreeModel GetCategoryTree()
        {
            
            int shopBrandId = mShop.ShopBrandId;
            shopId = mShop.Id;
            if (shopBrandId <= 0) throw new Exception("纪录不存在");
            var query = db.Query<ShopBrandCommodityCategory>()
                     .Where(m => !m.IsDel)
                     .Where(m => m.ShopBrandId == shopBrandId);

            var list = query
                .OrderByDescending(m => m.Id)
                .Select(m => new CategoryItem()
                {
                    Id = m.Id,
                    Name = m.Name,
                    OrderWeight = m.OrderWeight,
                    PId = m.PId,
                    Flag = m.Flag,
                    Ico = m.Ico,
                })
                .ToList();
            return new GetCategoryTreeModel(list);
        }


        public GetComboCommoditysModel GetComboList()
        {
            return GetComboList(this.shopId);
        }

        /// <summary>
        /// 获取固定套餐列表
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="diningWay"></param>
        public GetComboCommoditysModel GetComboList(int shopId)
        {
            var query = db.Query<ShopCommodityStock>()
                 .Where(m => !m.IsDel)
                 .Where(m => m.ShopId == shopId)
                 .Where(m => m.Sku.Commodity.CategoryId == 0);
            switch (diningWay)
            {
                case DiningWay.堂食:
                    query = query.Where(m => m.Sku.Commodity.IsScanCode);
                    break;
                case DiningWay.外卖:
                    query = query.Where(m => m.Sku.Commodity.IsTakeout);
                    break;
                case DiningWay.自助:
                    query = query.Where(m => m.Sku.Commodity.IsSelfOrder);
                    break;
                default:
                    break;
            }
            //这里获得指定店铺的库存及价格及关联的Sku
            var stockItems = query
                 .Select(m => new StockRowItem
                 {
                     SkuId = m.Sku.Id,
                     SkuFlag = m.Sku.Flag,
                     CommodityId = m.Sku.CommodityId,
                     SalePrice = m.SalePrice,
                     Cover = m.Sku.Commodity.Cover,
                     Name = m.Sku.Commodity.Name,
                     //MarketPrice = m.MarketPrice,
                     Stock = m.Stock
                 })
                 .ToList();

            var viewModel = new GetComboCommoditysModel();
            //获取库存关联的商铺信息，主要是一些描述
            var commodityIds = stockItems.Select(m => m.CommodityId).Distinct().ToList();
            var commodities = db.Query<ShopBrandCommodity>().Where(m => !m.IsDel)
                .Where(m => commodityIds.Contains(m.Id));
            viewModel.Items = commodities.Select(m => new ComboRowItem()
            {
                Id = m.Id,
                Cover = m.Cover,
                Detail = m.Detail,
                Name = m.Name,
                SalesForMonth = m.SalesForMonth,
                Upvote = m.Upvote
            }).ToList();
            //获取库存关联的规格的规格项
            var skuIds = stockItems.Select(m => m.SkuId).Distinct().ToList();
            var skuItems = db.Query<ShopBrandCommoditySkuItem>()
                .Where(m => !m.IsDel)
                .Where(m => skuIds.Contains(m.SkuId))
                .OrderByDescending(m => m.Id)
                .Select(m => new SkuItem()
                {
                    Id = m.Id,
                    SkuId = m.SkuId,
                    Value = m.ParameterValue.Value,
                    ParameterValueId = m.ParameterValueId,
                    ParameterId = m.ParameterId,
                    ParameterName = m.Parameter.Name
                })
                .ToList();

            //循环每一个商品

            foreach (var item in viewModel.Items)
            {
                //获得商品的sku id列表，用于下面获取商品的skuItems
                var commoditySkuIds = stockItems.Where(m => m.CommodityId == item.Id).Select(m => m.SkuId).Distinct().ToList();
                var stock = stockItems.Where(m => m.CommodityId == item.Id).First();
                item.Skus = skuItems.Where(m => commoditySkuIds.Contains(m.SkuId)).GroupBy(m => m.SkuId).Select(m => new CommoditySku(m.Key, m.ToList(), stockItems.Where(x => x.CommodityId == item.Id && x.SkuId == m.Key).FirstOrDefault())).ToList();
                item.Parameters = skuItems.Where(m => commoditySkuIds.Contains(m.SkuId)).GroupBy(m => m.ParameterId).Select(m => new CommodityParameter(m.ToList())).ToList();
                GetComboContent(item);
            }
            return viewModel;
        }

        /// <summary>
        /// 获取固定套餐详细内容，供小程序购物车显示
        /// </summary>
        private void GetComboContent(ComboRowItem comboRowItem)
        {
            var list = db.Query<ShopOrderComboItem>()
                .Where(m => !m.IsDel)
                .Where(m => m.Pid == comboRowItem.Id)
                .Select(m => m.Commodity)
                .Distinct()
                .GroupBy(m=>m.Category)
                .ToList();
            var comboContents = new List<ComboContent>();
            foreach (var item in list)
            {
                comboContents.Add(new ComboContent()
                {
                    Category = item.Key.Name,
                    CommoditysName = item.Select(m => m.Name).ToList()
                });
            }
            comboRowItem.Contents = comboContents;
        }

        /// <summary>
        /// 类别与商品集合
        /// </summary>
        /// <returns></returns>
        public CategoryAndCommodityModel GetCategoryAndCommodity(double? discount = null)
        {
            if (discount.HasValue)
            {
                hasDiscount = true;
                memberDiscount = discount.Value;
            }
            
            var categoryTree = GetCategoryTree();
            var nodes = categoryTree.Tree;
            List<TreeNode> resultNodes = new List<TreeNode>();
            List<GetCommoditysModel> commodities = new List<GetCommoditysModel>();
            foreach (TreeNode node in nodes)
            {
                GetCommoditysModel commodity = GetCommodity(node);
                if (commodity.Items.Count > 0)
                {
                    commodities.Add(commodity);
                    resultNodes.Add(node);
                }
            }
            commodities.Reverse();
            resultNodes.Reverse();
            return new CategoryAndCommodityModel()
            {
                Commodities = commodities,
                Nodes = resultNodes
            };

        }

        public GetCommoditysModel GetCommodity(TreeNode node)
        {
            string categoryFlag = node.Flag;
            bool isFb =categoryFlag == "fb"; //火爆分类
            var query = db.Query<ShopCommodityStock>()
                 .Where(m => !m.IsDel)
                 .Where(m => m.Shop.Flag == mShop.Flag);
            if (isFb)   //火爆分类
            {
                query = query.Where(m => m.Sku.Commodity.CategoryId != 0);
            }
            else if (!string.IsNullOrEmpty(categoryFlag))
            {
                query = query.Where(m => m.Sku.Commodity.Category.Flag == categoryFlag);
            }
            //这里获得指定店铺的库存及价格及关联的Sku
            List<StockRowItem> stockItems;
            if (hasDiscount)
            {
                stockItems = query
                .Select(m => new StockRowItem
                {
                    SkuId = m.Sku.Id,
                    SkuFlag = m.Sku.Flag,
                    HasVipPrice = m.Sku.Commodity.UseMemberPrice,
                    CommodityId = m.Sku.CommodityId,
                    PrimePrice = m.SalePrice,
                    SalePrice = m.Sku.Commodity.UseMemberPrice ? GetMemberPrice(m.SalePrice) : m.SalePrice,
                    Stock = m.Stock
                })
                .ToList();

            }else
            {
                stockItems = query
                .Select(m => new StockRowItem
                {
                    SkuId = m.Sku.Id,
                    SkuFlag = m.Sku.Flag,
                    CommodityId = m.Sku.CommodityId,
                    SalePrice = m.SalePrice,
                    HasVipPrice = false,
                    PrimePrice = m.SalePrice,
                    Stock = m.Stock
                 })
                 .ToList();
            }

            var viewModel = new GetCommoditysModel();
            //获取库存关联的商铺信息，主要是一些描述
            var commodityIds = stockItems.Select(m => m.CommodityId).Distinct().ToList();
            var commodities = db.Query<ShopBrandCommodity>().Where(m => !m.IsDel)
                .Where(m => commodityIds.Contains(m.Id));
            switch (diningWay)
            {
                case DiningWay.自助:
                    commodities = commodities.Where(m => m.IsSelfOrder);
                    break;
                case DiningWay.堂食:
                    commodities = commodities.Where(m => m.IsScanCode);
                    break;
                case DiningWay.外卖:
                    commodities = commodities.Where(m => m.IsTakeout);
                    break;
                default:
                    break;
            }
            if (isFb)
                commodities = commodities.OrderByDescending(m => m.SalesForMonth).Take(10);
            viewModel.Items = commodities.Select(m => new CommodityRowItem()
            {
                Id = m.Id,
                Cover = m.Cover,
                Detail = m.Detail,
                Name = m.Name,
                SalesForMonth = m.SalesForMonth,
                Upvote = m.Upvote
            }).ToList();
            //获取库存关联的规格的规格项
            var skuIds = stockItems.Select(m => m.SkuId).Distinct().ToList();
            var skuItems = db.Query<ShopBrandCommoditySkuItem>()
                .Where(m => !m.IsDel)
                .Where(m => skuIds.Contains(m.SkuId))
                .OrderByDescending(m => m.Id)
                .Select(m => new SkuItem()
                {
                    Id = m.Id,
                    SkuId = m.SkuId,
                    Value = m.ParameterValue.Value,
                    ParameterValueId = m.ParameterValueId,
                    ParameterId = m.ParameterId,
                    ParameterName = m.Parameter.Name
                })
                .ToList();

            //循环每一个商品
            foreach (var item in viewModel.Items)
            {
                //获得商品的sku id列表，用于下面获取商品的skuItems
                var commoditySkuIds = stockItems.Where(m => m.CommodityId == item.Id).Select(m => m.SkuId).Distinct().ToList();
                //
                var stock = stockItems.Where(m => m.CommodityId == item.Id).First();
                item.Skus = skuItems.Where(m => commoditySkuIds.Contains(m.SkuId))
                    .GroupBy(m => m.SkuId)
                    .Select(m => new CommoditySku(m.Key, m.ToList(), stockItems.Where(x => x.CommodityId == item.Id && x.SkuId == m.Key).FirstOrDefault()))
                    .OrderBy(m => m.SalePrice)
                    .ToList();

                item.Parameters = skuItems.Where(m => commoditySkuIds.Contains(m.SkuId)).GroupBy(m => m.ParameterId).Select(m => new CommodityParameter(m.ToList())).ToList();
            }
            return viewModel;
        }

        //单品会员价
        private int GetMemberPrice(int price)
        {
            return ShopMemberServer.GetMemberPrice(price, memberDiscount);
        }

    }


    //public enum DiningWay
    //{
    //    自助点餐 = 0,
    //    扫码点餐 = 1,
    //    外卖 = 2
    //}

    public class CommodityRowitem
    {
        public string Name { get; set; }
        public int SalesForMonth { get; set; }
        public int Upvote { get; set; }
        public int SalePrice { get; set; }
        public string Cover { get; set; }
        public int MarketPrice { get; set; }
        public string Detail { get; set; }
    }

    public class StockRowItem
    {
        public int SkuId { get; set; }
        public string SkuFlag { get; set; }
        public int CommodityId { get; set; }
        public int SalePrice { get; set; }
        public bool HasVipPrice { get; set; }
        public int PrimePrice { get; set; }
        public string Cover { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
    }



    public class GetListModel
    {
        [JsonProperty("items")]
        public IList<CommodityRowitem> Items { get; set; }
    }

    public class GetSkuItemsModel
    {
        public IList<SkuItem> Items { get; set; }
        public IList<Sku> Skus
        {
            get
            {
                return Items.GroupBy(m => m.SkuId).Select(g => new Sku
                {
                    Id = g.Key,
                    Values = g.OrderBy(m => m.Id).Select(m => m.Value).ToList()
                }).ToList();
            }
        }
    }


    public class GetComboCommoditysModel
    {
        public IList<ComboRowItem> Items { get; set; }
    }

    public class GetCommoditysModel
    {
        public IList<CommodityRowItem> Items { get; set; }
    }


    public class ComboContent
    {
        public string Category { get; set; }
        public List<string> CommoditysName { get; set; }
    }

    //固定套餐单条数据
    public class ComboRowItem: CommodityRowItem
    {
        public List<ComboContent> Contents { get; set; }
    }

    public class CommodityRowItem
    {
        public List<FullCut> FullCuts { get; set; }
        public string Name { get; set; }
        public int SalesForMonth { get; set; }
        public int Upvote { get; set; }
        public string Cover { get; set; }
        public string Detail { get; set; }
        public bool HasVipPrice { get; set; }
        public IList<CommoditySku> Skus { get; set; }
        public IList<CommodityParameter> Parameters { get; set; }
        public int Id { get; set; }
    }

    //满减
    public class FullCut
    {
        public int Count { get; set; }  //满多少
        public int Amount { get; set; }  //减多少
    }

    public class CommoditySku
    {
        public int SkuId { get; set; }
        public string SkuFlag { get; set; }
        public int Stock { get; set; }
        public int SalePrice { get; set; }
        public int PrimePrice { get; set; }
        public bool HasVipPrice { get; set; }
        public IList<SkuItem> Items { get; set; }

        public CommoditySku(int skuId, IList<SkuItem> items, StockRowItem stock)
        {
            this.SkuId = skuId;
            this.Items = items;
            if (stock != null)
            {
                this.SkuFlag = stock.SkuFlag;
                this.Stock = stock.Stock;
                this.SalePrice = stock.SalePrice;
                this.PrimePrice = stock.PrimePrice;
                this.HasVipPrice = stock.HasVipPrice;
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
        public int ParameterId { get; set; }
        public string ParameterName { get; set; }
        public int ParameterValueId { get; set; }
    }

    public class CommodityParameter
    {
        public CommodityParameter(IList<SkuItem> skuItems)
        {
            Id = skuItems.First().Id;
            Name = skuItems.First().ParameterName;
            Values = skuItems.GroupBy(m => m.ParameterValueId).Select(m => new CommodityParameterValues()
            {
                Id = m.First().ParameterValueId,
                Value = m.First().Value
            }).ToList();
        }
        public string Name { get; set; }
        public int Id { get; set; }
        public IList<CommodityParameterValues> Values { get; set; }
    }

    public class CommodityParameterValues
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }


    public class GetShopCommoditiesAndCombos
    {
        public CategoryAndCommodityModel CategoryAndCommodity{ get; set; }
        public GetComboCommoditysModel Commodity { get; set; }
    }

    public class CategoryAndCommodityModel
    {
        public List<TreeNode> Nodes { get; set; }
        public List<GetCommoditysModel> Commodities { get; set; }
    }


    public class CategoryItem
    {
        public virtual Int32 Id { get; set; }
        public virtual String Name { get; set; }
        public virtual float OrderWeight { get; set; }
        public virtual String Detail { get; set; }
        [JsonProperty("pid")]
        public virtual Int32? PId { get; set; }
        public virtual String Flag { get; set; }
        public virtual bool IsDel { get; set; }
        public string Ico { get; set; }
    }


    public class GetCategoryTreeModel
    {
        [JsonProperty("tree")]
        public List<TreeNode> Tree { get; set; }
        [JsonIgnore]
        public List<TreeNode> AllNodes { get; set; }
        public GetCategoryTreeModel(List<CategoryItem> items)
        {
            AllNodes = items.Select(m => new TreeNode(this)
            {
                Name = m.Name,
                Detail = m.Detail,
                Flag = m.Flag,
                Id = m.Id,
                OrderWeight = m.OrderWeight,
                PId = m.PId,
                Ico = m.Ico
            }).ToList();

            Tree = AllNodes.Where(m => !m.PId.HasValue || m.PId == 0).OrderByDescending(m => m.OrderWeight).ToList();
            Tree.Add(new TreeNode(this)
            {
                Id = -1,
                Name = "火爆",
                Flag = "fb"
            });

        }
    }

    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title
        {
            get
            {
                return Name;
            }
        }
        public List<TreeNode> Nodes
        {
            get
            {
                return root.AllNodes.Where(m => m.PId == Id).OrderByDescending(m => m.OrderWeight).ToList();
            }
        }
        public float OrderWeight { get; set; }
        public string Detail { get; set; }
        public int? PId { get; set; }
        public string Flag { get; set; }
        public string Ico { get; set; }

        GetCategoryTreeModel root;
        public TreeNode(GetCategoryTreeModel root)
        {
            this.root = root;
        }
    }

}
