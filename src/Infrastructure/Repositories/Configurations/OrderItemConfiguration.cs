using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repositories.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        
        builder.HasKey(oi => oi.Id);
        
        builder.Property(oi => oi.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(oi => oi.OrderId)
            .IsRequired();
        
        builder.Property(oi => oi.ProductName)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(oi => oi.Quantity)
            .IsRequired();
        
        builder.Property(oi => oi.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(oi => oi.CreatorUserId)
            .HasMaxLength(450)
            .IsRequired();
        
        builder.Property(oi => oi.CreationTime)
            .IsRequired();
        
        builder.Property(oi => oi.LastModifierUserId)
            .HasMaxLength(450);
        
        // Configure many-to-one relationship with Order
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure computed column for Total (read-only)
        builder.Ignore(oi => oi.Total);
    }
}
