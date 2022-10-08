﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NET6.Microservice.Order.API.Domain.AggregateModels.OrderAggregates;

namespace NET6.Microservice.Order.API.Infrastructure
{
    /*
     * Domain-driven design (DDD)
     * https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/seedwork-domain-model-base-classes-interfaces
     */
    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<NET6.Microservice.Order.API.Models.Order>
    {
        public void Configure(EntityTypeBuilder<NET6.Microservice.Order.API.Models.Order> orderConfiguration)
        {
            orderConfiguration.ToTable("orders", OrderingContext.DEFAULT_SCHEMA);

            orderConfiguration.HasKey(o => o.Id);

            orderConfiguration.Ignore(b => b.DomainEvents);

            orderConfiguration.Property(o => o.Id)
                .UseHiLo("orderseq", OrderingContext.DEFAULT_SCHEMA);

            //Address value object persisted as owned entity type supported since EF Core 2.0
            orderConfiguration
                .OwnsOne(o => o.Address, a =>
                {
                    // Explicit configuration of the shadow key property in the owned type 
                    // as a workaround for a documented issue in EF Core 5: https://github.com/dotnet/efcore/issues/20740
                    a.Property<int>("OrderId")
                    .UseHiLo("orderseq", OrderingContext.DEFAULT_SCHEMA);
                    a.WithOwner();
                });

            orderConfiguration
                .Property<int?>("_buyerId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("BuyerId")
                .IsRequired(false);

            orderConfiguration
                .Property<DateTime>("_orderDate")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderDate")
                .IsRequired();

            orderConfiguration
                .Property<int>("_orderStatusId")
                // .HasField("_orderStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderStatusId")
                .IsRequired();

            orderConfiguration
                .Property<int?>("_paymentMethodId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentMethodId")
                .IsRequired(false);

            orderConfiguration.Property<string>("Description").IsRequired(false);

            //var navigation = orderConfiguration.Metadata.FindNavigation(nameof(Order.OrderItems));

            // DDD Patterns comment:
            //Set as field (New since EF 1.1) to access the OrderItem collection property through its field
            //navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            //orderConfiguration.HasOne<PaymentMethod>()
            //    .WithMany()
            //    // .HasForeignKey("PaymentMethodId")
            //    .HasForeignKey("_paymentMethodId")
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.Restrict);

            //orderConfiguration.HasOne<Buyer>()
            //    .WithMany()
            //    .IsRequired(false)
            //    // .HasForeignKey("BuyerId");
            //    .HasForeignKey("_buyerId");

            orderConfiguration.HasOne(o => o.OrderStatus)
                .WithMany()
                // .HasForeignKey("OrderStatusId");
                .HasForeignKey("_orderStatusId");
        }
    }
}

