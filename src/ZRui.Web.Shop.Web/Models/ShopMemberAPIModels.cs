using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZRui.Web.BLL.Attribute;

namespace ZRui.Web.ShopMemberAPIModels
{

    public class RegisterArgsModel
    {
        [ArgumentRequired("ShopId不能为空")]
        public int? ShopId { get; set; }
        [PhoneNum]
        public string Phone { get; set; }
        public string Sex { get; set; }
        //[ArgumentRequired("支付密码不能为空")]
        //public string Password { get; set; }
        [ArgumentRequired("生日期不能为空")]
        public DateTime? BirthDay { get; set; }

        public string Code { get; set; }
    }
    public class CheckVerificationCode
    {
        [Phone(ErrorMessage ="手机号码格式错误")]
        public string Phone { get; set; }
        public string code { get; set; }
    }
    public class GetSingleArgsModel
    {
        public int ShopId { get; set; }
    }

    public class IDArgsModel
    {
        public int Id { get; set; }
    }


    public class ConsumeArgsModel
    {
        public int OrderId { get; set; }

    }


    public class MemberTransactionModel
    {
        public decimal Amount { get; set; }
        public string TransactionTime { get; set; }
    }


    public class GetMemberDiscountModel
    {
        public double Discount { get; set; }
        public double Balance { get; set; }
    }


    public class GetMemberDiscountArgsModel
    {
        public int? ShopId { get; set; }
        public List<AddItem> Items { get; set; }
    }


    public class AddItem
    {
        public string SkuFlag { get; set; }
        public int Count { get; set; }
    }


    public class GetBillArgsModel
    {
        public int? ShopId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }


    public class GetBillModel
    {
        public string Amount { get; set; }
        public int BillType { get; set; }
        public DateTime BillDateTime { get; set; }
    }

    public class GetBillModelFormat
    {
        public decimal Amount { get; set; }
        public int BillType { get; set; }
        public string BillDateTime { get; set; }
    }

    public class GetBillReturnModel
    {
        public Common.PagedList<GetBillModel> Bill { get; set; }
        public decimal Balance { get; set; }
    }

    public class GetSingleModel : ShopMember
    {
        public string LevelName { get; set; }
        public string MemberLevel { get; set; }
    }

    public class ChangePasswordArgsModel
    {
        public int? shopId { get; set; }
        public string OldPWD { get; set; }
        public string NewPWD { get; set; }
    }


}