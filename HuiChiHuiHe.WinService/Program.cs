using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HuiChiHuiHe.WinService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {          
            using (HchhConfigContext context = new HchhConfigContext())
            {
                List<TaskConfig> list = context.TaskConfig.Where(p => p.Status == 1).ToList();
                if (list != null && list.Count > 0)
                {
                    List<AutoTask> tasks = new List<AutoTask>();
                    list.ForEach(p =>
                    {
                        tasks.Add(new AutoTask(p));
                    });

                    ServiceBase.Run(tasks.ToArray());
                }
            }


        }
    }
}
