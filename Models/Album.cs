using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppLab;

public partial class Album
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Title")]

    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Release Date")]
    public DateTime ReleaseDate { get; set; }
    public virtual ICollection<AlbumReview> AlbumReviews { get; } = new List<AlbumReview>();

    public virtual ICollection<AlbumSong> AlbumSongs { get; } = new List<AlbumSong>();
}
