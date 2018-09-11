using System.Collections.Generic;

namespace ZRui.Web.MemberModels
{

    public class LoginModel
    {
        public string ReturnUrl { get; set; }
        public MemberAPIOptions MemberAPIOptions { get; set; }
    }

    public class IndexModel
    {
        public MemberAPIOptions MemberAPIOptions { get; set; }
    }
}