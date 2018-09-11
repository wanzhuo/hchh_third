namespace ZRui.Web.ShopTOOrderAPIModels
{
    public class AddAddressArgsModel : MemberAddress
    {
        public int? ShopId { get; set; }

        public int IsConglomeration { get; set; } = 1;  //1是外卖 2是拼团
    }

    public class SetIsUsedArgs
    {
        public int? shopId { get; set; }
        public int id { get; set; }
        public int IsConglomeration { get; set; } = 1;  //1是外卖 2是拼团
    }

    public class ReGeocoderArgsModel
    {
        public double? lat { get; set; }
        public double? lng { get; set; }
    }

    public class HasTakeOutArgsModels
    {
        public int? ShopId { get; set; }
    }

    public class GetTakeOutInfoModel
    {
        public bool IsUseTakeOut { get; set; }
        public bool IsOpen { get; set; }
        public double MinAmount { get; set; }
        public double BoxFee { get; set; }
        public double DeliveryFee { get; set; }
    }

    public class IsInScopeArgsModels
    {
        public int? shopId { get; set; }
        //纬度
        public double? lat { get; set; }
        //经度
        public double? lng { get; set; }
    }


    public class CanUsedArgsModels
    {
        public int? shopId { get; set; }
    }

}
