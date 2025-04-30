using Godot;
using System;
using System.Threading.Tasks;
using UIFramework.Enums;

public static class UIExtensions
{
    // 淡入扩展方法
    public static async Task FadeIn(this Control control, float duration = 0.3f)
    {
        control.Visible = true;
        control.Modulate = new Color(control.Modulate.R, control.Modulate.G, control.Modulate.B, 0);
        
        var tween = control.CreateTween();
        tween.TweenProperty(control, "modulate:a", 1.0f, duration);
        
        await ToSignal(tween, "finished");
    }

    // 淡出扩展方法
    public static async Task FadeOut(this Control control, float duration = 0.3f)
    {
        var tween = control.CreateTween();
        tween.TweenProperty(control, "modulate:a", 0.0f, duration);
        
        await ToSignal(tween, "finished");
        control.Visible = false;
    }
    
    // 设置UI元素的层级
    public static T SetLayer<T>(this T control, UILayer layer) where T : Control
    {
        if (control is UIBase uiBase)
        {
            // 修改层级属性
            var fieldInfo = uiBase.GetType().GetField("Layer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(uiBase, layer);
            }
        }
        
        // 如果UIManager已经实例化，尝试更新层级排序
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CallDeferred("SortUIElementsByLayer");
        }
        
        return control;
    }
    
    // 居中显示
    public static T CenterInParent<T>(this T control) where T : Control
    {
        if (control.GetParent() is Control parent)
        {
            control.AnchorLeft = 0.5f;
            control.AnchorTop = 0.5f;
            control.AnchorRight = 0.5f;
            control.AnchorBottom = 0.5f;
            
            control.Position = new Vector2(
                -control.Size.X / 2,
                -control.Size.Y / 2
            );
        }
        
        return control;
    }
    
    // 使UI元素填满父容器
    public static T FillParent<T>(this T control) where T : Control
    {
        control.AnchorRight = 1;
        control.AnchorBottom = 1;
        control.OffsetRight = 0;
        control.OffsetBottom = 0;
        control.OffsetLeft = 0;
        control.OffsetTop = 0;
        
        return control;
    }
    
    // 添加到UI管理器
    public static T AddToUIManager<T>(this T control, UIType type) where T : UIBase
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterUI(type, control);
        }
        else
        {
            GD.PushWarning("UIManager instance not found. UI element not registered.");
        }
        
        return control;
    }
    
    // 监听UI事件
    public static T ListenToEvent<T>(this T control, string eventName, Action<object> handler) where T : Node
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterEvent(eventName, handler);
        }
        else
        {
            GD.PushWarning("UIManager instance not found. Event handler not registered.");
        }
        
        return control;
    }
    
    // 停止监听UI事件
    public static T StopListeningToEvent<T>(this T control, string eventName, Action<object> handler) where T : Node
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterEvent(eventName, handler);
        }
        
        return control;
    }
    
    // 触发UI事件
    public static void TriggerUIEvent(this Node node, string eventName, object data = null)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.TriggerEvent(eventName, data);
        }
        else
        {
            GD.PushWarning("UIManager instance not found. Cannot trigger event.");
        }
    }
    
    // 辅助方法，用于等待信号
    private static async Task ToSignal(Godot.Tween tween, string signalName)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        
        tween.Connect(signalName, Callable.From(() => {
            taskCompletionSource.SetResult(true);
        }));
        
        await taskCompletionSource.Task;
    }
}