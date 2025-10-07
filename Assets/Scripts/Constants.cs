using UnityEngine;

public static class Constants
{
    public const float TileSize = 1600.0f / 3.0f;
    public const float MaxXY = 32.0f * TileSize;
    public const float ChunkSize = TileSize / 16.0f;
    public const float UnitSize = ChunkSize / 8.0f;
}
