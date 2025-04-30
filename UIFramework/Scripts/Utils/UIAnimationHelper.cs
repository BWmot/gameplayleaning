using Godot;
using System;
using System.Threading.Tasks;

public static class UIAnimationHelper
{
    public static void PlayAnimation(Node uiElement, string animationName, Action onComplete = null)
    {
        if (uiElement is AnimatedSprite2D animatedSprite)
        {
            animatedSprite.Play(animationName);
            if (onComplete != null)
            {
                animatedSprite.AnimationFinished += () => onComplete();
            }
        }
        else if (uiElement is AnimationPlayer animationPlayer)
        {
            animationPlayer.Play(animationName);
            if (onComplete != null)
            {
                animationPlayer.AnimationFinished += (_) => onComplete();
            }
        }
        else
        {
            GD.PrintErr("UI element does not support animations.");
        }
    }

    public static void StopAnimation(Node uiElement)
    {
        if (uiElement is AnimatedSprite2D animatedSprite)
        {
            animatedSprite.Stop();
        }
        else if (uiElement is AnimationPlayer animationPlayer)
        {
            animationPlayer.Stop();
        }
        else
        {
            GD.PrintErr("UI element does not support animations.");
        }
    }
    
    // 新增的Tween动画辅助方法
    
    // 淡入动画
    public static async Task FadeIn(Control control, float duration = 0.3f)
    {
        control.Visible = true;
        control.Modulate = new Color(control.Modulate.R, control.Modulate.G, control.Modulate.B, 0);
        
        var tween = control.CreateTween();
        tween.TweenProperty(control, "modulate:a", 1.0f, duration);
        
        await ToSignal(tween, "finished");
    }
    
    // 淡出动画
    public static async Task FadeOut(Control control, float duration = 0.3f)
    {
        var tween = control.CreateTween();
        tween.TweenProperty(control, "modulate:a", 0.0f, duration);
        
        await ToSignal(tween, "finished");
        control.Visible = false;
    }
    
    // 从上方滑入
    public static async Task SlideInFromTop(Control control, float duration = 0.5f)
    {
        float originalY = control.Position.Y;
        control.Position = new Vector2(control.Position.X, -control.Size.Y);
        control.Visible = true;
        
        var tween = control.CreateTween();
        tween.TweenProperty(control, "position:y", originalY, duration).SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);
        
        await ToSignal(tween, "finished");
    }
    
    // 向上滑出
    public static async Task SlideOutToTop(Control control, float duration = 0.5f)
    {
        var tween = control.CreateTween();
        tween.TweenProperty(control, "position:y", -control.Size.Y, duration).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
        
        await ToSignal(tween, "finished");
        control.Visible = false;
    }
    
    // 从下方滑入
    public static async Task SlideInFromBottom(Control control, float duration = 0.5f)
    {
        float viewportHeight = control.GetViewportRect().Size.Y;
        float originalY = control.Position.Y;
        control.Position = new Vector2(control.Position.X, viewportHeight);
        control.Visible = true;
        
        var tween = control.CreateTween();
        tween.TweenProperty(control, "position:y", originalY, duration).SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);
        
        await ToSignal(tween, "finished");
    }
    
    // 向下滑出
    public static async Task SlideOutToBottom(Control control, float duration = 0.5f)
    {
        float viewportHeight = control.GetViewportRect().Size.Y;
        
        var tween = control.CreateTween();
        tween.TweenProperty(control, "position:y", viewportHeight, duration).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
        
        await ToSignal(tween, "finished");
        control.Visible = false;
    }
    
    // 缩放入场
    public static async Task ScaleIn(Control control, float duration = 0.3f)
    {
        control.Scale = new Vector2(0, 0);
        control.Visible = true;
        
        var tween = control.CreateTween();
        tween.TweenProperty(control, "scale", new Vector2(1, 1), duration).SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);
        
        await ToSignal(tween, "finished");
    }
    
    // 缩放出场
    public static async Task ScaleOut(Control control, float duration = 0.3f)
    {
        var tween = control.CreateTween();
        tween.TweenProperty(control, "scale", new Vector2(0, 0), duration).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.In);
        
        await ToSignal(tween, "finished");
        control.Visible = false;
    }
    
    // 闪烁效果
    public static async Task Blink(Control control, int times = 3, float duration = 0.1f)
    {
        var tween = control.CreateTween();
        for (int i = 0; i < times; i++)
        {
            tween.TweenProperty(control, "modulate:a", 0.0f, duration);
            tween.TweenProperty(control, "modulate:a", 1.0f, duration);
        }
        
        await ToSignal(tween, "finished");
    }
    
    // 抖动效果
    public static async Task Shake(Control control, int times = 5, float amount = 5.0f, float duration = 0.05f)
    {
        Vector2 originalPosition = control.Position;
        var tween = control.CreateTween();
        
        for (int i = 0; i < times; i++)
        {
            // 随机方向的偏移
            var randomOffset = new Vector2(
                (float)GD.RandRange(-amount, amount),
                (float)GD.RandRange(-amount, amount)
            );
            
            tween.TweenProperty(control, "position", originalPosition + randomOffset, duration);
        }
        
        // 回到原位
        tween.TweenProperty(control, "position", originalPosition, duration);
        
        await ToSignal(tween, "finished");
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