using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Website.Model.Entities
{
    public partial class StockContext : DbContext
    {
        public StockContext()
        {
        }

        public StockContext(DbContextOptions<StockContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Alert> Alerts { get; set; } = null!;
        public virtual DbSet<AssetManagement> AssetManagements { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<DataAccessLog> DataAccessLogs { get; set; } = null!;
        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<HighlightedNews> HighlightedNews { get; set; } = null!;
        public virtual DbSet<Industry> Industries { get; set; } = null!;
        public virtual DbSet<MarketPrice> MarketPrices { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<PriceHistory> PriceHistories { get; set; } = null!;
        public virtual DbSet<Report> Reports { get; set; } = null!;
        public virtual DbSet<Stock> Stocks { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-JE52629R;Database=Stock;integrated security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alert>(entity =>
            {
                entity.Property(e => e.AlertId).HasColumnName("AlertID");

                entity.Property(e => e.AlertName).HasMaxLength(255);

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.PercentChange).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.PriceThresholdDown).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PriceThresholdUp).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StockId).HasColumnName("StockID");

                entity.Property(e => e.IsTriggered)
                      .HasColumnType("bit")
                      .IsRequired(false); // nullable

                entity.Property(e => e.TriggeredTime)
                      .HasColumnType("datetime")
                      .IsRequired(false); // nullable

                entity.Property(e => e.TriggerMessage)
                      .HasMaxLength(500) // hoặc theo ý bạn
                      .IsUnicode(false) // hoặc true nếu dùng unicode
                      .IsRequired(false); // nullable

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Alerts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Alerts__Customer__60A75C0F");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.Alerts)
                    .HasForeignKey(d => d.StockId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Alerts__StockID__5FB337D6");
            });

            modelBuilder.Entity<AssetManagement>(entity =>
            {
                entity.HasKey(e => e.AssetId)
                    .HasName("PK__AssetMan__43492372359C8152");

                entity.ToTable("AssetManagement");

                entity.Property(e => e.AssetId).HasColumnName("AssetID");

                entity.Property(e => e.Action).HasMaxLength(50);

                entity.Property(e => e.ActionDate).HasColumnType("datetime");

                entity.Property(e => e.AssetCode).HasMaxLength(255);

                entity.Property(e => e.AssetName).HasMaxLength(255);

                entity.Property(e => e.BuyPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.ProfitLoss)
                    .HasColumnType("decimal(30, 2)")
                    .HasComputedColumnSql("(([SellPrice]-[BuyPrice])*[Quantity])", false);

                entity.Property(e => e.SellPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StockId).HasColumnName("StockID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.AssetManagements)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__AssetMana__Custo__5441852A");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.AssetManagements)
                    .HasForeignKey(d => d.StockId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__AssetMana__Stock__5535A963");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AssetManagements)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AssetManagement_User");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName).HasMaxLength(255);

                entity.Property(e => e.CategoryType).HasMaxLength(100);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CommentId).HasColumnName("CommentID");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.CustomerName).HasMaxLength(255);

                entity.Property(e => e.NewsId).HasColumnName("NewsID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Comments__Custom__45F365D3");

                entity.HasOne(d => d.News)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.NewsId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Comments__NewsID__44FF419A");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.Phone, "UQ__Customer__5C7E359E4E19AC6B")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__Customer__A9D105344F7C5427")
                    .IsUnique();

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.RegisterDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DataAccessLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK__DataAcce__5E5499A888EA895B");

                entity.ToTable("DataAccessLog");

                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.AccessDate).HasColumnType("datetime");

                entity.Property(e => e.DataName).HasMaxLength(255);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.EventId).HasColumnName("EventID");

                entity.Property(e => e.EventName).HasMaxLength(255);
            });

            modelBuilder.Entity<HighlightedNews>(entity =>
            {
                entity.HasKey(e => e.HighlightId)
                    .HasName("PK__Highligh__B11CEDD0483BB46E");

                entity.Property(e => e.HighlightId).HasColumnName("HighlightID");

                entity.Property(e => e.NewsId).HasColumnName("NewsID");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.HasOne(d => d.News)
                    .WithMany(p => p.HighlightedNews)
                    .HasForeignKey(d => d.NewsId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Highlight__NewsI__59FA5E80");
            });

            modelBuilder.Entity<Industry>(entity =>
            {
                entity.Property(e => e.IndustryId).HasColumnName("IndustryID");

                entity.Property(e => e.IndustryName).HasMaxLength(255);
            });

            modelBuilder.Entity<MarketPrice>(entity =>
            {
                entity.HasKey(e => e.PriceId)
                    .HasName("PK__MarketPr__4957584FEF2DD39B");

                entity.Property(e => e.PriceId).HasColumnName("PriceID");

                entity.Property(e => e.AveragePrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CeilingPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Change).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FloorPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.HighPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LowPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MatchedPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OpenPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PriceDate).HasColumnType("datetime");

                entity.Property(e => e.ReferencePrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StockId).HasColumnName("StockID");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.MarketPrices)
                    .HasForeignKey(d => d.StockId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__MarketPri__Stock__4E88ABD4");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.NewsId).HasColumnName("NewsID");

                entity.Property(e => e.Author).HasMaxLength(255);

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Image1).HasMaxLength(255);

                entity.Property(e => e.Image2).HasMaxLength(255);

                entity.Property(e => e.Image3).HasMaxLength(255);

                entity.Property(e => e.Image4).HasMaxLength(255);

                entity.Property(e => e.Image5).HasMaxLength(255);

                entity.Property(e => e.PublishDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SubTitle).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__News__CategoryID__412EB0B6");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__News__UserID__4222D4EF");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.NotificationId).HasColumnName("NotificationID");

                entity.Property(e => e.Title).HasMaxLength(255);
                entity.Property(e => e.Rating)
                    .HasColumnName("Rating")
                    .HasDefaultValue(0);
            });

            modelBuilder.Entity<PriceHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId)
                    .HasName("PK__PriceHis__4D7B4ADDF4E8B975");

                entity.ToTable("PriceHistory");

                entity.Property(e => e.HistoryId).HasColumnName("HistoryID");

                entity.Property(e => e.ClosePrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.HighPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LowPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MatchPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.OpenPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.StockId).HasColumnName("StockID");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.PriceHistories)
                    .HasForeignKey(d => d.StockId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__PriceHist__Stock__5CD6CB2B");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.Property(e => e.ReportId).HasColumnName("ReportID");

                entity.Property(e => e.FilePath).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.Property(e => e.StockId).HasColumnName("StockID");

                entity.Property(e => e.Exchange).HasMaxLength(255);

                entity.Property(e => e.IndustryId).HasColumnName("IndustryID");

                entity.Property(e => e.StockName).HasMaxLength(255);

                entity.HasOne(d => d.Industry)
                    .WithMany(p => p.Stocks)
                    .HasForeignKey(d => d.IndustryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK__Stocks__Industry__4BAC3F29");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ__Users__A9D1053456A46792")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Avatar).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.UserRole).HasMaxLength(50);

                entity.Property(e => e.Username).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
