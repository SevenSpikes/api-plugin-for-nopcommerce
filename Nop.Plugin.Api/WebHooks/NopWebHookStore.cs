using Microsoft.AspNetCore.WebHooks;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Api.WebHooks
{
    public class NopWebHookStore : DbWebHookStore<ApiObjectContext, Domain.WebHooks>
    {
        //private ApiObjectContext _dbContext;

        //public NopWebHookStore(ApiObjectContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}

        public NopWebHookStore()
        {            
        }

        protected override ApiObjectContext GetContext()
        {
            return EngineContext.Current.Resolve<ApiObjectContext>();
        }
    }
}
