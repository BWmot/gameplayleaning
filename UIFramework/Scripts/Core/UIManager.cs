using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIFramework.Enums;
using System.Linq;

public partial class UIManager : Node
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;
    
    private Dictionary<UIType, UIBase> _uiElements = new Dictionary<UIType, UIBase>();
    private HashSet<UIType> _tempUIs = new HashSet<UIType>();
    private Dictionary<string, List<Action<object>>> _eventHandlers = new Dictionary<string, List<Action<object>>>();
    private Queue<UIType> _popupQueue = new Queue<UIType>();
    private bool _isShowingPopup = false;
    
    private Node _uiRoot;

    public override void _Ready()
    {
        if (_instance == null)
        {
            _instance = this;
            
            // 创建UI根节点，用于组织所有UI元素
            _uiRoot = new Control();
            _uiRoot.Name = "UIRoot";
            ((Control)_uiRoot).AnchorRight = 1;
            ((Control)_uiRoot).AnchorBottom = 1;
            AddChild(_uiRoot);
            
            // 预加载常用UI
            PreloadCommonUIs();
        }
        else if (_instance != this)
        {
            GD.PushWarning("Multiple UIManager instances detected! Using the first one.");
            QueueFree();
        }
    }
    
    private void PreloadCommonUIs()
    {
        // 这里可以预加载一些常用的UI
        // 例如：PreloadUI(UIType.MainMenu);
    }

    public async Task ShowUI(UIType type, object data = null)
    {
        UIBase ui;
        
        if (!_uiElements.ContainsKey(type))
        {
            ui = UIFactory.CreateUI(type);
            if (ui == null)
            {
                GD.PrintErr($"Failed to create UI of type {type}.");
                return;
            }
            RegisterUI(type, ui);
        }
        else
        {
            ui = _uiElements[type];
        }
        
        if (data != null)
        {
            ui.SetData(data);
        }
        
        await ui.ShowWithAnimation();
    }

    public async Task HideUI(UIType type)
    {
        if (_uiElements.ContainsKey(type))
        {
            await _uiElements[type].HideWithAnimation();
            
            // 如果是临时UI，隐藏后从注册表中移除
            if (_tempUIs.Contains(type))
            {
                UnregisterUI(type);
                _tempUIs.Remove(type);
            }
        }
        else
        {
            GD.Print($"UI of type {type} not found.");
        }
    }

    public void RegisterUI(UIType type, UIBase uiElement)
    {
        if (!_uiElements.ContainsKey(type))
        {
            _uiElements[type] = uiElement;
            
            // 根据层级顺序添加到场景中
            if (uiElement.GetParent() == null)
            {
                _uiRoot.AddChild(uiElement);
                
                // 按层级排序所有UI元素
                SortUIElementsByLayer();
            }
        }
        else
        {
            GD.Print($"UI of type {type} is already registered.");
        }
    }

    public void UnregisterUI(UIType type)
    {
        if (_uiElements.ContainsKey(type))
        {
            if (_uiElements[type] != null && _uiElements[type].IsInsideTree())
            {
                _uiElements[type].QueueFree();
            }
            _uiElements.Remove(type);
        }
        else
        {
            GD.Print($"UI of type {type} not found for unregistration.");
        }
    }
    
    public void PreloadUI(UIType type, bool keepInMemory = true)
    {
        if (!_uiElements.ContainsKey(type))
        {
            var ui = UIFactory.CreateUI(type);
            if (ui != null)
            {
                ui.Visible = false;
                RegisterUI(type, ui);
                
                if (!keepInMemory)
                {
                    _tempUIs.Add(type);
                }
            }
        }
    }
    
    public void CloseAllUIs()
    {
        foreach (var type in _uiElements.Keys.ToList())
        {
            _ = HideUI(type);
        }
    }
    
    private void SortUIElementsByLayer()
    {
        // 获取_uiRoot下的所有直接子节点
        var children = new List<UIBase>();
        foreach (var child in _uiRoot.GetChildren())
        {
            if (child is UIBase uiBase)
            {
                children.Add(uiBase);
            }
        }
        
        // 按层级排序 - 使用数值比较
        children.Sort((a, b) => {
            int aValue = (int)a.Layer;
            int bValue = (int)b.Layer;
            return aValue.CompareTo(bValue);
        });
        
        // 重新添加子节点以应用排序
        foreach (var child in children)
        {
            _uiRoot.MoveChild(child, -1);  // 移到末尾
        }
    }
    
    // 事件系统
    public void RegisterEvent(string eventName, Action<object> handler)
    {
        if (!_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers[eventName] = new List<Action<object>>();
        }
        _eventHandlers[eventName].Add(handler);
    }

    public void UnregisterEvent(string eventName, Action<object> handler)
    {
        if (_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers[eventName].Remove(handler);
            
            if (_eventHandlers[eventName].Count == 0)
            {
                _eventHandlers.Remove(eventName);
            }
        }
    }

    public void TriggerEvent(string eventName, object data = null)
    {
        if (_eventHandlers.ContainsKey(eventName))
        {
            // 创建一个副本以防止在迭代过程中有修改
            var handlers = new List<Action<object>>(_eventHandlers[eventName]);
            
            foreach (var handler in handlers)
            {
                handler.Invoke(data);
            }
        }
    }
    
    // 弹窗管理系统
    public async Task ShowPopup(UIType type, object data = null)
    {
        _popupQueue.Enqueue(type);
        await ProcessPopupQueue(data);
    }

    private async Task ProcessPopupQueue(object data = null)
    {
        if (_isShowingPopup || _popupQueue.Count == 0)
            return;
            
        _isShowingPopup = true;
        var popupType = _popupQueue.Dequeue();
        
        // 显示弹窗
        if (!_uiElements.ContainsKey(popupType))
        {
            var ui = UIFactory.CreateUI(popupType);
            if (ui == null)
            {
                _isShowingPopup = false;
                await ProcessPopupQueue();
                return;
            }
            
            RegisterUI(popupType, ui);
            
            // 连接关闭信号
            ui.OnClosed += () =>
            {
                _isShowingPopup = false;
                _ = ProcessPopupQueue();
            };
        }
        else
        {
            // 如果已经创建，确保连接了关闭信号
            if (!IsSignalConnected(_uiElements[popupType], "OnClosed"))
            {
                _uiElements[popupType].OnClosed += () =>
                {
                    _isShowingPopup = false;
                    _ = ProcessPopupQueue();
                };
            }
        }
        
        if (data != null)
        {
            _uiElements[popupType].SetData(data);
        }
        
        await _uiElements[popupType].ShowWithAnimation();
    }
    
    // 辅助方法：检查信号是否已连接
    private bool IsSignalConnected(UIBase ui, string signalName)
    {
        // 在Godot中检查信号连接的方法
        return ui.IsConnected(signalName, Callable.From(() => {}));
    }
    
    // 屏幕适配
    public override void _Notification(int what)
    {
        if (what == NotificationWMSizeChanged)
        {
            foreach (var ui in _uiElements.Values)
            {
                ui.AdaptToScreenSize();
            }
        }
    }
}