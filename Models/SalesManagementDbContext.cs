using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SalesManagement.Models;

public partial class SalesManagementDbContext : DbContext
{
    public SalesManagementDbContext()
    {
    }

    public SalesManagementDbContext(DbContextOptions<SalesManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }

    public virtual DbSet<PurchaseInvoiceHistory> PurchaseInvoiceHistories { get; set; }

    public virtual DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }

    public virtual DbSet<SalesInvoice> SalesInvoices { get; set; }

    public virtual DbSet<SalesInvoiceHistory> SalesInvoiceHistories { get; set; }

    public virtual DbSet<SalesItem> SalesItems { get; set; }

    public virtual DbSet<StockHistory> StockHistories { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__23CAF1D8F44E9E80");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3213E83FFBBC3128");

            entity.ToTable("Customer");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__2D10D16A4EFBBDE5");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<PurchaseInvoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Purchase__1252416C0481DF5E");

            entity.ToTable("PurchaseInvoice");

            entity.Property(e => e.InvoiceId).HasColumnName("invoiceId");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("date");
            entity.Property(e => e.SupplierId).HasColumnName("supplierId");

            entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseInvoices)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseInvoice_Supplier");
        });

        modelBuilder.Entity<PurchaseInvoiceHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__Purchase__19BDBDD3AE67F92C");

            entity.ToTable("PurchaseInvoiceHistory");

            entity.Property(e => e.HistoryId).HasColumnName("historyId");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("date");
            entity.Property(e => e.PurchaseInvoiceId).HasColumnName("purchaseInvoiceId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.PurchaseInvoice).WithMany(p => p.PurchaseInvoiceHistories)
                .HasForeignKey(d => d.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseInvoiceHistory_Invoice");
        });

        modelBuilder.Entity<PurchaseInvoiceItem>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.ProductId });

            entity.ToTable("PurchaseInvoiceItem");

            entity.Property(e => e.InvoiceId).HasColumnName("invoiceId");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Invoice).WithMany(p => p.PurchaseInvoiceItems)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_PurchaseInvoiceItem_Invoice");

            entity.HasOne(d => d.Product).WithMany(p => p.PurchaseInvoiceItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PurchaseInvoiceItem_Product");
        });

        modelBuilder.Entity<SalesInvoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__SalesInv__1252416C09EED21E");

            entity.ToTable("SalesInvoice");

            entity.Property(e => e.InvoiceId).HasColumnName("invoiceId");
            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("date");

            entity.HasOne(d => d.Customer).WithMany(p => p.SalesInvoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesInvoice_Customer");
        });

        modelBuilder.Entity<SalesInvoiceHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__SalesInv__19BDBDD3F92B14B7");

            entity.ToTable("SalesInvoiceHistory");

            entity.Property(e => e.HistoryId).HasColumnName("historyId");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("date");
            entity.Property(e => e.SalesInvoiceId).HasColumnName("salesInvoiceId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.SalesInvoice).WithMany(p => p.SalesInvoiceHistories)
                .HasForeignKey(d => d.SalesInvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesInvoiceHistory_Invoice");
        });

        modelBuilder.Entity<SalesItem>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.ProductId });

            entity.ToTable("SalesItem");

            entity.Property(e => e.InvoiceId).HasColumnName("invoiceId");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Invoice).WithMany(p => p.SalesItems)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_SalesItem_Invoice");

            entity.HasOne(d => d.Product).WithMany(p => p.SalesItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesItem_Product");
        });

        modelBuilder.Entity<StockHistory>(entity =>
        {
            entity.HasKey(e => e.StockhistoryId).HasName("PK__StockHis__FEEFFFF6623BB2DF");

            entity.ToTable("StockHistory");

            entity.Property(e => e.StockhistoryId).HasColumnName("stockhistoryId");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("date");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.PurchaseInvoiceId).HasColumnName("purchaseInvoiceId");
            entity.Property(e => e.QuantityChange).HasColumnName("quantityChange");
            entity.Property(e => e.Reason)
                .HasMaxLength(100)
                .HasColumnName("reason");
            entity.Property(e => e.SalesInvoiceId).HasColumnName("salesInvoiceId");

            entity.HasOne(d => d.Product).WithMany(p => p.StockHistories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockHistory_Product");

            entity.HasOne(d => d.PurchaseInvoice).WithMany(p => p.StockHistories)
                .HasForeignKey(d => d.PurchaseInvoiceId)
                .HasConstraintName("FK_StockHistory_PurchaseInvoice");

            entity.HasOne(d => d.SalesInvoice).WithMany(p => p.StockHistories)
                .HasForeignKey(d => d.SalesInvoiceId)
                .HasConstraintName("FK_StockHistory_SalesInvoice");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__DB8E62EDDC4ABD3F");

            entity.ToTable("Supplier");

            entity.Property(e => e.SupplierId).HasColumnName("supplierId");
            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .HasColumnName("phone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
