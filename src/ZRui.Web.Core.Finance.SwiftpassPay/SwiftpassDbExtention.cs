using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web.Core.Finance.SwiftpassPay
{
    public static class SwiftpassDbExtention
    {
        public static void SetFinish(this MemberTradeForRechange rechange,DbContext db, SwiftpassPayResponseHandler result)
        {
            if (result.TradeState != "NOTPAY")
            {
                if (rechange.Status == MemberTradeForRechangeStatus.未完成)
                {
                    if (rechange.TotalFee != result.TotalFee) throw new Exception("指定的金额不对应");
                    rechange.OutBank = result.Xml;
                    rechange.MechanismTradeNo = result.TransactionId;
                    switch (result.TradeState)
                    {
                        case "SUCCESS":
                            db.SetMemberTradeForRechangeSuccess(rechange);
                            break;
                        case "CLOSED":
                            rechange.Status = MemberTradeForRechangeStatus.取消;
                            break;
                        default:
                            db.SetMemberTradeForRechangeFail(rechange, result.TradeState);
                            break;
                    }
                }
            }
        }
    }
}
