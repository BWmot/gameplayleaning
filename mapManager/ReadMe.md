# 六边形地图生成器使用文档

## 概述

这是一个基于Godot 4.4和C#实现的六边形地图系统，使用立方体坐标系统进行六边形网格计算，支持动态瓦片管理、鼠标交互和可视化高亮效果。

## 系统架构

### 核心组件

1. **HexCoordinates** - 六边形立方体坐标结构
2. **HexTileMap** - 六边形地图管理器
3. **HexTileResource** - 瓦片资源管理器
4. **HexMapExample** - 使用示例和交互控制器

## 快速开始

### 1. 场景设置

创建以下场景结构：

```
Main (Node2D)
├── HexTileMap (TileMapLayer)
└── 其他UI节点...
```

### 2. 资源准备

#### 创建TileSet资源

1. 在文件系统中创建新的TileSet资源
2. 添加TileSetAtlasSource
3. 导入六边形瓦片图集
4. 设置正确的瓦片尺寸和分离参数

#### 创建HexTileResource

1. 创建新的HexTileResource资源
2. 配置TileSet引用和SourceId
3. 根据需要调整瓦片类型映射

### 3. 脚本配置

将以下脚本附加到对应节点：

- HexMapExample.cs → Main节点
- `HexTileMap.cs` → HexTileMap节点

## API参考

### HexCoordinates 结构

立方体坐标系统，遵循 q + r + s = 0 约束。

#### 构造函数

```csharp
HexCoordinates(int q, int r)
HexCoordinates(int q, int r, int s)
```

#### 主要方法

```csharp
// 获取相邻六边形
HexCoordinates GetNeighbor(int direction)
List<HexCoordinates> GetAllNeighbors()

// 计算距离
int DistanceTo(HexCoordinates other)

// 运算符支持
HexCoordinates operator +(HexCoordinates a, HexCoordinates b)
HexCoordinates operator -(HexCoordinates a, HexCoordinates b)
```

#### 方向常量

```csharp
// 六个方向的单位向量
static readonly HexCoordinates[] Directions = {
    new HexCoordinates(1, 0),   // 东
    new HexCoordinates(1, -1),  // 东北
    new HexCoordinates(0, -1),  // 西北
    new HexCoordinates(-1, 0),  // 西
    new HexCoordinates(-1, 1),  // 西南
    new HexCoordinates(0, 1)    // 东南
};
```

### HexTileMap 类

主要的六边形地图管理器。

#### 导出属性

```csharp
[Export] public float HexSize { get; set; } = 32.0f;
[Export] public bool FlatTop { get; set; } = true;
[Export] public HexTileResource TileResource { get; set; }
```

#### 坐标转换方法

```csharp
// 立方体坐标转世界坐标
Vector2 HexToWorld(HexCoordinates hex)

// 世界坐标转立方体坐标
HexCoordinates WorldToHex(Vector2 worldPos)
```

#### 瓦片操作方法

```csharp
// 设置瓦片
void SetHexTile(HexCoordinates hex, HexTileResource.TileType tileType)
void SetHexTile(HexCoordinates hex, Vector2I atlasCoords)

// 获取瓦片类型
HexTileResource.TileType GetHexTileType(HexCoordinates hex)

// 移除瓦片
void RemoveHexTile(HexCoordinates hex)

// 随机放置瓦片
void PlaceRandomTile(HexCoordinates hex)
```

#### 范围和路径方法

```csharp
// 获取指定范围内的所有六边形
List<HexCoordinates> GetHexesInRange(HexCoordinates center, int range)

// 获取路径（需要实现）
List<HexCoordinates> GetPath(HexCoordinates start, HexCoordinates end)
```

### HexTileResource 类

瓦片资源管理器，处理不同瓦片类型到图集坐标的映射。

#### 瓦片类型枚举

```csharp
public enum TileType
{
    Grass = 0,
    Water = 1,
    Mountain = 2,
    Forest = 3,
    Desert = 4,
    Highlight = 5
}
```

#### 主要方法

```csharp
// 获取图集坐标
Vector2I GetAtlasCoords(TileType tileType)

// 设置图集坐标映射
void SetAtlasCoords(TileType tileType, Vector2I coords)

// 获取所有瓦片类型
TileType[] GetAllTileTypes()
```

### HexMapExample 类

使用示例和交互控制器。

#### 交互功能

- **鼠标悬停**: 高亮当前六边形
- **左键点击**: 循环切换瓦片类型
- **右键点击**: 放置随机瓦片

#### 主要方法

```csharp
// 创建示例地图
void CreateExampleMap()

// 根据位置获取瓦片类型
HexTileResource.TileType GetTileTypeByPosition(HexCoordinates hex)

// 处理鼠标交互
void HandleMouseClick(InputEventMouseButton mouseEvent)
void HandleMouseMove(InputEventMouseMotion motionEvent)
```

## 使用示例

### 基本地图创建

```csharp
// 创建中心为(0,0)，半径为3的六边形地图
var center = new HexCoordinates(0, 0);
var hexes = hexMap.GetHexesInRange(center, 3);

foreach (var hex in hexes)
{
    var tileType = DetermineTileType(hex);
    hexMap.SetHexTile(hex, tileType);
}
```

### 邻居查找

```csharp
var hex = new HexCoordinates(0, 0);
var neighbors = hex.GetAllNeighbors();

foreach (var neighbor in neighbors)
{
    GD.Print($"邻居坐标: {neighbor}");
}
```

### 距离计算

```csharp
var hex1 = new HexCoordinates(0, 0);
var hex2 = new HexCoordinates(3, -1);
int distance = hex1.DistanceTo(hex2);
GD.Print($"距离: {distance}");
```

### 自定义瓦片类型映射

```csharp
// 在HexTileResource中自定义映射
tileResource.SetAtlasCoords(TileType.Water, new Vector2I(2, 3));
tileResource.SetAtlasCoords(TileType.Mountain, new Vector2I(1, 2));
```

## 配置选项

### HexTileMap配置

- **HexSize**: 六边形的大小（像素）
- **FlatTop**: true为平顶六边形，false为尖顶六边形
- **TileResource**: HexTileResource资源的引用

### TileSet配置要点

1. 使用TileSetAtlasSource作为瓦片源
2. 确保瓦片尺寸与HexSize匹配
3. 正确设置分离像素以避免渲染问题
4. 瓦片形状应配置为六边形

## 常见问题

### Q: 瓦片显示不正确或错位

A: 检查以下设置：

- TileSet中的瓦片尺寸是否正确
- HexSize是否与实际瓦片大小匹配
- 是否正确设置了FlatTop属性

### Q: 鼠标点击位置不准确

A: 确保：

- HexSize设置正确
- 坐标转换函数使用了正确的六边形布局
- 场景的缩放和变换设置正确

### Q: 如何添加新的瓦片类型

A:

1. 在TileType枚举中添加新类型
2. 在tileAtlasCoords字典中添加对应的图集坐标
3. 更新相关的逻辑代码

## 扩展建议

### 性能优化

- 对于大型地图，考虑实现视口裁剪
- 使用对象池管理临时坐标对象
- 实现分块加载系统

### 功能扩展

- 添加A*寻路算法
- 实现地图序列化/反序列化
- 添加动画过渡效果
- 支持多层地图（地形层、建筑层等）

### 编辑器工具

- 创建自定义编辑器插件
- 添加地图编辑工具面板
- 实现批量操作功能

这个六边形地图系统提供了完整的基础功能，可以根据具体游戏需求进行进一步的定制和扩展。
