using Nop.Plugin.Api.Domain;

namespace Nop.Plugin.Api.DataMappings
{
    using System.Data.Entity.ModelConfiguration;

    public class ClientMap : EntityTypeConfiguration<Client>
    {
        public ClientMap()
        {
            ToTable("API_Clients");
            
            HasKey(pt => pt.Id);
            
            Property(pt => pt.ClientId).IsRequired();
            Property(pt => pt.ClientSecret).IsRequired();
            Property(pt => pt.CallbackUrl).IsRequired();
        }
    }
}