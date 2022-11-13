using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WinterProjectAPIV3.Models;

public partial class PaymentApidbContext : DbContext
{
    public PaymentApidbContext()
    {
    }

    public PaymentApidbContext(DbContextOptions<PaymentApidbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<InPayment> InPayments { get; set; }

    public virtual DbSet<ShareGroup> ShareGroups { get; set; }

    public virtual DbSet<ShareUser> ShareUsers { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\ALLANSINSTANCE;database=PaymentAPIDB;trusted_connection=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__Expense__1445CFF3C7E22804");

            entity.ToTable("Expense");

            entity.Property(e => e.ExpenseId).HasColumnName("ExpenseID");
            entity.Property(e => e.UserGroupId).HasColumnName("UserGroupID");

            entity.HasOne(d => d.UserGroup).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.UserGroupId)
                .HasConstraintName("FK__Expense__UserGro__2C3393D0");
        });

        modelBuilder.Entity<InPayment>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__InPaymen__55433A4B9D5A69A2");

            entity.ToTable("InPayment");

            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
            entity.Property(e => e.UserGroupId).HasColumnName("UserGroupID");

            entity.HasOne(d => d.UserGroup).WithMany(p => p.InPayments)
                .HasForeignKey(d => d.UserGroupId)
                .HasConstraintName("FK__InPayment__UserG__2F10007B");
        });

        modelBuilder.Entity<ShareGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__ShareGro__149AF30AA0E21DF1");

            entity.ToTable("ShareGroup");

            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ShareUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__ShareUse__1788CCAC76A74B6B");

            entity.ToTable("ShareUser");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => e.UserGroupId).HasName("PK__UserGrou__FA5A61E08D23FB5E");

            entity.ToTable("UserGroup");

            entity.Property(e => e.UserGroupId).HasColumnName("UserGroupID");
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Group).WithMany(p => p.UserGroups)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__UserGroup__Group__29572725");

            entity.HasOne(d => d.User).WithMany(p => p.UserGroups)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserGroup__UserI__286302EC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
