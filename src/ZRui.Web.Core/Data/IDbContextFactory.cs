using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    public interface IDbContextFactory
    {
        DbContext Create(ICommunityService communityService, string communityFlag, string appFlag);
    }
}
