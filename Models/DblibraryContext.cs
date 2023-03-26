using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAppLab;

public partial class DblibraryContext : DbContext
{
    public DblibraryContext()
    {
    }

    public DblibraryContext(DbContextOptions<DblibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<AlbumReview> AlbumReviews { get; set; }

    public virtual DbSet<AlbumSong> AlbumSongs { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<SongReview> SongReviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server= DESKTOP-R04J9T9\\SQLEXPRESS; Database=DBLibrary; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.Property(e => e.ReleaseDate).HasColumnType("date");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<AlbumReview>(entity =>
        {
            entity.Property(e => e.ReviewContent).HasColumnType("ntext");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.WritingDate).HasColumnType("date");

            entity.HasOne(d => d.Album).WithMany(p => p.AlbumReviews)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlbumReviews_Albums");
        });

        modelBuilder.Entity<AlbumSong>(entity =>
        {
            entity.HasOne(d => d.Album).WithMany(p => p.AlbumSongs)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlbumSongs_Albums");

            entity.HasOne(d => d.Song).WithMany(p => p.AlbumSongs)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AlbumSongs_Songs");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.Property(e => e.ActiveSince).HasColumnType("date");
            entity.Property(e => e.ActivityStop).HasColumnType("date");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(e => e.GenreName).HasMaxLength(50);
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.Property(e => e.ReleaseDate).HasColumnType("date");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Artist).WithMany(p => p.Songs)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Songs_Artists");

            entity.HasOne(d => d.Genre).WithMany(p => p.Songs)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Songs_Genres");
        });

        modelBuilder.Entity<SongReview>(entity =>
        {
            entity.Property(e => e.ReviewContent).HasColumnType("ntext");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.WritingDate).HasColumnType("date");

            entity.HasOne(d => d.Song).WithMany(p => p.SongReviews)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SongReviews_Songs1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
