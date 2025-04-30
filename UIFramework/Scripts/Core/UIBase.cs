using Godot;
using System;
using System.Threading.Tasks;
using UIFramework.Enums;

public partial class UIBase : Control
{
    [Signal]
    public delegate void OnClosedEventHandler();
    
    public new bool IsVisible { get; protected set; }
    
    public virtual UIFramework.Enums.UILayer Layer { get; protected set; } = UIFramework.Enums.UILayer.Normal;
    
    private bool _isInitialized = false;

    public override void _Ready()
    {
        Visible = false;
        IsVisible = false;
        AdaptToScreenSize();
    }

    public virtual void Initialize()
    {
        if (!_isInitialized)
        {
            IsVisible = false;
            _isInitialized = true;
        }
    }

    public new virtual void Show()
    {
        if (!_isInitialized)
        {
            Initialize();
        }
        
        IsVisible = true;
        Visible = true;
    }

    public new virtual void Hide()
    {
        IsVisible = false;
        Visible = false;
    }
    
    public virtual async Task ShowWithAnimation()
    {
        if (!UIConfig.UseAnimations)
        {
            Show();
            return;
        }
        
        // 实现显示动画
        Show();
        // 例如淡入效果
        Modulate = new Color(1, 1, 1, 0);
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 1.0f, UIConfig.DefaultFadeInTime);
        await ToSignal(tween, "finished");
    }

    public virtual async Task HideWithAnimation()
    {
        if (!UIConfig.UseAnimations)
        {
            Hide();
            return;
        }
        
        // 实现隐藏动画
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0.0f, UIConfig.DefaultFadeOutTime);
        await ToSignal(tween, "finished");
        Hide();
        EmitSignal(SignalName.OnClosed);
    }
    
    public virtual void AdaptToScreenSize()
    {
        // 根据当前屏幕尺寸进行调整
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        
        // 这里可以添加自定义的屏幕适配逻辑
        // 例如，确保UI元素在不同分辨率下正确显示
        // 可以使用Godot的锚点和边距来实现
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationResized)
        {
            AdaptToScreenSize();
        }
    }
    
    public virtual void SetData(object data)
    {
        // 可以被子类重写以接收数据
    }
    
    public virtual void Close()
    {
        _ = HideWithAnimation();
    }
    
    // 辅助方法，用于等待信号
    protected async Task ToSignal(Godot.Tween tween, string signalName)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        
        tween.Connect(signalName, Callable.From(() => {
            taskCompletionSource.SetResult(true);
        }));
        
        await taskCompletionSource.Task;
    }
}