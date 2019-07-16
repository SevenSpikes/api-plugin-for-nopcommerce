using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebHooks.Storage;
using Nop.Core;

namespace Nop.Plugin.Api.Domain
{
    public class WebHooks : BaseEntity, IRegistration
    {
        public string User { get; set; }

        public string Id { get; set; }

        public string ProtectedData { get; set; }

        public Byte[] RowVer { get; set; }
    }
}
