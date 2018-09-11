using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZRui.Web
{
   public class SystemVersion 
    {
        [Key]
        public string VersionCode { get; set; }
        public string VersionDesc { get; set; }
      

    }
}
