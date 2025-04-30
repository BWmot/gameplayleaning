using Godot;
using System;
using System.Collections.Generic;
using UIFramework.Enums;

namespace UIFramework.Resources
{
    [Tool]
    public partial class UIDefinitionsResource : Resource
    {
        // 使用数组代替字典，Godot可以更好地处理数组导出
        [Export]
        public UIDefinition[] Definitions { get; set; } = Array.Empty<UIDefinition>();
        
        // 缓存字典，用于快速查找
        private Dictionary<string, UIDefinition> _elementsCache;
        private Dictionary<UIType, UIDefinition> _elementsTypeCache;
        
        public void _Ready()
        {
            RebuildCache();
        }
        
        private void RebuildCache()
        {
            _elementsCache = new Dictionary<string, UIDefinition>();
            _elementsTypeCache = new Dictionary<UIType, UIDefinition>();
            
            if (Definitions == null)
                return;
                
            foreach (var def in Definitions)
            {
                if (def == null)
                    continue;
                    
                var type = def.GetUIType();
                var typeName = Enum.GetName(typeof(UIType), type);
                
                if (!string.IsNullOrEmpty(typeName))
                {
                    _elementsCache[typeName] = def;
                    _elementsTypeCache[type] = def;
                }
            }
        }
        
        public UIDefinition GetDefinition(UIType type)
        {
            if (_elementsCache == null)
                RebuildCache();
                
            if (_elementsTypeCache.TryGetValue(type, out var def))
                return def;
                
            string typeName = Enum.GetName(typeof(UIType), type);
            GD.PushWarning($"UI definition not found for type: {typeName}");
            return null;
        }
        
        public UIDefinition GetDefinition(string name)
        {
            if (_elementsCache == null)
                RebuildCache();
                
            if (_elementsCache.TryGetValue(name, out var def))
                return def;
                
            GD.PushWarning($"UI definition not found for name: {name}");
            return null;
        }
    }
}