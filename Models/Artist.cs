using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppLab;

public partial class Artist
{
    public int Id { get; set; }
    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Name")]

    public string Name { get; set; } = null!;
    [Required(ErrorMessage = "This field can't be empty")]
    [Display(Name = "Artist is active since")]

    public DateTime ActiveSince { get; set; }

    public DateTime? ActivityStop { get; set; }

    public virtual ICollection<Song> Songs { get; } = new List<Song>();
}
