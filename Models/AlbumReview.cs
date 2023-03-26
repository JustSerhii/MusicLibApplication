using System;
using System.Collections.Generic;

namespace WebAppLab;

public partial class AlbumReview
{
    public int Id { get; set; }

    public int AlbumId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime WritingDate { get; set; }

    public string ReviewContent { get; set; } = null!;

    public int AlbumScore { get; set; }

    public virtual Album Album { get; set; } = null!;
}
