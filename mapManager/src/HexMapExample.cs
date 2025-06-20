using Godot;

public partial class HexMapExample : Node2D
{
    private HexTileMap hexMap;
    private HexCoordinates lastHighlightedHex = new HexCoordinates(999, 999); // 无效坐标作为初始值

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
            // 根据位置设置不同类型的瓦片
            HexTileResource.TileType tileType = GetTileTypeByPosition(hex);
            hexMap.SetHexTile(hex, tileType);
        }
    }

    private HexTileResource.TileType GetTileTypeByPosition(HexCoordinates hex)
    {
        // 根据坐标决定瓦片类型
        int distance = hex.DistanceTo(new HexCoordinates(0, 0));
        
        return distance switch
        {
            0 => HexTileResource.TileType.Mountain,
            1 => HexTileResource.TileType.Forest,
            2 => HexTileResource.TileType.Grass,
            3 => HexTileResource.TileType.Water,
            4 => HexTileResource.TileType.Desert,
            _ => HexTileResource.TileType.White // 默认类型
        };
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            HandleMouseClick(mouseEvent);
        }
        else if (@event is InputEventMouseMotion motionEvent)
        {
            HandleMouseMove(motionEvent);
        }
    }

    private void HandleMouseClick(InputEventMouseButton mouseEvent)
    {
        // 鼠标点击获取六边形坐标
        Vector2 globalPos = GetGlobalMousePosition();
        HexCoordinates clickedHex = hexMap.WorldToHex(globalPos);

        Vector2I axisPos = hexMap.HexToAxial(clickedHex);
        
        GD.Print($"点击的六边形坐标: {clickedHex}, 轴坐标: {axisPos}");
        
        if (mouseEvent.ButtonIndex == MouseButton.Left)
        {
            // 左键点击循环切换瓦片类型
            CycleTileType(clickedHex);
        }
        else if (mouseEvent.ButtonIndex == MouseButton.Right)
        {
            // 右键点击放置随机瓦片
            hexMap.PlaceRandomTile(clickedHex);
        }
    }

    private void HandleMouseMove(InputEventMouseMotion motionEvent)
    {
        Vector2 globalPos = GetGlobalMousePosition();
        HexCoordinates hoveredHex = hexMap.WorldToHex(globalPos);
        
        // 如果悬停在新的六边形上
        if (!hoveredHex.Equals(lastHighlightedHex))
        {
            // 恢复之前高亮的六边形
            if (!lastHighlightedHex.Equals(new HexCoordinates(999, 999)))
            {
                RestoreOriginalTile(lastHighlightedHex);
            }
            
            // 高亮当前悬停的六边形
            HighlightHex(hoveredHex);
            lastHighlightedHex = hoveredHex;
        }
    }

    private void CycleTileType(HexCoordinates hex)
    {
        var currentType = hexMap.GetHexTileType(hex);
        var allTypes = hexMap.TileResource.GetAllTileTypes();
        
        // 找到当前类型的索引
        int currentIndex = System.Array.IndexOf(allTypes, currentType);
        int nextIndex = (currentIndex + 1) % (allTypes.Length - 1); // 排除Highlight类型
        
        hexMap.SetHexTile(hex, allTypes[nextIndex]);
    }

    private void HighlightHex(HexCoordinates hex)
    {
        hexMap.SetHexTile(hex, HexTileResource.TileType.Highlight);
    }

    private void RestoreOriginalTile(HexCoordinates hex)
    {
        // 恢复为原始瓦片类型（这里简单使用grass）
        HexTileResource.TileType originalType = GetTileTypeByPosition(hex);
        hexMap.SetHexTile(hex, originalType);
    }
}