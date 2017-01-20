using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Api.Domain;

namespace Nop.Plugin.Api.DataMappings
{
    public class WebHooksMap : EntityTypeConfiguration<Domain.WebHooks>
    {
        public WebHooksMap()
        {
            ToTable("WebHooks", "WebHooks");

            HasKey(wh => new {wh.User, wh.Id});

            Property(wh => wh.ProtectedData).IsRequired();
            Property(wh => wh.RowVer).IsRowVersion();
        }
    }
}
