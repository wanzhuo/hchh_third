using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZRui.Web
{
    public class MemberAPIOptions
    {
        public string[] Roots { get; set; }
        public bool IsRoot(string username)
        {
            return Roots != null && Roots.Contains(username);
        }
        public string Host { get; set; }
        public string Title { get; set; }
        public string SimpleTitle { get; set; }
    }
}
