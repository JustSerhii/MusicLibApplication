using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppLab;

public partial class SongReview
{
    public int Id { get; set; }

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Song")]
    public int SongId { get; set; }

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Title")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Date of Writing")]
    public DateTime WritingDate { get; set; }

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Content of Review")]
    public string ReviewContent { get; set; } = null!;

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Score of the song")]
    public int SongScore { get; set; }

    public virtual Song Song { get; set; } = null!;
}
