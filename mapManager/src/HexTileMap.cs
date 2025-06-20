using Godot;
using System.Collections.Generic;

public partial class HexTileMap : TileMapLayer
{
    [Export]
    public float HexSize { get; set; } = 32.0f;
    
    [Export]
    public bool FlatTop { get; set; } = true;
    
    [Export]
    public HexTileResource TileResource { get; set; }

    private Dictionary<HexCoordinates, HexTileResource.TileType> hexTiles = new Dictionary<HexCoordinates, HexTileResource.TileType>();

    public override void _Ready()
    {
        // 确保使用六边形瓦片集
        SetupHexTileSet();
    }

    private void SetupHexTileSet()
    {
        if (TileResource?.TileSet == null)
        {
            GD.PrintErr("需要设置HexTileResource和TileSet资源");
            return;
        }
        
        // 设置TileMapLayer的TileSet
        TileSet = TileResource.TileSet;
        
        // 验证源是否存在
        if (!TileSet.HasSource(TileResource.SourceId))
        {
            GD.PrintErr($"TileSet中不存在源ID: {TileResource.SourceId}");
            return;
        }
        
        var source = TileSet.GetSource(TileResource.SourceId);
        if (source is TileSetAtlasSource atlasSource)
        {
            GD.Print($"图集源加载成功, 尺寸: {atlasSource.TextureRegionSize}");
        }
    }

    // 立方体坐标转换为世界坐标
    public Vector2 HexToWorld(HexCoordinates hex)
    {
        float x, y;
        
        if (FlatTop)
        {
            x = HexSize * (3.0f / 2.0f * hex.Q);
            y = HexSize * (Mathf.Sqrt(3.0f) / 2.0f * hex.Q + Mathf.Sqrt(3.0f) * hex.R);
        }
        else
        {
            x = HexSize * (Mathf.Sqrt(3.0f) * hex.Q + Mathf.Sqrt(3.0f) / 2.0f * hex.R);
            y = HexSize * (3.0f / 2.0f * hex.R);
        }
        
        return new Vector2(x, y);
    }

    // 世界坐标转换为立方体坐标
    public HexCoordinates WorldToHex(Vector2 worldPos)
    {
        float q, r;
        
        if (FlatTop)
        {
            q = (2.0f / 3.0f * worldPos.X) / HexSize;
            r = (-1.0f / 3.0f * worldPos.X + Mathf.Sqrt(3.0f) / 3.0f * worldPos.Y) / HexSize;
        }
        else
        {
            q = (Mathf.Sqrt(3.0f) / 3.0f * worldPos.X - 1.0f / 3.0f * worldPos.Y) / HexSize;
            r = (2.0f / 3.0f * worldPos.Y) / HexSize;
        }
        
        return RoundHex(q, r);
    }

    // 立方体坐标转为轴坐标
    public Vector2I HexToAxial(HexCoordinates hex)
    {
        int col = hex.Q;
        int row = hex.R + (hex.Q - (hex.Q & 1)) / 2; // 偶数列偏移
        return new Vector2I(col, row);
    }

    // 轴坐标转为立方体坐标
    public HexCoordinates AxialToHex(Vector2I axial)
    {
        int q = axial.X;
        int r = axial.Y - (axial.X - (axial.X & 1)) / 2; // 偶数列偏移
        return new HexCoordinates(q, r);
    }


    // 四舍五入到最近的六边形
    private HexCoordinates RoundHex(float q, float r)
    {
        float s = -q - r;

        int roundQ = Mathf.RoundToInt(q);
        int roundR = Mathf.RoundToInt(r);
        int roundS = Mathf.RoundToInt(s);

        float qDiff = Mathf.Abs(roundQ - q);
        float rDiff = Mathf.Abs(roundR - r);
        float sDiff = Mathf.Abs(roundS - s);

        if (qDiff > rDiff && qDiff > sDiff)
        {
            roundQ = -roundR - roundS;
        }
        else if (rDiff > sDiff)
        {
            roundR = -roundQ - roundS;
        }

        return new HexCoordinates(roundQ, roundR);
    }

    // 设置六边形瓦片（通过瓦片类型）
    public void SetHexTile(HexCoordinates hex, HexTileResource.TileType tileType)
    {
        if (TileResource == null) return;
        
        Vector2I tilePos = HexToTileMapCoords(hex);
        Vector2I atlasCoords = TileResource.GetAtlasCoords(tileType);
        
        SetCell(tilePos, TileResource.SourceId, atlasCoords);
        hexTiles[hex] = tileType;
    }

    // 设置六边形瓦片（直接指定图集坐标）
    public void SetHexTile(HexCoordinates hex, Vector2I atlasCoords)
    {
        if (TileResource == null) return;
        
        Vector2I tilePos = HexToTileMapCoords(hex);
        SetCell(tilePos, TileResource.SourceId, atlasCoords);
    }

    // 获取六边形瓦片类型
    public HexTileResource.TileType GetHexTileType(HexCoordinates hex)
    {
        return hexTiles.GetValueOrDefault(hex, HexTileResource.TileType.Grass);
    }

    // 移除六边形瓦片
    public void RemoveHexTile(HexCoordinates hex)
    {
        Vector2I tilePos = HexToTileMapCoords(hex);
        EraseCell(tilePos);
        hexTiles.Remove(hex);
    }

    // 立方体坐标转换为TileMap坐标
    private Vector2I HexToTileMapCoords(HexCoordinates hex)
    {
        // 使用offset坐标系统
        int col = hex.Q;
        int row = hex.R + (hex.Q - (hex.Q & 1)) / 2;
        return new Vector2I(col, row);
    }

    // 获取指定范围内的所有六边形
    public List<HexCoordinates> GetHexesInRange(HexCoordinates center, int range)
    {
        var results = new List<HexCoordinates>();
        
        for (int q = -range; q <= range; q++)
        {
            int r1 = Mathf.Max(-range, -q - range);
            int r2 = Mathf.Min(range, -q + range);
            
            for (int r = r1; r <= r2; r++)
            {
                results.Add(center + new HexCoordinates(q, r));
            }
        }
        
        return results;
    }

    // 随机放置瓦片
    public void PlaceRandomTile(HexCoordinates hex)
    {
        if (TileResource == null) return;
        
        var tileTypes = TileResource.GetAllTileTypes();
        var randomType = tileTypes[GD.RandRange(0, tileTypes.Length - 1)];
        SetHexTile(hex, randomType);
    }
}