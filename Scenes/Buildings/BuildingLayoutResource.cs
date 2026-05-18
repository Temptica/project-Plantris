using Godot;

namespace ProjectPlantris.Scenes.Buildings;

[Tool, GlobalClass]
public partial class BuildingLayoutResource : Resource
{
    [Export] public Texture2D Texture { get; set; } = null!;

    [ExportGroup("Grid Units")] [Export] public int Depth { get; private set; } = 3; // Left facade grid count
    [Export] public int Width { get; private set; } = 4; // Right facade grid count
    [Export] public int Height { get; private set; } = 5; // Vertical grid count

    [ExportGroup("Calculated Dimensions (Pixels)")]
    [Export]
    public float BuildingHeight { get; set; }

    [Export] public float BuildingWidth { get; set; }
    [Export] public float BuildingDepth { get; set; }
    [Export] public Vector2 GridOffset { get; set; }
    [Export] public float BuildingAngle { get; set; } = 20.75f;

    [ExportToolButton("Calculate Facades", Icon = "Ruler")]
    private Callable CalculateFacades => Callable.From(CalculateDimensions);

    public void CalculateDimensions()
    {
        if (Texture == null)
        {
            GD.PrintErr("Texture is null.");
            return;
        }

        var img = Texture.GetImage();
        if (img == null) return;

        var imgWidth = img.GetWidth();
        var imgHeight = img.GetHeight();
        var halfHeight = imgHeight / 2;

        // 1. Scan for Left and Right X-positions STRICLY using the bottom half rows
        // This guarantees we find the true outer base corners without roof interference!
        var leftX = ScanXFromLeftBottomHalf(img, imgWidth, imgHeight, halfHeight);
        var rightX = ScanXFromRightBottomHalf(img, imgWidth, imgHeight, halfHeight);

        // 2. Scan the absolute top and bottom rows of the FULL image for total vertical bounds
        var topY = ScanYFromTop(img, imgWidth, imgHeight);
        var bottomY = ScanYFromBottom(img, imgWidth, imgHeight);

        if (leftX == -1 || rightX == -1 || topY == -1 || bottomY == -1)
        {
            GD.PrintErr("Could not find valid building edges in the texture.");
            return;
        }

        // 3. Mathematical Isometric Projection
        var angleRad = Mathf.DegToRad(BuildingAngle);
        var tanAngle = Mathf.Tan(angleRad);

        // The true horizontal pixel span across the base corners
        float baselineHorizontalWidth = rightX - leftX;
        float totalGridWidthUnits = Width + Depth;
        
        // Distribute the width proportionally using your 3:4 grid unit ratio
        var pixelWidthPerUnit = baselineHorizontalWidth / totalGridWidthUnits;
        BuildingDepth = Depth * pixelWidthPerUnit;
        BuildingWidth = Width * pixelWidthPerUnit;

        // Total vertical span of the entire texture asset
        float totalPixelHeight = bottomY - topY;

        // Visual height occupied by the roof projection slanting up and back
        var roofProjectedHeight = totalGridWidthUnits * pixelWidthPerUnit * tanAngle;
        
        // Deduct the roof's visual height from the total asset height to find the true vertical wall height
        BuildingHeight = totalPixelHeight - roofProjectedHeight;

        // 4. Pinpoint the Front Center Ground Origin Vertex
        var frontCornerX = leftX + BuildingDepth;
        
        // Projecting from the lowest absolute row down to the center seam based on asymmetric width layout
        float frontCornerY = bottomY;
        if (Width > Depth)
        {
            frontCornerY = bottomY - (BuildingWidth * tanAngle);
        }
        else if (Depth > Width)
        {
            frontCornerY = bottomY - (BuildingDepth * tanAngle);
        }

        // Drop the grid baseline down by one cell height to match your mesh origin anchor
        var singleCellHeightPixels = BuildingHeight / Height;
        frontCornerY += singleCellHeightPixels;

        // 5. Calculate Center Offset for Godot's Renderer
        var textureCenter = new Vector2(imgWidth / 2f, imgHeight / 2f);
        GridOffset = new Vector2(frontCornerX - textureCenter.X, frontCornerY - textureCenter.Y);

        EmitChanged();
        GD.Print($"Corrected Full Scale -> Width: {BuildingWidth}px | Depth: {BuildingDepth}px | Height: {BuildingHeight}px | Offset: {GridOffset}");
    }

    #region Scanners (Only X boundaries look at the bottom half)

    private static int ScanXFromLeftBottomHalf(Image img, int w, int h, int startY)
    {
        for (var x = 0; x < w; x++) {
            for (var y = startY; y < h; y++) {
                if (img.GetPixel(x, y).A > 0.1f) return x; // Found leftmost X in bottom half
            }
        }
        return -1;
    }

    private static int ScanXFromRightBottomHalf(Image img, int w, int h, int startY)
    {
        for (var x = w - 1; x >= 0; x--) {
            for (var y = startY; y < h; y++) {
                if (img.GetPixel(x, y).A > 0.1f) return x; // Found rightmost X in bottom half
            }
        }
        return -1;
    }

    private static int ScanYFromTop(Image img, int w, int h)
    {
        for (var y = 0; y < h; y++) {
            for (var x = 0; x < w; x++) {
                if (img.GetPixel(x, y).A > 0.1f) return y; // Absolute top Y of entire image
            }
        }
        return -1;
    }

    private static int ScanYFromBottom(Image img, int w, int h)
    {
        for (var y = h - 1; y >= 0; y--) {
            for (var x = 0; x < w; x++) {
                if (img.GetPixel(x, y).A > 0.1f) return y; // Absolute bottom Y of entire image
            }
        }
        return -1;
    }

    #endregion
}