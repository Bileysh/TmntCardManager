using Microsoft.EntityFrameworkCore;

namespace TmntCardManager.Models.Data;

public partial class TmntCardsDbContext : DbContext
{
    public TmntCardsDbContext()
    {
    }

    public TmntCardsDbContext(DbContextOptions<TmntCardsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Card> Cards { get; set; }

    public virtual DbSet<Cardclass> Cardclasses { get; set; }

    public virtual DbSet<Deck> Decks { get; set; }

    public virtual DbSet<Deckcard> Deckcards { get; set; }

    public virtual DbSet<Playerprofile> Playerprofiles { get; set; }

    public virtual DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cards_pkey");

            entity.ToTable("cards");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agility).HasColumnName("agility");
            entity.Property(e => e.Classid).HasColumnName("classid");
            entity.Property(e => e.Imageurl).HasColumnName("imageurl");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Skill).HasColumnName("skill");
            entity.Property(e => e.Strength).HasColumnName("strength");
            entity.Property(e => e.Wit).HasColumnName("wit");

            entity.HasOne(d => d.Class).WithMany(p => p.Cards)
                .HasForeignKey(d => d.Classid)
                .HasConstraintName("fk_cards_class");
        });

        modelBuilder.Entity<Cardclass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cardclasses_pkey");

            entity.ToTable("cardclasses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Deck>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("decks_pkey");

            entity.ToTable("decks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Decks)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("fk_decks_users");
        });

        modelBuilder.Entity<Deckcard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("deckcards_pkey");

            entity.ToTable("deckcards");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cardid).HasColumnName("cardid");
            entity.Property(e => e.Deckid).HasColumnName("deckid");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Card).WithMany(p => p.Deckcards)
                .HasForeignKey(d => d.Cardid)
                .HasConstraintName("fk_deckcards_card");

            entity.HasOne(d => d.Deck).WithMany(p => p.Deckcards)
                .HasForeignKey(d => d.Deckid)
                .HasConstraintName("fk_deckcards_deck");
        });

        modelBuilder.Entity<Playerprofile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("playerprofiles_pkey");

            entity.ToTable("playerprofiles");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Avatarurl).HasColumnName("avatarurl");
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .HasColumnName("nickname");
            entity.Property(e => e.Winrate)
                .HasDefaultValueSql("0.0")
                .HasColumnName("winrate");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Playerprofile)
                .HasForeignKey<Playerprofile>(d => d.Id)
                .HasConstraintName("fk_playerprofile_users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValueSql("'User'::character varying")
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
