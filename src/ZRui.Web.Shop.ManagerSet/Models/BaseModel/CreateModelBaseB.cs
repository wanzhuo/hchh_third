using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    public class CreateModelBaseB
    {
        public int Id { get; set; }
        public bool IsDel { get; set; }

        /// <summary>
        /// 添加时的Ip
        /// </summary>
        public string AddIp { get; set; }

        DateTime _AddTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime AddTime
        {
            get
            {
                if (_AddTime.Year == 0001)
                {
                    return DateTime.Now;
                }
                return _AddTime;
            }
            set
            {

                _AddTime = value;
            }
        }

    }
}
