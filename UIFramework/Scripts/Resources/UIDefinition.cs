using Godot;
using System;
using UIFramework.Enums;

namespace UIFramework.Resources
{
    [Tool]
    public partial class UIDefinition : Resource
    {
        [Export]
        public string Type { get; set; }
        
        [Export]
        public string Layer { get; set; }
        
        [Export]
        public string ScenePath { get; set; }
        
        public UIType GetUIType()
        {
            if (Type != null && Enum.TryParse(Type, out UIType result))
            {
                return result;
            }
            
            GD.PushWarning($"Failed to parse UI type: {Type}");
            return UIType.MainMenu; // 默认返回
        }
        
        public UIFramework.Enums.UILayer GetUILayer()
        {
            if (Layer != null && Enum.TryParse(Layer, out UIFramework.Enums.UILayer result))
            {
                return result;
            }
            
            GD.PushWarning($"Failed to parse UI layer: {Layer}");
            return UIFramework.Enums.UILayer.Normal; // 默认返回
        }
    }
}