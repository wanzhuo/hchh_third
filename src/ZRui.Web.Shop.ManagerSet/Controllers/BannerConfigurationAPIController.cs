using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZRui.Web;
using ZRui.Web.Models;

namespace ZRui.Web.Controllers
{
    /// <summary>
    /// 广告图设置
    /// </summary>
    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/[controller]/Manager/[action]")]
    public class BannerConfigurationAPIController : ApiControllerBase
    {
        ShopDbContext _db;
        private IMapper _mapper { get; set; }
        public BannerConfigurationAPIController(ICommunityService communityService
            , ShopDbContext db, IMapper mapper)
        {
            this._db = db;
            this._mapper = mapper;
        }
        /// <summary>
        /// 设置banner
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetingBanner([FromBody] BannerSetingAPIModels model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    return Error("请填写广告图名称");
                }
                var banner = _mapper.Map<BannerConfiguration>(model);
                _db.Add(banner);
                _db.SaveChanges();
                return Success();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// 更新banner
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult SetingBannerUpdate([FromBody] UpdateArgsModel model)
        {
            try
            {
                var viewModel = _db.BannerConfiguration.Find(model.Id);
                //_db.Query<BannerConfiguration>().Where(m => m.Id == model.Id.Value).FirstOrDefault();
                if (viewModel == null)
                {
                    return Error("未找到BannerConfiguration");
                }
                if (string.IsNullOrEmpty(model.Name))
                {
                    return Error("请填写广告图名称");
                }
                viewModel.Name = model.Name;
                viewModel.Url = model.Url;
                viewModel.Link = model.Link;
                viewModel.IsShow = model.IsShow;
                viewModel.IsDel = model.IsDel;
                viewModel.Sorting = model.Sorting;
                //此处映射出错
                // var banner = _mapper.Map<BannerSetingAPIModels,BannerConfiguration>(model);
                //_db.Update(banner);
                _db.SaveChanges();
                return Success();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
        /// <summary>
        /// banner列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public APIResult BannerList()
        {
            try
            {
                var query = _db.Query<BannerConfiguration>().OrderByDescending(r => r.Sorting).ToList();
                return Success(query);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id查询banner
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult GetSingle([FromBody]IdArgsModel args)
        {
            var viewModel = _db.Query<BannerConfiguration>().Where(m => m.Id == args.Id).FirstOrDefault();
            if (viewModel == null) throw new Exception("记录不存在");
            var obj = _mapper.Map<BannerSetingAPIModels>(viewModel);
            return Success(obj);
        }

        /// <summary>
        /// 根据id删除banner(软删除)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public APIResult DeleteId([FromBody]IdArgsModel args)
        {
            var viewModel = _db.Query<BannerConfiguration>().Where(m => m.Id == args.Id).FirstOrDefault();
            if (viewModel == null) throw new Exception("记录不存在");
            viewModel.IsDel = true;
            _db.SaveChanges();
            return Success();
        }
    }
}
