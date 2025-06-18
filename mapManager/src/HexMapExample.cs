using Godot;

public partial class HexMapExample : Node2D
{
    private HexTileMap hexMap;

    public override void _Ready()
    {
        // 获取HexTileMap节点
        hexMap = GetNode<HexTileMap>("HexTileMap");
        
        // 创建一些示例瓦片
        CreateExampleMap();
    }

    private void CreateExampleMap()
    {
        // 创建一个7x7的六边形地图
        var center = new HexCoordinates(0, 0);
        var hexes = hexMap.GetHexesInRange(center, 3);
        
        foreach (var hex in hexes)
        {
            // 设置不同类型的瓦片
            int tileType = (hex.Q + hex.R) % 3;
            hexMap.SetHexTile(hex, 1, new Vector2I(tileType, 0));
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            // 鼠标点击获取六边形坐标
            Vector2 globalPos = GetGlobalMousePosition();
            HexCoordinates clickedHex = hexMap.WorldToHex(globalPos);
            
            GD.Print($"点击的六边形坐标: {clickedHex}");
            
            // 高亮显示相邻的六边形
            var neighbors = clickedHex.GetAllNeighbors();
            foreach (var neighbor in neighbors)
            {
                hexMap.SetHexTile(neighbor, 0, new Vector2I(1, 1)); // 使用高亮瓦片
            }
        }
    }
}