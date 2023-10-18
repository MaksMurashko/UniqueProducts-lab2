using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace UniqueProducts.Models;

public partial class UniqueProductsContext : DbContext
{
    public UniqueProductsContext()
    {
    }

    public UniqueProductsContext(DbContextOptions<UniqueProductsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersAndClientsView> OrdersAndClientsViews { get; set; }

    public virtual DbSet<OrdersClientsProductsView> OrdersClientsProductsViews { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ConfigurationBuilder builder = new();
        // установка пути к текущему каталогу
        builder.SetBasePath(Directory.GetCurrentDirectory());
        // получаем конфигурацию из файла appsettings.json
        builder.AddJsonFile("appsettings.json");
        // создаем конфигурацию
        IConfigurationRoot config = builder.Build();
        // получаем строку подключения
        string connectionString = config.GetConnectionString("DefaultConnection");
        _ = optionsBuilder
            .UseSqlServer(connectionString)
            .Options;
        optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__E67E1A0417296E2A");

            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.Company).HasMaxLength(30);
            entity.Property(e => e.CompanyAddress).HasMaxLength(60);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.Representative).HasMaxLength(30);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF164B90780");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EmployeeMidname).HasMaxLength(30);
            entity.Property(e => e.EmployeeName).HasMaxLength(30);
            entity.Property(e => e.EmployeePosition).HasMaxLength(30);
            entity.Property(e => e.EmployeeSurname).HasMaxLength(30);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__C50613175C809C6C");

            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.MaterialDescript).HasMaxLength(60);
            entity.Property(e => e.MaterialName).HasMaxLength(30);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF90EC9157");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.OrderDate).HasColumnType("date");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Orders_Clients");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Orders_Employees");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Orders_Products");
        });

        modelBuilder.Entity<OrdersAndClientsView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OrdersAndClientsView");

            entity.Property(e => e.Company).HasMaxLength(30);
            entity.Property(e => e.OrderDate).HasColumnType("date");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Representative).HasMaxLength(30);
        });

        modelBuilder.Entity<OrdersClientsProductsView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OrdersClientsProductsView");

            entity.Property(e => e.Company).HasMaxLength(30);
            entity.Property(e => e.OrderDate).HasColumnType("date");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.ProductName).HasMaxLength(30);
            entity.Property(e => e.Representative).HasMaxLength(30);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED22729270");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.MaterialId).HasColumnName("MaterialID");
            entity.Property(e => e.ProductColor).HasMaxLength(30);
            entity.Property(e => e.ProductDescript).HasMaxLength(60);
            entity.Property(e => e.ProductName).HasMaxLength(30);
            entity.Property(e => e.ProductPrice).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Material).WithMany(p => p.Products)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Products_Materials");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
