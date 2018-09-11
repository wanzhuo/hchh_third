using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    public class GetPagedListBaseModel
    {
        int _PageIndex;
        public int PageIndex
        {
            get
            {
                if (_PageIndex == 0)
                {
                    return 1;
                }
                return _PageIndex;
            }
            set
            {
                _PageIndex = value;
            }
        }
        int _PageSize;
        public int PageSize
        {
            get
            {
                if (_PageSize <= 0)
                {
                    return 1;
                }
                return _PageSize;
            }
            set
            {
                _PageSize = value;
            }
        }


        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string KeyWord { get; set; } = "";

        /// <summary>
        /// 商铺Id
        /// </summary>
        public int ShopId { get; set; }
    }
}
