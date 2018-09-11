namespace ZRui.Web.Core.Finance.PayBase
{
    public interface IPayOption
    {
        string Key { get; set; }
        string OrderUrl { get; set; }
        string OrderQueryUrl { get; set; }
        string NotifyUrl { get; set; }
        string Version { get; set; }
        string WftPublicKey { get; set; }
    }
}
