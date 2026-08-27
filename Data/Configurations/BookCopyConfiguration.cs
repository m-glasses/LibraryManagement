using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations
{
    public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
    {

        public void Configure(EntityTypeBuilder<BookCopy> builder)
        {
            builder
                .HasOne(cp => cp.Book)
                .WithMany(b => b.BookCopies)
                .HasForeignKey(cp => cp.BookId)
                .IsRequired();

            builder
                .HasIndex(bc => bc.InventoryNumber)
                .IsUnique();
        }
    }
}
