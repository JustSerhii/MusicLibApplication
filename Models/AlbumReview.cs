using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppLab;

public partial class AlbumReview
{
    public int Id { get; set; }

    public int AlbumId { get; set; }
    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Title")]
    public string Title { get; set; } = null!;
    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Writing Date")]
    public DateTime WritingDate { get; set; }
    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Content of Riview")]
    public string ReviewContent { get; set; } = null!;
    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Score of the Album")]
    public int AlbumScore { get; set; }

    public virtual Album Album { get; set; } = null!;
}
