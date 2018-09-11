using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace ZRui.Web
{
    /// <summary>
    /// 接口返回结果
    /// </summary>
    public class APIResult
    {
        /// <summary>
        /// 是否成功，如果成功则返回真，否则返回假
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }
        /// <summary>
        /// 返回的信息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <param name="message">成功的信息</param>
        /// <returns>返回成功结果</returns>
        public static APIResult CreateSuccess(string message)
        {
            return new APIResult()
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <returns>返回成功结果</returns>
        public static APIResult CreateSuccess()
        {
            return CreateSuccess("");
        }

        /// <summary>
        /// 创建错误结果
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误信息</param>
        /// <returns>返回错误结果</returns>
        public static APIResult CreateError(int errorCode, string message)
        {
            return new APIResult()
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// 创建指定内容的成功结果
        /// </summary>
        /// <typeparam name="T">内容类型</typeparam>
        /// <param name="content">指定的内容</param>
        /// <param name="message">成功信息</param>
        /// <returns>返回指定内容的成功结果</returns>
        public static APIResult<T> CreateSuccess<T>(T content, string message)
        {
            return new APIResult<T>()
            {
                Success = true,
                Message = message,
                Content = content
            };
        }

        /// <summary>
        /// 创建指定内容的成功结果
        /// </summary>
        /// <typeparam name="T">内容类型</typeparam>
        /// <param name="content">指定的内容</param>
        /// <returns>返回指定内容的成功结果</returns>
        public static APIResult<T> CreateSuccess<T>(T content)
        {
            return CreateSuccess<T>(content, "");
        }

        /// <summary>
        /// 创建指定类型的错误结果，只是用来匹配成功信息
        /// </summary>
        /// <typeparam name="T">内容类型</typeparam>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">错误信息</param>
        /// <returns>返回指定类型的错误结果</returns>
        public static APIResult<T> CreateError<T>(int errorCode, string message)
        {
            return new APIResult<T>()
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
        }
    }

    /// <summary>
    /// 指定内容的接口返回结果
    /// </summary>
    /// <typeparam name="T">内容类型</typeparam>
    public class APIResult<T> : APIResult
    {
        /// <summary>
        /// 内容
        /// </summary>
        [JsonProperty("content")]
        public T Content { get; set; }
    }
}
