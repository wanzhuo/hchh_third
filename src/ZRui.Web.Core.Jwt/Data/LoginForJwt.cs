using System;
using System.Collections;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using ZRui.Web.Common;

namespace ZRui.Web
{
    /// <summary>
    /// Jwt登陆信息
    /// </summary>
    public class LoginForJwt : EntityBase
    {
        public LoginForJwt()
        {
            LoginSettings = "{}";
        }
        /// <summary>
        /// 标识
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 登陆类型
        /// </summary>
        public JwtLoginType LoginType { get; set; }
        /// <summary>
        /// 关联的用户Id
        /// </summary>
        public int? MemberId { get; set; }
        /// <summary>
        /// 登陆参数
        /// </summary>
        public string LoginSettings { get; set; }
        /// <summary>
        /// 解析LoginSettings为XDocument
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        XDocument GetloginSettingsToXDocument()
        {
            if (string.IsNullOrEmpty(LoginSettings)) throw new Exception("LoginSettings为空");
            if (xdoc == null)
            {
                xdoc = Newtonsoft.Json.JsonConvert.DeserializeXNode(LoginSettings);
                if (xdoc.Root == null)
                {
                    xdoc.Add(new XElement("settings"));
                }
            }
            return xdoc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        public void SetloginSettingValue(string name, object value)
        {
            var doc = GetloginSettingsToXDocument();
            var el = doc.Root.Element(name);
            if (el == null)
            {
                el = new XElement(name, value);
                doc.Root.Add(el);
            }
            else
            {
                el.SetValue(value);
            }
            LoginSettings = Newtonsoft.Json.JsonConvert.SerializeXNode(doc);
        }

        public T GetloginSettingValue<T>(string name)
        {
            var doc = GetloginSettingsToXDocument();
            var el = doc.Root.Element(name);
            if (el != null)
            {
                return (T)Convert.ChangeType(el.Value, typeof(T));
            }
            else
            {
                return default(T);
            }
        }
        public static string CreateJwtToken(LoginForJwt model)
        {
            return CreateJwtToken(model, null);
        }

        public static string CreateJwtToken(LoginForJwt model, TimeSpan? expiration)
        {
            var secretKey = "STkzn6iROYF8YqE840An";
            var signingKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secretKey));

            var now = DateTime.UtcNow;
            if (!expiration.HasValue)
                expiration = TimeSpan.FromMinutes(30 * 2 * 2);

            var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, model.Flag),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, CommonUtil.ToTimestamp(now).ToString(), ClaimValueTypes.Integer64)
                };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                    issuer: "wechat",//token 是给谁的 
                    audience: "user", //读者
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(expiration.Value),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        private XDocument xdoc;
    }

    public static class LoginForJwtDbContextExtention
    {
        public static LoginForJwt AddToLoginForJwt(this DbContext context, LoginForJwt model)
        {
            context.Set<LoginForJwt>().Add(model);
            return model;
        }

        public static LoginForJwt GetSingleLoginForJwt(this DbContext context, string flag)
        {
            return context.Set<LoginForJwt>().Where(m => m.Flag == flag).FirstOrDefault();
        }

        public static IQueryable<LoginForJwt> QueryLoginForJwt(this DbContext context)
        {
            return context.Set<LoginForJwt>().AsQueryable();
        }

        public static DbSet<LoginForJwt> LoginForJwtDbSet(this DbContext context)
        {
            return context.Set<LoginForJwt>();
        }
    }

    public enum JwtLoginType
    {
        WechatMp = 0,
        WechatOpen = 1,
        WechatWxOpen = 2
    }
}
