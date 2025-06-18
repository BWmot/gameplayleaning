using Godot;
using System;
using System.Collections.Generic;

public struct HexCoordinates : IEquatable<HexCoordinates>
{
    public int Q { get; }
    public int R { get; }
    public int S { get; }

    public HexCoordinates(int q, int r)
    {
        Q = q;
        R = r;
        S = -q - r; // 立方体坐标约束：q + r + s = 0
    }

    public HexCoordinates(int q, int r, int s)
    {
        if (q + r + s != 0)
            throw new ArgumentException("立方体坐标必须满足 q + r + s = 0");
        
        Q = q;
        R = r;
        S = s;
    }

    // 六个方向的单位向量
    public static readonly HexCoordinates[] Directions = new HexCoordinates[]
    {
        new HexCoordinates(1, 0),   // 东
        new HexCoordinates(1, -1),  // 东北
        new HexCoordinates(0, -1),  // 西北
        new HexCoordinates(-1, 0),  // 西
        new HexCoordinates(-1, 1),  // 西南
        new HexCoordinates(0, 1)    // 东南
    };

    // 获取相邻六边形
    public HexCoordinates GetNeighbor(int direction)
    {
        return this + Directions[direction];
    }

    public List<HexCoordinates> GetAllNeighbors()
    {
        var neighbors = new List<HexCoordinates>();
        for (int i = 0; i < 6; i++)
        {
            neighbors.Add(GetNeighbor(i));
        }
        return neighbors;
    }

    // 计算距离
    public int DistanceTo(HexCoordinates other)
    {
        return (Math.Abs(Q - other.Q) + Math.Abs(R - other.R) + Math.Abs(S - other.S)) / 2;
    }

    // 运算符重载
    public static HexCoordinates operator +(HexCoordinates a, HexCoordinates b)
    {
        return new HexCoordinates(a.Q + b.Q, a.R + b.R);
    }

    public static HexCoordinates operator -(HexCoordinates a, HexCoordinates b)
    {
        return new HexCoordinates(a.Q - b.Q, a.R - b.R);
    }

    public bool Equals(HexCoordinates other)
    {
        return Q == other.Q && R == other.R && S == other.S;
    }

    public override bool Equals(object obj)
    {
        return obj is HexCoordinates other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Q, R, S);
    }

    public override string ToString()
    {
        return $"Hex({Q}, {R}, {S})";
    }
}