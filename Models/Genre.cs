using System;
using System.Collections.Generic;

namespace WebAppLab;

public partial class Genre
{
    public int Id { get; set; }

    public string GenreName { get; set; } = null!;

    public virtual ICollection<Song> Songs { get; } = new List<Song>();
}
