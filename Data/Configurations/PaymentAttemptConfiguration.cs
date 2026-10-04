using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations
{
    public class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
    {
        public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasPrecision(18, 0)
                .IsRequired();

            builder.Property(x => x.Authority)
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.PaymentAttempts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}


