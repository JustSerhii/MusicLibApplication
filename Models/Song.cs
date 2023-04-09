using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppLab;

public partial class Song
{
    public int Id { get; set; }

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Genre")]
    public int GenreId { get; set; }

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Artist")]
    public int ArtistId { get; set; }

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Title")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Date of Release")]
    public DateTime ReleaseDate { get; set; }

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Length")]
    public double SongLength { get; set; }

    public virtual ICollection<AlbumSong> AlbumSongs { get; } = new List<AlbumSong>();

    public virtual Artist Artist { get; set; } = null!;

    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<SongReview> SongReviews { get; } = new List<SongReview>();
}
