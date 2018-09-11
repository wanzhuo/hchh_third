using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HuiChiHuiHe.Auth
{
    public class WebAPIResult
    {
       
      
        public bool Success { get; set; }    
        public int ErrorCode { get; set; }      
        public string Message { get; set; }
    }
    }
