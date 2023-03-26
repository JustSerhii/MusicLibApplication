using System;
using System.Collections.Generic;

namespace WebAppLab;

public partial class SongReview
{
    public int Id { get; set; }

    public int SongId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime WritingDate { get; set; }

    public string ReviewContent { get; set; } = null!;

    public int SongScore { get; set; }

    public virtual Song Song { get; set; } = null!;
}
