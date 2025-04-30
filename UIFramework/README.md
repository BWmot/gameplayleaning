# Godot UI 框架

这是一个为 Godot 4.x 游戏引擎设计的 C# UI 框架，用于统一管理游戏中各种 UI 的生成与销毁。

## 核心功能

1. **统一的 UI 管理**：通过 `UIManager` 统一管理所有 UI 元素的创建、显示和销毁
2. **UI 层级系统**：支持 8 个不同的 UI 层级，确保 UI 元素按正确的顺序显示
3. **UI 动画系统**：为 UI 元素提供丰富的动画效果，如淡入淡出、滑动和缩放
4. **UI 事件系统**：允许 UI 元素之间通过事件进行通信
5. **UI 资源加载**：支持从场景文件自动加载 UI 预制体
6. **UI 缓存机制**：可选择性地缓存 UI 元素以提高性能
7. **UI 弹窗队列**：管理多个弹窗的自动顺序显示
8. **屏幕适配**：自动适应不同屏幕分辨率的 UI 元素

## 主要组件

- **UIManager**: UI 框架的核心管理类，提供统一的 UI 管理接口
- **UIBase**: 所有 UI 元素的基类，提供基本的 UI 功能
- **UIFactory**: 负责创建和加载不同类型的 UI 元素
- **UILayer**: 定义 UI 层级枚举，确保正确的显示顺序
- **UIType**: 定义不同类型的 UI 元素
- **UIAnimationHelper**: 提供常用的 UI 动画效果
- **UIExtensions**: 提供便捷的 UI 扩展方法
- **UIConfig**: 集中管理 UI 框架的配置参数

## 使用方法

### 基本设置

1. 将 UIManager 添加到你的场景中：

```csharp
var uiManager = new UIManager();
AddChild(uiManager);
```

2. 创建 UI 场景并保存在 `res://UIFramework/Scenes/` 目录下

3. 使用 UIManager 显示 UI：

```csharp
// 显示 UI
UIManager.Instance.ShowUI(UIType.MainMenu);

// 隐藏 UI
await UIManager.Instance.HideUI(UIType.MainMenu);
```

### 创建自定义 UI

1. 继承 UIBase 类来创建自定义 UI：

```csharp
public partial class MyCustomUI : UIBase
{
    // 设置 UI 层级
    public override UILayer Layer { get; protected set; } = UILayer.Normal;
    
    public override void _Ready()
    {
        base._Ready();
        // 初始化 UI 元素
    }
    
    public override void SetData(object data)
    {
        // 处理传入的数据
    }
}
```

2. 在 UIFactory 中注册自定义 UI 的路径：

```csharp
UIFactory.RegisterUIPath(UIType.MyCustomUI, "res://UIFramework/Scenes/MyCustomUI.tscn");
```

### 使用 UI 事件系统

```csharp
// 注册事件
UIManager.Instance.RegisterEvent("my_event", (data) => {
    GD.Print("收到事件，数据：" + data);
});

// 触发事件
UIManager.Instance.TriggerEvent("my_event", "Hello World");

// 取消注册事件
UIManager.Instance.UnregisterEvent("my_event", myHandler);
```

### 使用弹窗队列

```csharp
// 显示弹窗（会自动按顺序显示）
UIManager.Instance.ShowPopup(UIType.Notification, notificationData);
UIManager.Instance.ShowPopup(UIType.DialogueBox, dialogueData);
```

## 示例

查看 `UIFramework/Examples` 目录中的示例，了解如何使用这个框架。

## 注意事项

- 确保 UI 场景继承自 UIBase 类
- 使用 UILayer 枚举来控制 UI 的显示顺序
- 使用 UIType 枚举来标识不同类型的 UI 元素
- 在需要传递数据给 UI 时，创建专用的数据类并通过 SetData 方法传递