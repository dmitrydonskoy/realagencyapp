using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RealAgencyModels
{
    public partial class RealAgencyDBContext : DbContext
    {
        public RealAgencyDBContext()
        {
        }

        public RealAgencyDBContext(DbContextOptions<RealAgencyDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Announcement> Announcements { get; set; } = null!;
        public virtual DbSet<AreaInfo> AreaInfos { get; set; } = null!;
        public virtual DbSet<Bid> Bids { get; set; } = null!;
        public virtual DbSet<Profile> Profiles { get; set; } = null!;
        public virtual DbSet<RealEstatePhoto> RealEstatePhotos { get; set; } = null!;
        public virtual DbSet<Realestate> Realestates { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Сooperation> Сooperations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=RealAgencyDB;Username=postgres;password=urogil01;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.ToTable("announcement");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasColumnName("type");

                entity.Property(e => e.Userid).HasColumnName("userid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Announcements)
                    .HasForeignKey(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("announcement_userid_fkey");

                entity.HasMany(d => d.Users)
                    .WithMany(p => p.AnnouncementsNavigation)
                    .UsingEntity<Dictionary<string, object>>(
                        "Favorite",
                        l => l.HasOne<User>().WithMany().HasForeignKey("Userid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fkannounceme524006"),
                        r => r.HasOne<Announcement>().WithMany().HasForeignKey("Announcementid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fkannounceme271332"),
                        j =>
                        {
                            j.HasKey("Announcementid", "Userid").HasName("announcement_user_pkey");

                            j.ToTable("Favorites");

                            j.IndexerProperty<int>("Announcementid").HasColumnName("announcementid");

                            j.IndexerProperty<int>("Userid").HasColumnName("userid");
                        });
            });

            modelBuilder.Entity<AreaInfo>(entity =>
            {
                entity.ToTable("area_info");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Electricity)
                    .HasMaxLength(255)
                    .HasColumnName("electricity");

                entity.Property(e => e.Gas)
                    .HasMaxLength(255)
                    .HasColumnName("gas");

                entity.Property(e => e.Heating)
                    .HasMaxLength(255)
                    .HasColumnName("heating");

                entity.Property(e => e.Realestateid).HasColumnName("realestateid");

                entity.Property(e => e.Sewerage)
                    .HasMaxLength(255)
                    .HasColumnName("sewerage");

                entity.Property(e => e.Square)
                    .HasMaxLength(255)
                    .HasColumnName("square");

                entity.Property(e => e.WaterSupply)
                    .HasMaxLength(255)
                    .HasColumnName("water_supply");

                entity.HasOne(d => d.Realestate)
                    .WithMany(p => p.AreaInfos)
                    .HasForeignKey(d => d.Realestateid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkarea_info301688");
            });

            modelBuilder.Entity<Bid>(entity =>
            {
                entity.HasKey(e => new { e.Partnerid, e.Userid })
                    .HasName("bid_pkey");

                entity.ToTable("bid");

                entity.Property(e => e.Partnerid).HasColumnName("partnerid");

                entity.Property(e => e.Userid).HasColumnName("userid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bids)
                    .HasForeignKey(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkbid448364");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("profile");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Experience)
                    .HasMaxLength(255)
                    .HasColumnName("experience");

                entity.Property(e => e.Percent)
                    .HasMaxLength(255)
                    .HasColumnName("percent");

                entity.Property(e => e.Transactions)
                    .HasMaxLength(255)
                    .HasColumnName("transactions");

                entity.Property(e => e.Userid).HasColumnName("userid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Profiles)
                    .HasForeignKey(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("profile_userid_fkey");
            });

            modelBuilder.Entity<RealEstatePhoto>(entity =>
            {
                entity.ToTable("RealEstatePhoto");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('entity_id_seq'::regclass)");

                entity.Property(e => e.Filepath)
                    .HasMaxLength(255)
                    .HasColumnName("filepath");

                entity.Property(e => e.Realestateid).HasColumnName("realestateid");

                entity.HasOne(d => d.Realestate)
                    .WithMany(p => p.RealEstatePhotos)
                    .HasForeignKey(d => d.Realestateid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkentity797069");
            });

            modelBuilder.Entity<Realestate>(entity =>
            {
                entity.ToTable("realestate");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.Announcementid).HasColumnName("announcementid");

                entity.Property(e => e.Bathroom)
                    .HasMaxLength(255)
                    .HasColumnName("bathroom");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.Floor)
                    .HasMaxLength(255)
                    .HasColumnName("floor");

                entity.Property(e => e.Furniture)
                    .HasMaxLength(255)
                    .HasColumnName("furniture");

                entity.Property(e => e.Price)
                    .HasPrecision(19, 2)
                    .HasColumnName("price");

                entity.Property(e => e.Repair)
                    .HasMaxLength(255)
                    .HasColumnName("repair");

                entity.Property(e => e.Rooms)
                    .HasMaxLength(255)
                    .HasColumnName("rooms");

                entity.Property(e => e.Square)
                    .HasMaxLength(255)
                    .HasColumnName("square");

                entity.Property(e => e.TransactionType)
                    .HasColumnType("character varying")
                    .HasColumnName("transaction_type");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasColumnName("type");

                entity.HasOne(d => d.Announcement)
                    .WithMany(p => p.Realestates)
                    .HasForeignKey(d => d.Announcementid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkrealestate655153");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Role)
                    .HasMaxLength(255)
                    .HasColumnName("role");
            });

            modelBuilder.Entity<Сooperation>(entity =>
            {
                entity.ToTable("Сooperation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Bidpartnerid).HasColumnName("bidpartnerid");

                entity.Property(e => e.Biduserid).HasColumnName("biduserid");

                entity.HasOne(d => d.Bid)
                    .WithMany(p => p.Сooperations)
                    .HasForeignKey(d => new { d.Bidpartnerid, d.Biduserid })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkСooperatio195664");

                entity.HasMany(d => d.Announcements)
                    .WithMany(p => p.Сooperations)
                    .UsingEntity<Dictionary<string, object>>(
                        "Offer",
                        l => l.HasOne<Announcement>().WithMany().HasForeignKey("Announcementid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fkoffer240709"),
                        r => r.HasOne<Сooperation>().WithMany().HasForeignKey("Сooperationid").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fkoffer253278"),
                        j =>
                        {
                            j.HasKey("Сooperationid", "Announcementid").HasName("offer_pkey");

                            j.ToTable("offer");

                            j.IndexerProperty<int>("Announcementid").HasColumnName("announcementid");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
