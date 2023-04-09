using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppLab;

public partial class AlbumSong
{
    public int Id { get; set; }
    [Required(ErrorMessage = "This field can't be empty")]

    [Display(Name = "Song")]
    public int SongId { get; set; }
    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Song")]

    public int AlbumId { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual Song Song { get; set; } = null!;
}
