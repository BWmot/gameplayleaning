using Godot;
using System.Collections.Generic;

public partial class HexTileMap : TileMapLayer
{
    [Export]
    public float HexSize { get; set; } = 32.0f;
    
    [Export]
    public bool FlatTop { get; set; } = true; // true为平顶，false为尖顶

    private Dictionary<HexCoordinates, int> hexTiles = new Dictionary<HexCoordinates, int>();

    public override void _Ready()
    {
        // 确保使用六边形瓦片集
        SetupHexTileSet();
    }

    private void SetupHexTileSet()
    {
        if (TileSet == null)
        {
            GD.PrintErr("需要设置TileSet资源");
            return;
        }
        
        // 这里可以添加验证TileSet是否为六边形的逻辑
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

    // 设置六边形瓦片
    public void SetHexTile(HexCoordinates hex, int sourceId, Vector2I atlasCoords)
    {
        // 将立方体坐标转换为TileMap的坐标系统
        Vector2I tilePos = HexToTileMapCoords(hex);
        SetCell(tilePos, sourceId, atlasCoords);
        hexTiles[hex] = sourceId;
    }

    // 获取六边形瓦片
    public int GetHexTile(HexCoordinates hex)
    {
        return hexTiles.GetValueOrDefault(hex, -1);
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

    // 获取从起点到终点的路径
    public List<HexCoordinates> GetPath(HexCoordinates start, HexCoordinates end)
    {
        var path = new List<HexCoordinates>();
        int distance = start.DistanceTo(end);
        
        for (int i = 0; i <= distance; i++)
        {
            float t = distance == 0 ? 0.0f : (float)i / distance;
            path.Add(LerpHex(start, end, t));
        }
        
        return path;
    }

    private HexCoordinates LerpHex(HexCoordinates a, HexCoordinates b, float t)
    {
        float q = Mathf.Lerp(a.Q, b.Q, t);
        float r = Mathf.Lerp(a.R, b.R, t);
        return RoundHex(q, r);
    }
}