using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HuiChiHuiHe.Auth
{
    public class TaskLog 
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public string ExeResult { get; set; }
        public DateTime AddTime { get; set; }

    }
}
