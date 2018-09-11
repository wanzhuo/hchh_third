using Microsoft.AspNetCore.Mvc;
using Aliyun.OSS;
using Microsoft.AspNetCore.Authorization;
using System;
using Aliyun.Acs.Sts.Model.V20150401;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core;

namespace ZRui.Web.Controllers
{

    [Microsoft.AspNetCore.Cors.EnableCors("AllowTestOrigin")]
    [Route("api/ShopResource/[action]")]
    public class ShopResourceController : ApiControllerBase
    {     
        [HttpGet]
        [Authorize]
        public ObjectListing GetShopImages(int shopId)
        {
            return new ShopResource().GetShopImages(shopId);
        }

        [HttpGet]
        public SSTObject GetSST()
        {
            return new ShopResource().GetSST();
        }

        [HttpPost]
        public APIResult Upload()
        {
            //if (HttpContext.Request.Files.Count > 0)
            //{
            //    HttpPostedFile file = HttpContext.Current.Request.Files[0];
            //    string fileName = HttpContext.Current.Request["fileName"];
            //    if (string.IsNullOrWhiteSpace(fileName))
            //    {
            //        fileName = file.FileName;
            //        if (fileName.Contains("."))
            //        {
            //            fileName = fileName.Substring(fileName.LastIndexOf("."));
            //        }
            //        fileName = Guid.NewGuid().ToString() + fileName;
            //    }
            //    IUploadServices upload = Container.Resolve<IUploadServices>();
            //    return upload.Upload(file.InputStream, fileName);
            //}

            //return new ApiResult() { StatusCode = APIStatusCode.Failure, Message = "没有接收到待上传的文件" };
            return null;
        }

        [HttpGet]
        public string Test()
        {
            return "ok";
        }

    }

    public class ShopResource
    {
        const string EndPoint = "http://oss-cn-shenzhen.aliyuncs.com";
        const string AccessKeyId = "LTAIOYffGZzyls27";
        const string AccessKeySecret = "0bWpMGhCVGgNGRXhJVk9LRV7a5MALt";
        const string RegionId = "cn-shenzhen";
        const string RoleArn = "acs:ram::1050113819028063:role/uploadrole";
        const string RoleSessionName = "uploadrole";
        const string BucketName = "91huichihuihe";
        const string FileDir = "http://91huichihuihe.oss-cn-shenzhen.aliyuncs.com/";


        static AssumeRoleResponse _response = null;
        static AssumeRoleResponse response
        {
            get
            {
                if (_response == null || DateTime.Parse(_response.Credentials.Expiration).ToLocalTime().AddSeconds(-30) < DateTime.Now)
                {
                    _response = GetAcsResponse();
                }
                return _response;
            }
        }

        public static AssumeRoleResponse GetAcsResponse()
        {

            IClientProfile profile = DefaultProfile.GetProfile(RegionId, AccessKeyId, AccessKeySecret);
            DefaultAcsClient client = new DefaultAcsClient(profile);

            // 构造AssumeRole请求
            AssumeRoleRequest request = new AssumeRoleRequest();
            // 指定角色Arn
            request.RoleArn = RoleArn;
            request.RoleSessionName = RoleSessionName;

            // 可以设置Token有效期，可选参数，默认3600秒；
            request.DurationSeconds = 3600;
            // 可以设置Token的附加Policy，可以在获取Token时，通过额外设置一个Policy进一步减小Token的权限；
            // request.Policy="<policy-content>"              
            return client.GetAcsResponse(request);
        }

        public SSTObject GetSST()
        {
            try
            {
                return new SSTObject
                {
                    FileDir = FileDir,
                    RegionId= "oss-" + RegionId,
                    EndPoint = EndPoint,
                    BucketName = BucketName,
                    AccessKeyId = response.Credentials.AccessKeyId,
                    AccessKeySecret = response.Credentials.AccessKeySecret,
                    SecurityToken = response.Credentials.SecurityToken,
                    Expiration = DateTime.Parse(response.Credentials.Expiration).ToLocalTime().ToString("yyyy-MM-dd hh:MM:ss")
                };
            }
            catch (Exception ex)
            {
                throw  ex;
            }
        }

        public ObjectListing GetShopImages(int shopId)
        {
            var listObjectsRequest = new ListObjectsRequest(BucketName);
            string prefix = string.Format("shop_{0}/images/", shopId);
            listObjectsRequest.Prefix = prefix;
            var sst = GetSST();
            OssClient client = new OssClient(EndPoint, sst.AccessKeyId, sst.AccessKeySecret, sst.SecurityToken);
          return  client.ListObjects(listObjectsRequest);
        }
    }

    public class SSTObject
    {
        public string FileDir { get; set; }
        public string RegionId { get; set; }
        public string EndPoint { get; set; }
        public string BucketName { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string SecurityToken { get; set; }
        public string Expiration { get; set; }
    }
}
