using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using ZRui.Web.Common;
using ZRui.Web.ShopManager.ShopOrderSetAPIModels;

namespace ZRui.Web.OrderHandlers
{
    public class OrderContext
    {
        private IOrderStrategy strategy;
        private IMapper _mapper { get; set; }

        public OrderContext(OrderTypeE orderTypeE, IMapper mapper)
        {
            this._mapper = mapper;
            switch (orderTypeE)
            {
                case OrderTypeE.扫码点餐订单:
                    this.strategy = new ShopOrderStrategy(_mapper);
                    break;
                case OrderTypeE.外卖订单:
                    this.strategy = new TakeOutOrderStrategy(_mapper);
                    break;
                case OrderTypeE.自助点餐订单:
                    strategy = new SelfHelpStrategy(_mapper);
                    break;
                case OrderTypeE.拼团订单:
                    this.strategy = new ConglomerationOrderStrategy(_mapper);
                    break;
                default:
                    this.strategy = new TakeOutOrderStrategy(_mapper);
                    break;

            }

        }

        public PagedList<GetPagedListResulrModel> ExecuteGetPagedList(GetPagedListRequestModel input, ShopDbContext db)
        {
            return strategy.GetPagedList(input, db);
        }

        public T ExecuteGetOrderItems<T>(GetOrderItemsArgsModel input, ShopDbContext db)
        {
            return strategy.GetOrderItems<T>(input, db);
        }
    }
}
