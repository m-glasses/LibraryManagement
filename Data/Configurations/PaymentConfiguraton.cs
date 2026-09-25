using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .HasOne(p => p.WalletTransaction)
            .WithOne(wt => wt.Payment)
            .HasForeignKey<Payment>(p => p.WalletTransactionId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder
            .HasOne(p => p.Loan)
            .WithOne(l => l.Payment)
            .HasForeignKey<Payment>(p => p.LoanId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder
            .Property(p => p.Amount)
            .HasPrecision(18, 0);
    }
}