using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Utils
{
    public static class ExOrderStatus
    {
        public static string ExOrderStatusAction(ShopOrderStatus orderStatus) {
            string value = string.Empty;
            switch (orderStatus)
            {
                case ShopOrderStatus.待成团:
                    value = "待成团";
                    break;
                case ShopOrderStatus.待自提:
                    value = "待自提";
                    break;
                case ShopOrderStatus.待配送:
                    value = "待配送";
                    break;
                case ShopOrderStatus.已完成:
                    value = "已完成";
                    break;
                case ShopOrderStatus.已取消:
                    value = "已取消";
                    break;
                case ShopOrderStatus.已确认:
                    value = "已确认";
                    break;
                case ShopOrderStatus.已退款:
                    value = "已退款";
                    break;
                case ShopOrderStatus.未处理:
                    value = "未处理";
                    break;
                case ShopOrderStatus.已支付:
                    value = "已支付";
                    break;
                case ShopOrderStatus.待支付:
                    value = "待支付";
                    break;
                case ShopOrderStatus.已打印:
                    value = "已打印";
                    break;
                case ShopOrderStatus.加急申请中:
                    value = "加急申请中";
                    break;
                case ShopOrderStatus.申请发票:
                    value = "申请发票";
                    break;
                case ShopOrderStatus.发票待领取:
                    value = "发票待领取";
                    break;
                case ShopOrderStatus.已领取:
                    value = "已领取";
                    break;
                case ShopOrderStatus.配送中:
                    value = "配送中";
                    break;
                case ShopOrderStatus.待取消:
                    value = "待取消";
                    break;
                case ShopOrderStatus.退款审批:
                    value = "退款审批";
                    break;
                case ShopOrderStatus.退款中:
                    value = "退款中";
                    break;
                case ShopOrderStatus.自提中:
                    value = "自提中";
                    break;
                case ShopOrderStatus.待接单:
                    value = "待接单";
                    break;
                case ShopOrderStatus.待取货:
                    value = "待取货";
                    break;
                default:
                    value = "未知";
                    break;
            }
            return value;

        }
    }
}
