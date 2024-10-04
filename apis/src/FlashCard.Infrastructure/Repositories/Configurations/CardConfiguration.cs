using FlashCard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashCard.Infrastructure.Repositories.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.ToTable("cards");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Text).HasMaxLength(255).IsRequired();
        builder.Property(u => u.Description).IsRequired();
    }
}
