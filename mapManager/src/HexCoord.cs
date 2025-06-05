using Godot;
using System;
using System.Collections.Generic;

public partial class HexCoord : GodotObject
{
    public int Q { get; private set; }
    public int R { get; private set; }
    public int S { get; private set; }

    // 预定义的六个方向向量
    public static readonly Vector3I[] Directions = new Vector3I[]
    {
        new Vector3I(1, -1, 0), new Vector3I(1, 0, -1), new Vector3I(0, 1, -1),
        new Vector3I(-1, 1, 0), new Vector3I(-1, 0, 1), new Vector3I(0, -1, 1)
    };

    public HexCoord(int q, int r, int s = 0)
    {
        Q = q;
        R = r;
        S = s != 0 ? s : -q - r;
        
        // 验证立方体坐标约束
        if (Q + R + S != 0)
        {
            GD.PushError("立方体坐标必须满足 q + r + s = 0");
        }
    }

    // 从偏移坐标转换为立方体坐标
    public static HexCoord FromOffset(int col, int row)
    {
        int q = col;
        int r = row - (col - (col & 1)) / 2;
        return new HexCoord(q, r);
    }

    // 转换为偏移坐标
    public Vector2I ToOffset()
    {
        int col = Q;
        int row = R + (Q - (Q & 1)) / 2;
        return new Vector2I(col, row);
    }

    // 计算两个六边形之间的距离
    public int DistanceTo(HexCoord other)
    {
        return (Math.Abs(Q - other.Q) + Math.Abs(R - other.R) + Math.Abs(S - other.S)) / 2;
    }

    // 获取指定方向上的邻居
    public HexCoord Neighbor(int direction)
    {
        Vector3I dir = Directions[direction];
        return new HexCoord(Q + dir.X, R + dir.Y, S + dir.Z);
    }

    // 获取所有邻居
    public List<HexCoord> GetAllNeighbors()
    {
        var neighbors = new List<HexCoord>();
        for (int i = 0; i < 6; i++)
        {
            neighbors.Add(Neighbor(i));
        }
        return neighbors;
    }

    // 转换为Vector3I，便于存储和比较
    public Vector3I ToVector3I()
    {
        return new Vector3I(Q, R, S);
    }

    // 从Vector3I创建HexCoord
    public static HexCoord FromVector3I(Vector3I v)
    {
        return new HexCoord(v.X, v.Y, v.Z);
    }

    // 获取在给定半径范围内的所有六边形
    public List<HexCoord> GetRange(int radius)
    {
        var results = new List<HexCoord>();
        for (int dq = -radius; dq <= radius; dq++)
        {
            for (int dr = Math.Max(-radius, -dq - radius); dr <= Math.Min(radius, -dq + radius); dr++)
            {
                int ds = -dq - dr;
                results.Add(new HexCoord(Q + dq, R + dr, S + ds));
            }
        }
        return results;
    }

    public override string ToString()
    {
        return $"HexCoord({Q}, {R}, {S})";
    }
}