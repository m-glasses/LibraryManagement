using LibraryManagement.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(l => l.BookCopy)
                .WithMany(bc =>  bc.Loans)
                .HasForeignKey(l => l.BookCopyId)
                 .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .Property(l => l.DailyRate)
                .HasPrecision(18 , 0);

            builder
                .Property(l => l.LateFeePerDay)
                .HasPrecision(18, 0);

            builder
                .Property(l => l.CalculatedAmount)
                .HasPrecision(18, 0);

            builder
                .Property(l => l.FinalAmount)
                .HasPrecision(18, 0);

        }
    }
}
