using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Trojan.Services;

public sealed class AvatarSpriteService : IAvatarSpriteService
{
    private const int DefaultColumns = 4;
    private const int DefaultRows = 4;
    private const int DefaultFrameCount = DefaultColumns * DefaultRows;
    private const string SpriteImageRelativePath = "Assets/SpriteSheet/sprite_sheet.png";
    private const string SpriteMetadataRelativePath = "Assets/SpriteSheet/sprite_sheet.json";

    private readonly IReadOnlyList<CroppedBitmap> _frames;
    private int _currentFrameIndex;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int CurrentFrameIndex => _currentFrameIndex;

    public ImageSource CurrentFrameImage => _frames[_currentFrameIndex];

    public AvatarSpriteService()
    {
        BitmapImage spriteSheet = LoadSpriteSheet();
        SpriteSheetMetadata metadata = LoadMetadataOrDefault(spriteSheet);
        _frames = CreateFrames(spriteSheet, metadata);
        _currentFrameIndex = 0;
    }

    public void SetCurrentFrame(int index)
    {
        int normalizedIndex = Math.Clamp(index, 0, _frames.Count - 1);
        if (_currentFrameIndex == normalizedIndex)
        {
            return;
        }

        _currentFrameIndex = normalizedIndex;
        OnPropertyChanged(nameof(CurrentFrameIndex));
        OnPropertyChanged(nameof(CurrentFrameImage));
    }

    public int GetCurrentFrame()
    {
        return _currentFrameIndex;
    }

    private static BitmapImage LoadSpriteSheet()
    {
        string imagePath = Path.Combine(AppContext.BaseDirectory, SpriteImageRelativePath);
        if (!File.Exists(imagePath))
        {
            throw new IOException($"Sprite sheet not found at '{imagePath}'.");
        }

        Uri imageUri = new(imagePath, UriKind.Absolute);
        BitmapImage bitmap = new();
        bitmap.BeginInit();
        bitmap.UriSource = imageUri;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    private static SpriteSheetMetadata LoadMetadataOrDefault(BitmapSource spriteSheet)
    {
        SpriteSheetMetadata fallback = CreateFallbackMetadata(spriteSheet);
        string metadataPath = Path.Combine(AppContext.BaseDirectory, SpriteMetadataRelativePath);
        if (!File.Exists(metadataPath))
        {
            return fallback;
        }

        try
        {
            string json = File.ReadAllText(metadataPath);
            SpriteSheetJson? parsed = JsonSerializer.Deserialize<SpriteSheetJson>(json);
            if (parsed is null)
            {
                return fallback;
            }

            int columns = parsed.Columns > 0 ? parsed.Columns : fallback.Columns;
            int rows = parsed.Rows > 0 ? parsed.Rows : fallback.Rows;
            int cellWidth = parsed.CellWidth > 0 ? parsed.CellWidth : fallback.CellWidth;
            int cellHeight = parsed.CellHeight > 0 ? parsed.CellHeight : fallback.CellHeight;
            int padding = parsed.Padding >= 0 ? parsed.Padding : fallback.Padding;

            return new SpriteSheetMetadata(columns, rows, cellWidth, cellHeight, padding);
        }
        catch
        {
            return fallback;
        }
    }

    private static SpriteSheetMetadata CreateFallbackMetadata(BitmapSource spriteSheet)
    {
        int cellWidth = Math.Max(1, spriteSheet.PixelWidth / DefaultColumns);
        int cellHeight = Math.Max(1, spriteSheet.PixelHeight / DefaultRows);
        return new SpriteSheetMetadata(DefaultColumns, DefaultRows, cellWidth, cellHeight, 0);
    }

    private static IReadOnlyList<CroppedBitmap> CreateFrames(BitmapSource spriteSheet, SpriteSheetMetadata metadata)
    {
        int targetFrames = Math.Max(1, metadata.Columns * metadata.Rows);
        List<CroppedBitmap> frames = new(targetFrames);

        for (int index = 0; index < targetFrames; index++)
        {
            int row = index / metadata.Columns;
            int column = index % metadata.Columns;

            int x = metadata.Padding + (column * (metadata.CellWidth + metadata.Padding));
            int y = metadata.Padding + (row * (metadata.CellHeight + metadata.Padding));
            x = Math.Clamp(x, 0, Math.Max(0, spriteSheet.PixelWidth - 1));
            y = Math.Clamp(y, 0, Math.Max(0, spriteSheet.PixelHeight - 1));

            int width = Math.Min(metadata.CellWidth, Math.Max(1, spriteSheet.PixelWidth - x));
            int height = Math.Min(metadata.CellHeight, Math.Max(1, spriteSheet.PixelHeight - y));

            Int32Rect cropRect = new(x, y, width, height);
            CroppedBitmap frame = new(spriteSheet, cropRect);
            frame.Freeze();
            frames.Add(frame);
        }

        if (frames.Count == 0)
        {
            CroppedBitmap fullFrame = new(spriteSheet, new Int32Rect(0, 0, spriteSheet.PixelWidth, spriteSheet.PixelHeight));
            fullFrame.Freeze();
            frames.Add(fullFrame);
        }

        return frames;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed record SpriteSheetMetadata(int Columns, int Rows, int CellWidth, int CellHeight, int Padding);

    private sealed class SpriteSheetJson
    {
        [JsonPropertyName("image")]
        public string? Image { get; init; }
        [JsonPropertyName("cell_width")]
        public int CellWidth { get; init; }
        [JsonPropertyName("cell_height")]
        public int CellHeight { get; init; }
        [JsonPropertyName("columns")]
        public int Columns { get; init; }
        [JsonPropertyName("rows")]
        public int Rows { get; init; }
        [JsonPropertyName("padding")]
        public int Padding { get; init; }
    }
}
