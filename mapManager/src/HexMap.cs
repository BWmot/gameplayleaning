using Godot;
using System;
using System.Collections.Generic;

public partial class HexMap : Node2D
{
    [Export] public float HexSize { get; set; } = 64.0f;
    [Export] public int MapWidth { get; set; } = 5;
    [Export] public int MapHeight { get; set; } = 5;

    // 存储地图数据
    private Dictionary<Vector3I, HexTile> _tiles = new Dictionary<Vector3I, HexTile>();
    private PackedScene _tileScene;

    [Signal]
    public delegate void TileRightClickedEventHandler(HexTile tile);

    public override void _Ready()
    {
        _tileScene = GD.Load<PackedScene>("res://mapManager/sence/hex_tile.tscn");
        GenerateEmptyMap();
    }

    // 生成一个空地图
    public void GenerateEmptyMap()
    {
        for (int q = 0; q < MapWidth; q++)
        {
            for (int r = 0; r < MapHeight; r++)
            {
                var hexCoord = new HexCoord(q, r);
                CreateTile(hexCoord);
            }
        }
    }

    // 创建一个六边形瓦片
    public HexTile CreateTile(HexCoord hexCoord)
    {
        var tile = _tileScene.Instantiate<HexTile>();
        tile.HexCoord = hexCoord;
        
        // 计算瓦片的世界坐标
        Vector2 position = HexToPixel(hexCoord);
        tile.Position = position;
        
        AddChild(tile);
        _tiles[hexCoord.ToVector3I()] = tile;
        return tile;
    }

    // 六边形坐标转换为像素坐标（尖顶朝上）
    public Vector2 HexToPixel(HexCoord hex)
    {
        float x = HexSize * (Mathf.Sqrt(3) * hex.Q + Mathf.Sqrt(3) / 2 * hex.R);
        float y = HexSize * (3.0f / 2.0f * hex.R);
        return new Vector2(x, y);
    }

    // 像素坐标转换为六边形坐标
    public HexCoord PixelToHex(Vector2 point)
    {
        float q = (Mathf.Sqrt(3) / 3 * point.X - 1.0f / 3 * point.Y) / HexSize;
        float r = (2.0f / 3 * point.Y) / HexSize;
        
        // 使用四舍五入转换为最接近的六边形坐标
        return HexRound(q, r);
    }

    // 四舍五入到最近的六边形坐标
    private HexCoord HexRound(float qFloat, float rFloat)
    {
        float sFloat = -qFloat - rFloat;
        
        int q = (int)Mathf.Round(qFloat);
        int r = (int)Mathf.Round(rFloat);
        int s = (int)Mathf.Round(sFloat);
        
        float qDiff = Math.Abs(q - qFloat);
        float rDiff = Math.Abs(r - rFloat);
        float sDiff = Math.Abs(s - sFloat);
        
        if (qDiff > rDiff && qDiff > sDiff)
        {
            q = -r - s;
        }
        else if (rDiff > sDiff)
        {
            r = -q - s;
        }
        else
        {
            s = -q - r;
        }
        
        return new HexCoord(q, r, s);
    }

    // 获取指定坐标的瓦片
    public HexTile GetTile(HexCoord hexCoord)
    {
        if (_tiles.TryGetValue(hexCoord.ToVector3I(), out HexTile tile))
        {
            return tile;
        }
        return null;
    }

    // 根据坐标设置瓦片的类型
    public void SetTileType(HexCoord hexCoord, int type)
    {
        var tile = GetTile(hexCoord);
        if (tile != null)
        {
            tile.SetType(type);
        }
    }

    // 获取两点间的路径（使用A*算法）
    public List<HexCoord> FindPath(HexCoord start, HexCoord end)
    {
        var path = new List<HexCoord>();
        var openSet = new List<HexCoord> { start };
        var cameFrom = new Dictionary<Vector3I, HexCoord>();
        
        var gScore = new Dictionary<Vector3I, float>();
        gScore[start.ToVector3I()] = 0;
        
        var fScore = new Dictionary<Vector3I, float>();
        fScore[start.ToVector3I()] = start.DistanceTo(end);
        
        while (openSet.Count > 0)
        {
            // 找到f_score最小的节点
            HexCoord current = openSet[0];
            int currentIdx = 0;
            for (int i = 1; i < openSet.Count; i++)
            {
                var node = openSet[i];
                float currentFScore = fScore.TryGetValue(current.ToVector3I(), out float cfs) ? cfs : float.MaxValue;
                float nodeFScore = fScore.TryGetValue(node.ToVector3I(), out float nfs) ? nfs : float.MaxValue;
                
                if (nodeFScore < currentFScore)
                {
                    current = node;
                    currentIdx = i;
                }
            }
            
            // 如果到达目标，重建路径
            if (current.ToVector3I() == end.ToVector3I())
            {
                var curr = current;
                path.Add(curr);
                while (cameFrom.ContainsKey(curr.ToVector3I()))
                {
                    curr = cameFrom[curr.ToVector3I()];
                    path.Insert(0, curr);
                }
                return path;
            }
            
            openSet.RemoveAt(currentIdx);
            
            // 检查每个邻居
            foreach (var neighbor in current.GetAllNeighbors())
            {
                var tile = GetTile(neighbor);
                
                // 如果瓦片不存在或不可通行，跳过
                if (tile == null || !tile.IsPassable())
                {
                    continue;
                }
                
                float currentGScore = gScore.TryGetValue(current.ToVector3I(), out float cgs) ? cgs : float.MaxValue;
                float tentativeGScore = currentGScore + 1;
                
                float neighborGScore = gScore.TryGetValue(neighbor.ToVector3I(), out float ngs) ? ngs : float.MaxValue;
                if (tentativeGScore < neighborGScore)
                {
                    cameFrom[neighbor.ToVector3I()] = current;
                    gScore[neighbor.ToVector3I()] = tentativeGScore;
                    fScore[neighbor.ToVector3I()] = tentativeGScore + neighbor.DistanceTo(end);
                    
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        
        return new List<HexCoord>(); // 没有找到路径
    }

    // 生成随机地图
    public void GenerateRandomMap()
    {
        ClearMap();
        for (int q = 0; q < MapWidth; q++)
        {
            for (int r = 0; r < MapHeight; r++)
            {
                var hexCoord = new HexCoord(q, r);
                var tile = CreateTile(hexCoord);
                int randomType = new Random().Next(0, 5);  // 随机类型
                tile.SetType(randomType);
            }
        }
    }

    // 清除地图
    public void ClearMap()
    {
        foreach (var tile in _tiles.Values)
        {
            tile.QueueFree();
        }
        _tiles.Clear();
    }
}