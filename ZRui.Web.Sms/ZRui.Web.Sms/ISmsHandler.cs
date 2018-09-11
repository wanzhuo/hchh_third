using System;
using System.Threading.Tasks;

namespace ZRui.Web.Sms
{
    public interface ISmsHandler
    {
        Task<bool> SendAsync(string phone, string content);
    }
}
