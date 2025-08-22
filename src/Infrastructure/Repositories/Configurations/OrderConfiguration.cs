using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repositories.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(o => o.OrderDate)
            .IsRequired();
        
        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        // Configure Address value object
        builder.OwnsOne(o => o.ShippingAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("ShippingStreet")
                .HasMaxLength(200);
            
            addressBuilder.Property(a => a.City)
                .HasColumnName("ShippingCity")
                .HasMaxLength(100);
            
            addressBuilder.Property(a => a.Country)
                .HasColumnName("ShippingCountry")
                .HasMaxLength(100);
            
            addressBuilder.Property(a => a.PostCode)
                .HasColumnName("ShippingPostCode")
                .HasMaxLength(20);
        });
        
        builder.Property(o => o.CreatorUserId)
            .HasMaxLength(450)
            .IsRequired();
        
        builder.Property(o => o.CreationTime)
            .IsRequired();
        
        builder.Property(o => o.LastModifierUserId)
            .HasMaxLength(450);
        
        // Configure one-to-many relationship with OrderItems
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
