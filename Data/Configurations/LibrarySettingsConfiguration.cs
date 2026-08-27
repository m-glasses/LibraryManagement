using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Data.Configurations
{
    public class LibrarySettingsConfiguration : IEntityTypeConfiguration<LibrarySettings>
    {
        public void Configure(EntityTypeBuilder<LibrarySettings> builder)
        {
            builder
                .Property(x => x.DailyRate)
                .HasPrecision(18, 0);

            builder
                .Property(x => x.LateFeePerDay)
                .HasPrecision(18, 0);
        }
    }
}
