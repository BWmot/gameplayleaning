using Godot;
using UIFramework.Enums;

public static class UIConfig
{
    public const float DefaultAnimationDuration = 0.3f;
    public const float DefaultFadeInTime = 0.3f;
    public const float DefaultFadeOutTime = 0.2f;
    public static bool UseAnimations = true;
    public static Color DefaultButtonColor = Colors.White;
    
    // UI路径配置
    public static readonly string UIBasePath = "res://UIFramework/Scenes/";
    
    // 是否启用UI缓存
    public static bool EnableUICache = true;
    
    // 默认的UI层级
    public static UIFramework.Enums.UILayer DefaultLayer = UIFramework.Enums.UILayer.Normal;
    
    // 是否启用自动屏幕适配
    public static bool EnableAutoScreenAdaptation = true;
}