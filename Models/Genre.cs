using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppLab;

public partial class Genre
{
    public int Id { get; set; }

    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Name of Genre")]
    public string GenreName { get; set; } = null!;

    public virtual ICollection<Song> Songs { get; } = new List<Song>();
}
