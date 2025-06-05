using Godot;
using System;
using System.Collections.Generic;

public partial class HexTile : Area2D
{
    public HexCoord HexCoord { get; set; }
    public int Type { get; set; } = 0;
    public float HexSize { get; set; } = 64.0f;

    private Label _label;
    private Polygon2D _polygon;
    private Line2D _line;

    // 不同类型的颜色
    private readonly Color[] _colors = new Color[]
    {
        new Color(0.2f, 0.8f, 0.2f),  // 草地
        new Color(0.8f, 0.7f, 0.3f),  // 沙漠
        new Color(0.1f, 0.3f, 0.8f),  // 水
        new Color(0.5f, 0.5f, 0.5f),  // 岩石
        new Color(0.8f, 0.2f, 0.2f)   // 熔岩
    };

    // 不同类型的可通行性
    private readonly bool[] _passable = new bool[]
    {
        true,   // 草地可通行
        true,   // 沙漠可通行
        false,  // 水不可通行
        true,   // 岩石可通行
        false   // 熔岩不可通行
    };

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        DrawHex();
        UpdateLabel();
    }

    // 绘制六边形
    private void DrawHex()
    {
        var points = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = 2 * Mathf.Pi * i / 6 + Mathf.Pi / 6;
            points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * HexSize;
        }
        
        _polygon = new Polygon2D();
        _polygon.Polygon = points;
        _polygon.Color = _colors[Type];
        
        // 添加边框
        _line = new Line2D();
        var linePoints = new Vector2[7];
        for (int i = 0; i < 6; i++)
        {
            linePoints[i] = points[i];
        }
        linePoints[6] = points[0];
        _line.Points = linePoints;
        _line.Width = 2.0f;
        _line.DefaultColor = Colors.Black;
        
        AddChild(_polygon);
        AddChild(_line);
    }

    // 更新标签显示坐标
    private void UpdateLabel()
    {
        if (_label != null && HexCoord != null)
        {
            _label.Text = $"{HexCoord.Q},{HexCoord.R},{HexCoord.S}";
            GD.Print($"HexTile at {HexCoord.Q},{HexCoord.R},{HexCoord.S} with Type {Type}");
        }
    }

    // 设置瓦片类型
    public void SetType(int newType)
    {
        Type = newType;
        if (_polygon != null)
        {
            _polygon.Color = _colors[Type];
        }
    }

    // 判断瓦片是否可通行
    public bool IsPassable()
    {
        return _passable[Type];
    }

    // 处理点击事件
    private void OnInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                // 左键点击切换类型
                Type = (Type + 1) % _colors.Length;
                SetType(Type);
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right)
            {
                // 右键点击触发自定义事件
                GetParent<HexMap>().EmitSignal(HexMap.SignalName.TileRightClicked, this);
            }
        }
    }
}