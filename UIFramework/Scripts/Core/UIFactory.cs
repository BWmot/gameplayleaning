using Godot;
using System;
using System.Collections.Generic;
using UIFramework.Enums;

public static class UIFactory
{
    private static Dictionary<UIType, string> _uiPaths = new Dictionary<UIType, string>()
    {
        { UIType.MainMenu, "res://UIFramework/Scenes/MainMenu.tscn" },
        { UIType.PauseMenu, "res://UIFramework/Scenes/PauseMenu.tscn" },
        { UIType.GameOverScreen, "res://UIFramework/Scenes/GameOverScreen.tscn" },
        { UIType.SettingsMenu, "res://UIFramework/Scenes/SettingsMenu.tscn" },
        { UIType.Inventory, "res://UIFramework/Scenes/Inventory.tscn" },
        { UIType.CharacterScreen, "res://UIFramework/Scenes/CharacterScreen.tscn" },
        { UIType.LoadingScreen, "res://UIFramework/Scenes/LoadingScreen.tscn" },
        { UIType.DialogueBox, "res://UIFramework/Scenes/DialogueBox.tscn" },
        { UIType.Notification, "res://UIFramework/Scenes/Notification.tscn" },
        { UIType.HUD, "res://UIFramework/Scenes/HUD.tscn" }
    };

    public static UIBase CreateUI(UIType type)
    {
        UIBase uiInstance = null;
        
        // 尝试从场景文件加载
        uiInstance = LoadUIFromScene(type);
        
        // 如果加载失败，则尝试实例化代码里定义的UI类
        if (uiInstance == null)
        {
            uiInstance = CreateUIInstance(type);
        }

        if (uiInstance != null)
        {
            uiInstance.Initialize();
        }
        else
        {
            GD.PrintErr($"Failed to create UI of type {type}. Make sure the scene file exists or the class is defined.");
        }

        return uiInstance;
    }
    
    private static UIBase CreateUIInstance(UIType type)
    {
        UIBase uiInstance = null;

        switch (type)
        {
            case UIType.MainMenu:
                // 这里应该实例化具体的UI类
                // 例如：uiInstance = new MainMenuUI();
                GD.PushWarning($"Implementation for {type} not found in code. Try using a scene file instead.");
                break;
            case UIType.PauseMenu:
                // uiInstance = new PauseMenuUI();
                GD.PushWarning($"Implementation for {type} not found in code. Try using a scene file instead.");
                break;
            case UIType.GameOverScreen:
                // uiInstance = new GameOverScreenUI();
                GD.PushWarning($"Implementation for {type} not found in code. Try using a scene file instead.");
                break;
            // 添加更多UI类型的处理
            default:
                GD.PrintErr("Unknown UI type: " + type);
                break;
        }
        
        return uiInstance;
    }
    
    public static UIBase LoadUIFromScene(UIType type)
    {
        if (!_uiPaths.ContainsKey(type))
        {
            GD.PushWarning($"No scene path defined for UI type: {type}");
            return null;
        }
        
        string path = _uiPaths[type];
        
        // 检查文件是否存在
        if (!ResourceLoader.Exists(path))
        {
            GD.PushWarning($"UI scene file not found at path: {path}");
            return null;
        }
        
        // 加载场景
        PackedScene scene = ResourceLoader.Load<PackedScene>(path);
        if (scene == null)
        {
            GD.PushWarning($"Failed to load scene from path: {path}");
            return null;
        }
        
        // 实例化场景
        Node instance = scene.Instantiate();
        if (instance is UIBase uiBase)
        {
            return uiBase;
        }
        else
        {
            GD.PrintErr($"Scene at {path} does not inherit from UIBase");
            instance.QueueFree();  // 清理不需要的实例
            return null;
        }
    }
    
    public static void RegisterUIPath(UIType type, string path)
    {
        _uiPaths[type] = path;
    }
}