using Godot;
using System.Collections.Generic;

public partial class HexTileResource : Resource
{
    [Export]
    public TileSet TileSet { get; set; }
    
    [Export]
    public int SourceId { get; set; } = 0;

    // 定义不同类型的瓦片
    public enum TileType
    {
        Grass = 0,
        Water = 1,
        Mountain = 2,
        Forest = 3,
        Desert = 4,
        Highlight = 5,
        White = 6 // 空白瓦片
    }
    
    // 瓦片类型到图集坐标的映射
    private Dictionary<TileType, Vector2I> tileAtlasCoords = new Dictionary<TileType, Vector2I>
    {
        { TileType.Grass, new Vector2I(0, 2) },
        { TileType.Water, new Vector2I(0, 1) },
        { TileType.Mountain, new Vector2I(0, 0) },
        { TileType.Forest, new Vector2I(0, 5) },
        { TileType.Desert, new Vector2I(0, 3) },
        { TileType.Highlight, new Vector2I(0, 6) },
        { TileType.White, new Vector2I(999, 999) } // 突出显示瓦片
    };
    
    public Vector2I GetAtlasCoords(TileType tileType)
    {
        return tileAtlasCoords.GetValueOrDefault(tileType, Vector2I.Zero);
    }
    
    public void SetAtlasCoords(TileType tileType, Vector2I coords)
    {
        tileAtlasCoords[tileType] = coords;
    }
    
    // 获取所有可用的瓦片类型
    public TileType[] GetAllTileTypes()
    {
        return (TileType[])System.Enum.GetValues(typeof(TileType));
    }
}