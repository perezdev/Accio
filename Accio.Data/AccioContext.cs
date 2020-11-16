using Microsoft.EntityFrameworkCore;

#nullable disable

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

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountRole> AccountRoles { get; set; }
        public virtual DbSet<AccountVerificationNumber> AccountVerificationNumbers { get; set; }
        public virtual DbSet<AuthenticationHistory> AuthenticationHistories { get; set; }
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<CardDetail> CardDetails { get; set; }
        public virtual DbSet<CardImage> CardImages { get; set; }
        public virtual DbSet<CardProvidesLesson> CardProvidesLessons { get; set; }
        public virtual DbSet<CardRuling> CardRulings { get; set; }
        public virtual DbSet<CardSearchHistory> CardSearchHistories { get; set; }
        public virtual DbSet<CardSubType> CardSubTypes { get; set; }
        public virtual DbSet<CardType> CardTypes { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<ImageSize> ImageSizes { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LessonType> LessonTypes { get; set; }
        public virtual DbSet<Rarity> Rarities { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Ruling> Rulings { get; set; }
        public virtual DbSet<RulingSource> RulingSources { get; set; }
        public virtual DbSet<RulingType> RulingTypes { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<SetLanguage> SetLanguages { get; set; }
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<SubType> SubTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.EmailAddress })
                    .HasName("PK_Account_1");

                entity.ToTable("Account");

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

            modelBuilder.Entity<AccountRole>(entity =>
            {
                entity.ToTable("AccountRole");

                entity.Property(e => e.AccountRoleId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AccountVerificationNumber>(entity =>
            {
                entity.ToTable("AccountVerificationNumber");

                entity.Property(e => e.AccountVerificationNumberId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Expires).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AuthenticationHistory>(entity =>
            {
                entity.ToTable("AuthenticationHistory");

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
                entity.ToTable("Card");

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

                entity.ToTable("CardDetail");

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
                entity.ToTable("CardImage");

                entity.Property(e => e.CardImageId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardProvidesLesson>(entity =>
            {
                entity.ToTable("CardProvidesLesson");

                entity.Property(e => e.CardProvidesLessonId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardRuling>(entity =>
            {
                entity.ToTable("CardRuling");

                entity.Property(e => e.CardRulingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardSearchHistory>(entity =>
            {
                entity.ToTable("CardSearchHistory");

                entity.Property(e => e.CardSearchHistoryId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.SortBy).HasMaxLength(200);

                entity.Property(e => e.SortOrder).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardSubType>(entity =>
            {
                entity.ToTable("CardSubType");

                entity.Property(e => e.CardSubTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardType>(entity =>
            {
                entity.ToTable("CardType");

                entity.Property(e => e.CardTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.ImageId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Url).IsRequired();
            });

            modelBuilder.Entity<ImageSize>(entity =>
            {
                entity.ToTable("ImageSize");

                entity.Property(e => e.ImageSizeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("Language");

                entity.Property(e => e.LanguageId).ValueGeneratedNever();

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<LessonType>(entity =>
            {
                entity.ToTable("LessonType");

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
                entity.ToTable("Rarity");

                entity.Property(e => e.RarityId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ImageName).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.Symbol).HasMaxLength(2);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Ruling>(entity =>
            {
                entity.ToTable("Ruling");

                entity.Property(e => e.RulingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Ruling1).HasColumnName("Ruling");

                entity.Property(e => e.RulingDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RulingSource>(entity =>
            {
                entity.ToTable("RulingSource");

                entity.Property(e => e.RulingSourceId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RulingType>(entity =>
            {
                entity.ToTable("RulingType");

                entity.Property(e => e.RulingTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.ToTable("Set");

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
                entity.ToTable("SetLanguage");

                entity.Property(e => e.SetLanguageId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.ToTable("Source");

                entity.Property(e => e.SourceId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<SubType>(entity =>
            {
                entity.ToTable("SubType");

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
