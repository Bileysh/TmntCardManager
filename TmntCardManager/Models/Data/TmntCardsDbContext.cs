using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TmntCardManager.Models.Data;

public class TmntCardsDbContext : IdentityDbContext<User, IdentityRole<int>, int>
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
    public virtual DbSet<UserCard> Usercards { get; set; }
    public virtual DbSet<ShopItem> ShopItems { get; set; }
    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<Match> Matches { get; set; }
    public virtual DbSet<Tournament> Tournaments { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tournaments_pkey");
            entity.ToTable("tournaments");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("startedat");
            entity.Property(e => e.EndDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("endedat");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isactive");
            entity.Property(e => e.Description)
                .HasColumnName("description");
        });
        
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("matches_pkey");
            entity.ToTable("matches");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Player1Id).HasColumnName("player1id");
            entity.Property(e => e.Player2Id).HasColumnName("player2id");
            entity.Property(e => e.WinnerId).HasColumnName("winnerid");
            entity.Property(e => e.PlayedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("playedat");      
                entity.HasOne(d => d.Player1).WithMany(p => p.MatchesAsPlayer1)
                .HasForeignKey(d => d.Player1Id)
                .HasConstraintName("fk_matches_player1");
            entity.HasOne(d => d.Player2).WithMany(p => p.MatchesAsPlayer2)   
                .HasForeignKey(d => d.Player2Id)
                .HasConstraintName("fk_matches_player2");
            entity.HasOne(d => d.Winner).WithMany(p => p.MatchesAsWinner)   
                .HasForeignKey(d => d.WinnerId)
                .HasConstraintName("fk_matches_winner");
        });
        
        modelBuilder.Entity<ShopItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shopitems_pkey");
            entity.ToTable("shopitems");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PriceCoins).HasColumnName("price");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.HasMany(d => d.Transactions).WithOne(p => p.ShopItem)
                .HasForeignKey(d => d.ShopItemId)
                .HasConstraintName("fk_shopitems_transactions");
        });
        
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_pkey");
            entity.ToTable("transactions");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.ShopItemId).HasColumnName("shopitemid");
            entity.Property(e => e.SpentCoins).HasColumnName("spentcoins");
            entity.Property(e => e.PurchasedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("purchasedat");      
                entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_transactions_user");
            entity.HasOne(d => d.ShopItem).WithMany(p => p.Transactions)   
                .HasForeignKey(d => d.ShopItemId)
                .HasConstraintName("fk_transactions_shopitems");
                
                    
        });
        modelBuilder.Entity<UserCard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usercards_pkey");

            entity.ToTable("usercards");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CardId).HasColumnName("cardid");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Card).WithMany(p => p.Usercards)
                .HasForeignKey(d => d.CardId)
                .HasConstraintName("fk_usercards_card");

            entity.HasOne(d => d.User).WithMany(p => p.Usercards)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_usercards_user");
        });
        
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
            entity.ToTable("users");
        });
    }
}
