using Microsoft.EntityFrameworkCore;

namespace HpTcgCardBrowser.Data
{
    public partial class HpTcgContext : DbContext
    {
        public HpTcgContext()
        {
        }

        public HpTcgContext(DbContextOptions<HpTcgContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Card> Card { get; set; }
        public virtual DbSet<CardDetail> CardDetail { get; set; }
        public virtual DbSet<CardRarity> CardRarity { get; set; }
        public virtual DbSet<CardSet> CardSet { get; set; }
        public virtual DbSet<CardType> CardType { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<LessonType> LessonType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>(entity =>
            {
                entity.Property(e => e.CardId).ValueGeneratedNever();

                entity.Property(e => e.CardNumber).HasMaxLength(7);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CssSizeClass)
                    .IsRequired()
                    .HasMaxLength(50);

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

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardRarity>(entity =>
            {
                entity.Property(e => e.CardRarityId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardSet>(entity =>
            {
                entity.Property(e => e.CardSetId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(150);

                entity.Property(e => e.IconFileName).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CardType>(entity =>
            {
                entity.Property(e => e.CardTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(150);

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

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
