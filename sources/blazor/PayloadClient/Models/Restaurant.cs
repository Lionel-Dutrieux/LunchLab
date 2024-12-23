namespace PayloadClient.Models;

public class Restaurant
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public MediaImage Image { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public MenuItems MenuItems { get; set; } = new();
}

public class MediaImage
{
    public string Id { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public int Filesize { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int FocalX { get; set; }
    public int FocalY { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? ThumbnailURL { get; set; }
}

public class MenuItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Restaurant { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Size { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class MenuItems
{
    public List<MenuItem> Docs { get; set; } = new();
    public bool HasNextPage { get; set; }
} 