using System;
using System.Linq;
using ZRui.Web.ShopManager.ShopBookingSetAPIModels;
using ZRui.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using ZRui.Web.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZRui.Web.BLL.ServerDto;
using ZRui.Web.BLL.Servers;
using Microsoft.Extensions.Logging;

namespace ZRui.Web.Controllers
{

    /// <summary>
    /// 会员等级设置
    /// </summary>
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class ShopMemberLevelController : ShopManagerApiControllerBase
    {

        readonly IHostingEnvironment hostingEnvironment;
        private IMapper _mapper { get; set; }
        ILogger _logger;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="communityService"></param>
        /// <param name="options"></param>
        /// <param name="memberDb"></param>
        /// <param name="db"></param>
        /// <param name="hostingEnvironment"></param>
        public ShopMemberLevelController(ICommunityService communityService
            , IOptions<MemberAPIOptions> options
            , MemberDbContext memberDb
            , ShopDbContext db
            , IMapper mapper
            , ILoggerFactory loggerFactory
            , IHostingEnvironment hostingEnvironment)
            : base(options, db, memberDb)
        {
            this.db = db;
            this.hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<ShopMemberLevelController>();
        }


        /// <summary>
        /// 更新等级
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> UpadteMemberLevel([FromBody] UpadteMemberSetting input)
        {
            var shopMemberLevel = SetSort(input.Source);
            var update = shopMemberLevel.Where(m => m.Id != 0);
            var add = shopMemberLevel.Where(m => m.Id == 0);
            foreach (var updateitem in update)
            {
                var updateShopMemberLevel = await db.ShopMemberLevel.FindAsync(updateitem.Id);
                updateShopMemberLevel.LevelName = updateitem.LevelName;
                updateShopMemberLevel.MemberLevel = updateitem.MemberLevel;
            }
            await db.ShopMemberLevel.AddRangeAsync(add);
            await db.SaveChangesAsync();
            return Success();
        }

        /// <summary>
        /// 获取会员设置等级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> GetMemberLevels([FromBody]GetMemberLevelsModel input)
        {
            var shopMemberLevels = db.ShopMemberLevel.Where(m => !m.IsDel && m.ShopId.Equals(input.ShopId)).OrderBy(m => m.Sort).AsNoTracking();
            return await Task.FromResult(Success(shopMemberLevels));
        }

        /// <summary>
        /// 删除会员等级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> DelMemberLevel([FromBody]AddMemberLevelModel input)
        {
            var shopMemberLevels = db.ShopMemberLevel.Find(input.Id);
            if (shopMemberLevels == null || shopMemberLevels.IsDel)
            {
                return await Task.FromResult(Error("记录不存在"));
            }

            var minLevel = await db.ShopMemberLevel.Where(m => m.Shop.Equals(input.ShopId) && !m.IsDel).OrderByDescending(m => m.Sort).FirstOrDefaultAsync();
            if (minLevel.Id != input.Id)
            {
                return await Task.FromResult(Error("需要从等级最低的开始删除"));
            }
            minLevel.IsDel = true;
            await db.SaveChangesAsync();
            await ShopMemberLevelServer.UpdateAllMemberLevel(db, db.ShopMemberSet.FirstOrDefault(m => !m.IsDel && m.ShopId.Equals(input.ShopId)), input.ShopId, _logger);
            return await Task.FromResult(Success("删除成功"));
        }



        /// <summary>
        /// 更新会员设置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> UpadteMemberSetting([FromBody]UpadteMemberSettingModel input)
        {
            try
            {
                if (input.Source.shopMemberSetModel.IsConsumeIntegral)
                {
                    if (input.Source.shopMemberSetModel.GetIntegral<=0)
                    {
                        return await Task.FromResult(Error("获取积分不能为负数"));

                    }
                }
                //更新设置
                var shopMemberSet = await db.ShopMemberSet.FirstOrDefaultAsync(m => m.ShopId.Equals(input.Source.shopMemberSetModel.ShopId) &&!m.IsDel);
                if (shopMemberSet == null)
                {
                    shopMemberSet = _mapper.Map<ShopMemberSet>(input.Source.shopMemberSetModel);
                    await db.ShopMemberSet.AddAsync(shopMemberSet);
                }
                else
                {
                    shopMemberSet.ConsumeAmount = input.Source.shopMemberSetModel.ConsumeAmount;
                    shopMemberSet.GetIntegral = input.Source.shopMemberSetModel.GetIntegral;
                    shopMemberSet.IsTopUpIntegral = input.Source.shopMemberSetModel.IsTopUpIntegral;
                    shopMemberSet.IsConsumeIntegral = input.Source.shopMemberSetModel.IsConsumeIntegral;
                    shopMemberSet.IsTopUpDiscount = input.Source.shopMemberSetModel.IsTopUpDiscount;
                    shopMemberSet.IsSavaLevel = input.Source.shopMemberSetModel.IsSavaLevel;
                    shopMemberSet.IsShowCustomTopUpSet = input.Source.shopMemberSetModel.IsShowCustomTopUpSet;
                }

                await UpdateTopUpSet(input.Source);
                await db.SaveChangesAsync();
                return await Task.FromResult(Success());
            }
            catch (Exception e)
            {

                return await Task.FromResult(Error($"请输入正确的数据类型:{e}"));

            }
        }


        /// <summary>
        /// 获取会员设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> GetShopMemberSet([FromBody]GetPagedListBaseModel input)
        {
            var shopMemberSet = await db.ShopMemberSet.FirstOrDefaultAsync(m => m.ShopId.Equals(input.ShopId) && !m.IsDel);
            if (shopMemberSet == null)
            {
                return await Task.FromResult(Error("记录不存在"));
            }
            //固定充值设置
            var shopTopUpSets = db.ShopTopUpSet.Where(m => m.ShopId.Equals(input.ShopId) && !m.IsDel).OrderByDescending(m => m.AddTime).AsNoTracking();
            var shopCustomTopUpSet = await db.ShopCustomTopUpSet.FirstOrDefaultAsync(m => m.ShopId.Equals(input.ShopId) && !m.IsDel);
            var result = new { ShopMemberSetModel = _mapper.Map<ShopMemberSetModel>(shopMemberSet), TopUpSetModels = _mapper.Map<List<TopUpSetModel>>(shopTopUpSets.ToList()), ShopCustomTopUpSet = _mapper.Map<ShopCustomTopUpSetModel>(shopCustomTopUpSet) };
            result.ShopMemberSetModel.IsShowTopUpSet = (result.TopUpSetModels.Count != 0 || result.ShopCustomTopUpSet.MeetAmount != 0 || result.ShopCustomTopUpSet.StartAmount != 0 || result.ShopCustomTopUpSet.Additional != 0D);
            return await Task.FromResult(Success(result));
        }

        /// <summary>
        /// 删除固定充值记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> DelTopUpSet([FromBody]TopUpSetModel input)
        {
            var shopTopUpSet = db.ShopTopUpSet.Find(input.Id);
            if (shopTopUpSet == null || shopTopUpSet.IsDel)
            {
                return await Task.FromResult(Error("记录不存在"));
            }
            shopTopUpSet.IsDel = true;
            await db.SaveChangesAsync();

            return await Task.FromResult(Success());
        }


        ///// <summary>
        ///// 充值设置
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>

        //[HttpPost]
        //public async Task<APIResult> UpdateTopUpSet([FromBody]UpadteMemberSetting input)
        //{
        //    await UpdateTopUpSet(input);

        //    await db.SaveChangesAsync();
        //    return await Task.FromResult(Success());
        //}

        /// <summary>
        /// 添加等级
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> AddMemberLevel([FromBody] AddMemberLevelModel input)
        {

            var shopMemberLevel = db.ShopMemberLevel.Where(m => !m.IsDel && m.ShopId.Equals(input.ShopId));
            if (shopMemberLevel.Count() != 0)
            {
                var maxlevel = await shopMemberLevel.OrderByDescending(m => m.Sort).FirstOrDefaultAsync();
                if (input.MinIntegral - maxlevel.MaxIntegral != 1)
                {
                    return await Task.FromResult(Error($"最低积分必须比上一个会员等级的最高积分加一,最低积分必须为 { maxlevel.MaxIntegral + 1}"));
                }

                input.Sort = maxlevel.Sort + 1;
                var addShopMemberLevel = _mapper.Map<ShopMemberLevel>(input);
                await db.ShopMemberLevel.AddAsync(addShopMemberLevel);
            }
            else
            {
                input.Sort = 1;
                var addShopMemberLevel = _mapper.Map<ShopMemberLevel>(input);
                await db.ShopMemberLevel.AddAsync(addShopMemberLevel);
            }
            await db.SaveChangesAsync();
            await ShopMemberLevelServer.UpdateAllMemberLevel(db, db.ShopMemberSet.FirstOrDefault(m => !m.IsDel && m.ShopId.Equals(input.ShopId)), input.ShopId, _logger);
            SetSort(input.ShopId);
            return await Task.FromResult(Success());
        }
        /// <summary>
        /// 编辑等级
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<APIResult> UpdateMemberLevel([FromBody] AddMemberLevelModel input)
        {
            var shopMemberLevel = await db.ShopMemberLevel.FindAsync(input.Id);
            if (shopMemberLevel.IsDel)
            {
                return await Task.FromResult(Error("记录不存在"));
            }
            shopMemberLevel.LevelName = input.LevelName;
            shopMemberLevel.MemberLevel = input.MemberLevel;
            shopMemberLevel.Discount = input.Discount;
            await db.SaveChangesAsync();
            return await Task.FromResult(Success());
        }

        #region 内部使用
        /// <summary>
        /// 更新充值金额设置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task UpdateTopUpSet(UpadteMemberSetting input)
        {


            //固定充值
            //var updateList = input.topUpSetModels.Where(m => m.Id != 0).ToList();
            //var addList = input.topUpSetModels.Where(m => m.Id == 0).ToList();
            //foreach (var updateListItem in updateList.topUpSetModels)
            //{
            //    var shopTopUpSet = db.ShopTopUpSet.Find(updateListItem.Id);
            //    if (shopTopUpSet != null)
            //    {
            //        shopTopUpSet.FixationTopUpAmount = updateListItem.FixationTopUpAmount;
            //        shopTopUpSet.PresentedAmount = updateListItem.PresentedAmount;
            //    }


            //}
            //if (addList.Count != 0)
            //{
            //    var shopTopUpSets = _mapper.Map<List<ShopTopUpSet>>(addList);
            //    await db.ShopTopUpSet.AddRangeAsync(shopTopUpSets);
            //}

            var remoShopTopUpSet = db.ShopTopUpSet.Where(m => !m.IsDel && m.ShopId.Equals(input.shopMemberSetModel.ShopId)).ToList();
            foreach (var remoShopTopUpSetItem in remoShopTopUpSet)
            {
                remoShopTopUpSetItem.IsDel = true;
            }
            List<TopUpSetModel> addModel = new List<TopUpSetModel>();
            foreach (var topUpSetModelsItem in input.topUpSetModels)
            {
               
               
            }
            var addShopTopUpSet = _mapper.Map<List<ShopTopUpSet>>(input.topUpSetModels);
            await db.ShopTopUpSet.AddRangeAsync(addShopTopUpSet);


            //自定义充值
            var shopCustomTopUpSet = await db.ShopCustomTopUpSet.FirstOrDefaultAsync(m => m.ShopId.Equals(input.shopCustomTopUpSet.ShopId) && !m.IsDel);
            if (shopCustomTopUpSet == null)
            {
                var shopTopUpSets = _mapper.Map<ShopCustomTopUpSet>(input.shopCustomTopUpSet);
                await db.ShopCustomTopUpSet.AddAsync(shopTopUpSets);
            }
            else
            {
                shopCustomTopUpSet.Additional = input.shopCustomTopUpSet.Additional;
                shopCustomTopUpSet.MeetAmount = input.shopCustomTopUpSet.MeetAmount;
                shopCustomTopUpSet.StartAmount = input.shopCustomTopUpSet.StartAmount;
            }
        }


        /// <summary>
        ///更新排序
        /// </summary>
        /// <param name="memberLevelModel"></param>
        /// <returns></returns>
        private List<ShopMemberLevel> SetSort(List<AddMemberLevelModel> memberLevelModel)
        {
            try
            {
                bool isWhile = true;
                int level = 1;
                List<AddMemberLevelModel> newlist = new List<AddMemberLevelModel>();
                var minMemberLevel = memberLevelModel.FirstOrDefault(m => m.MinIntegral.Equals(memberLevelModel.Min(d => d.MinIntegral)));
                minMemberLevel.Sort = level;
                newlist.Add(minMemberLevel);
                memberLevelModel.Remove(minMemberLevel);
                while (isWhile)
                {

                    if (memberLevelModel.Count.Equals(0))
                    {
                        isWhile = false;
                        continue;
                    }
                    minMemberLevel = memberLevelModel.FirstOrDefault(m => m.MinIntegral.Equals(newlist.FirstOrDefault(c => c.Sort.Equals(level)).MaxIntegral + 1));
                    level++;
                    minMemberLevel.Sort = level;
                    newlist.Add(minMemberLevel);
                    memberLevelModel.Remove(minMemberLevel);
                }

                var shopMemberLevels = _mapper.Map<List<ShopMemberLevel>>(newlist);
                return shopMemberLevels;
            }
            catch
            {

                throw new Exception("积分数量有误");
            }
        }


        /// <summary>
        ///更新排序
        /// </summary>
        /// <param name="shopid">店铺Ids</param>
        /// <returns></returns>
        private void SetSort(int shopid)
        {
            try
            {
                int level = 1;
                ShopMemberLevel minMemberLevel = new ShopMemberLevel();
                var shopidMemberLevel = db.ShopMemberLevel.Where(m => !m.IsDel && m.ShopId.Equals(shopid));
                minMemberLevel = shopidMemberLevel.FirstOrDefault(m => m.MinIntegral.Equals(shopidMemberLevel.Min(d => d.MinIntegral)));
                minMemberLevel.Sort = level;
                foreach (var minMemberLevelItem in shopidMemberLevel)
                {
                    level++;
                    minMemberLevel = shopidMemberLevel.FirstOrDefault(m => m.MinIntegral.Equals(minMemberLevel.MaxIntegral + 1));
                    if (minMemberLevel != null)
                    {
                        minMemberLevel.Sort = level;
                    }
                }
                db.SaveChanges();

                //List<AddMemberLevelModel> newlist = new List<AddMemberLevelModel>();
                //var minMemberLevel = memberLevelModel.FirstOrDefault(m => m.MinIntegral.Equals(memberLevelModel.Min(d => d.MinIntegral)));
                //minMemberLevel.Sort = level;
                //newlist.Add(minMemberLevel);
                //memberLevelModel.Remove(minMemberLevel);
                //while (isWhile)
                //{

                //    if (memberLevelModel.Count.Equals(0))
                //    {
                //        isWhile = false;
                //        continue;
                //    }
                //    minMemberLevel = memberLevelModel.FirstOrDefault(m => m.MinIntegral.Equals(newlist.FirstOrDefault(c => c.Sort.Equals(level)).MaxIntegral + 1));
                //    level++;
                //    minMemberLevel.Sort = level;
                //    newlist.Add(minMemberLevel);
                //    memberLevelModel.Remove(minMemberLevel);
                //}

                //var shopMemberLevels = _mapper.Map<List<ShopMemberLevel>>(newlist);
                //return shopMemberLevels;
            }
            catch (Exception e)
            {

                throw new Exception($"更新排序错误：{e}");
            }
        }


        #endregion
    }
}
