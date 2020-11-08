using Microsoft.EntityFrameworkCore;

namespace Accio.Data
{
    public partial class AccioContext : DbContext
    {
        public AccioContext()
        {
        }

        public AccioContext(DbContextOptions<AccioContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AccountVerificationNumber> AccountVerificationNumber { get; set; }
        public virtual DbSet<AuthenticationHistory> AuthenticationHistory { get; set; }
        public virtual DbSet<Card> Card { get; set; }
        public virtual DbSet<CardDetail> CardDetail { get; set; }
        public virtual DbSet<CardImage> CardImage { get; set; }
        public virtual DbSet<CardProvidesLesson> CardProvidesLesson { get; set; }
        public virtual DbSet<CardRuling> CardRuling { get; set; }
        public virtual DbSet<CardSearchHistory> CardSearchHistory { get; set; }
        public virtual DbSet<CardSubType> CardSubType { get; set; }
        public virtual DbSet<CardType> CardType { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<ImageSize> ImageSize { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<LessonType> LessonType { get; set; }
        public virtual DbSet<Rarity> Rarity { get; set; }
        public virtual DbSet<Ruling> Ruling { get; set; }
        public virtual DbSet<RulingSource> RulingSource { get; set; }
        public virtual DbSet<RulingType> RulingType { get; set; }
        public virtual DbSet<Set> Set { get; set; }
        public virtual DbSet<SetLanguage> SetLanguage { get; set; }
        public virtual DbSet<Source> Source { get; set; }
        public virtual DbSet<SubType> SubType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.EmailAddress })
                    .HasName("PK_Account_1");

                entity.Property(e => e.EmailAddress).HasMaxLength(100);

                entity.Property(e => e.AccountName)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AccountVerificationNumber>(entity =>
            {
                entity.Property(e => e.AccountVerificationNumberId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Expires).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AuthenticationHistory>(entity =>
            {
                entity.Property(e => e.AuthenticationHistoryId).ValueGeneratedNever();

                entity.Property(e => e.BogusData).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmailAddress).HasMaxLength(100);

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Username).HasMaxLength(100);
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.Property(e => e.CardId).ValueGeneratedNever();

                entity.Property(e => e.CardNumber).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Orientation).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardDetail>(entity =>
            {
                entity.HasKey(e => new { e.CardDetailId, e.CardId, e.LanguageId })
                    .HasName("PK_CardDetails");

                entity.Property(e => e.Copyright).HasMaxLength(300);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Illustrator).HasMaxLength(300);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Orientation).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardImage>(entity =>
            {
                entity.Property(e => e.CardImageId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardProvidesLesson>(entity =>
            {
                entity.Property(e => e.CardProvidesLessonId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardRuling>(entity =>
            {
                entity.Property(e => e.CardRulingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardSearchHistory>(entity =>
            {
                entity.Property(e => e.CardSearchHistoryId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.SortBy).HasMaxLength(200);

                entity.Property(e => e.SortOrder).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardSubType>(entity =>
            {
                entity.Property(e => e.CardSubTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardType>(entity =>
            {
                entity.Property(e => e.CardTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.Property(e => e.ImageId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Url).IsRequired();
            });

            modelBuilder.Entity<ImageSize>(entity =>
            {
                entity.Property(e => e.ImageSizeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.Property(e => e.LanguageId).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<LessonType>(entity =>
            {
                entity.Property(e => e.LessonTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CssClassName).HasMaxLength(100);

                entity.Property(e => e.ImageName).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Rarity>(entity =>
            {
                entity.Property(e => e.RarityId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ImageName).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.Symbol).HasMaxLength(2);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Ruling>(entity =>
            {
                entity.Property(e => e.RulingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Ruling1).HasColumnName("Ruling");

                entity.Property(e => e.RulingDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RulingSource>(entity =>
            {
                entity.Property(e => e.RulingSourceId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RulingType>(entity =>
            {
                entity.Property(e => e.RulingTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.Property(e => e.SetId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(150);

                entity.Property(e => e.IconFileName).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.ReleaseDate).HasMaxLength(50);

                entity.Property(e => e.ShortName).HasMaxLength(10);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<SetLanguage>(entity =>
            {
                entity.Property(e => e.SetLanguageId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.Property(e => e.SourceId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<SubType>(entity =>
            {
                entity.Property(e => e.SubTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
