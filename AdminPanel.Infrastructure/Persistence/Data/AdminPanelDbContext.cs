using System;
using System.Collections.Generic;
using AdminPanel.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Infrastructure.Persistence.Data;

public partial class AdminPanelDbContext : DbContext
{
    public AdminPanelDbContext()
    {
    }

    public AdminPanelDbContext(DbContextOptions<AdminPanelDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminUser> AdminUsers { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<CampaignStatus> CampaignStatuses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Charity> Charities { get; set; }

    public virtual DbSet<CharityCategory> CharityCategories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Social> Socials { get; set; }

    public virtual DbSet<SocialCharity> SocialCharities { get; set; }

    public virtual DbSet<StoredFile> StoredFiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AdminUser");

            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.Mobile).HasMaxLength(12);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.Property(e => e.ChargedAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ModifiedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Banner).WithMany(p => p.Campaigns).HasForeignKey(d => d.BannerId);

            entity.HasOne(d => d.Charity).WithMany(p => p.Campaigns).HasForeignKey(d => d.CharityId);

            entity.HasOne(d => d.City).WithMany(p => p.Campaigns)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(d => d.Categories).WithMany(p => p.Campaigns)
                .UsingEntity<Dictionary<string, object>>(
                    "CampaignCategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Campaign>().WithMany()
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("CampaignId", "CategoryId");
                        j.ToTable("CampaignCategories");
                    });
        });

        modelBuilder.Entity<CampaignStatus>(entity =>
        {
            entity.HasKey(e => e.Status);

            entity.Property(e => e.Status).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<Charity>(entity =>
        {
            entity.Property(e => e.ContactName).HasMaxLength(50);
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.ManagerName).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Telephone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Website).HasMaxLength(255);

            entity.HasOne(d => d.Banner).WithMany(p => p.CharityBanners).HasForeignKey(d => d.BannerId);

            entity.HasOne(d => d.Logo).WithMany(p => p.CharityLogos).HasForeignKey(d => d.LogoId);
        });

        modelBuilder.Entity<CharityCategory>(entity =>
        {
            entity.HasKey(e => new { e.CharityId, e.CategoryId });

            entity.HasOne(d => d.Category).WithMany().HasForeignKey(d => d.CategoryId);
            entity.HasOne(d => d.Charity).WithMany().HasForeignKey(d => d.CharityId);
        });


        modelBuilder.Entity<City>(entity =>
        {
            entity.Property(e => e.Abbreviation)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CityId)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Social>(entity =>
        {
            entity.Property(e => e.Abbreviation).HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<SocialCharity>(entity =>
        {
            entity.ToTable("SocialCharity");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CharityId).HasColumnName("charityId");
            entity.Property(e => e.SocialId).HasColumnName("socialId");
            entity.Property(e => e.Value)
                .HasMaxLength(50)
                .HasColumnName("value");

            entity.HasOne(d => d.Charity).WithMany(p => p.SocialCharities)
                .HasForeignKey(d => d.CharityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocialCharity_Charities");

            entity.HasOne(d => d.Social).WithMany(p => p.SocialCharities)
                .HasForeignKey(d => d.SocialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SocialCharity_Socials");
        });

        modelBuilder.Entity<StoredFile>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("smalldatetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("smalldatetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
