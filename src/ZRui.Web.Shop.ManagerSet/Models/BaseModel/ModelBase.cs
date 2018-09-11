using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    public class ModelBase
    {
        public int Id { get; set; }
        public bool IsDel { get; set; }


        DateTime _CreateTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                if (_CreateTime.Year == 0001)
                {
                    return DateTime.Now;
                }
                return _CreateTime ;
            }
            set
            {

                _CreateTime = value;
            }
        }

    }
}
