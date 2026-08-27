using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder
                .HasKey(x => x.Id);


            builder
                .HasOne(u => u.User)
                .WithOne(u => u.Wallet)
                .HasForeignKey<Wallet>(u => u.UserId);
        }
    }
}
