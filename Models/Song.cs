using System;
using System.Collections.Generic;

namespace WebAppLab;

public partial class Song
{
    public int Id { get; set; }

    public int GenreId { get; set; }

    public int ArtistId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime ReleaseDate { get; set; }

    public double SongLength { get; set; }

    public virtual ICollection<AlbumSong> AlbumSongs { get; } = new List<AlbumSong>();

    public virtual Artist Artist { get; set; } = null!;

    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<SongReview> SongReviews { get; } = new List<SongReview>();
}
