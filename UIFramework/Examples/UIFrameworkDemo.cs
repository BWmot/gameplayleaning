using Godot;
using System;
using System.Threading.Tasks;
using UIFramework.Enums;
using UIFramework.Examples;

public partial class UIFrameworkDemo : Node
{
    private UIManager _uiManager;
    
    public override void _Ready()
    {
        // 初始化UI管理器
        _uiManager = new UIManager();
        AddChild(_uiManager);
        
        // 创建一些按钮来演示UI框架
        CreateDemoButtons();
    }
    
    private void CreateDemoButtons()
    {
        var container = new VBoxContainer();
        container.Name = "DemoButtons";
        container.AnchorRight = 0.3f;
        container.AnchorBottom = 1f;
        AddChild(container);
        
        // 创建标题
        var titleLabel = new Label();
        titleLabel.Text = "UI框架演示";
        titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
        container.AddChild(titleLabel);
        
        // 创建演示按钮
        AddDemoButton(container, "显示消息框", OnShowMessageBox);
        AddDemoButton(container, "显示确认框", OnShowConfirmBox);
        AddDemoButton(container, "注册事件", OnRegisterEvent);
        AddDemoButton(container, "触发事件", OnTriggerEvent);
        AddDemoButton(container, "显示弹窗", OnShowPopup);
        AddDemoButton(container, "关闭所有UI", OnCloseAllUIs);
    }
    
    private void AddDemoButton(Container container, string text, Action onPressed)
    {
        var button = new Button();
        button.Text = text;
        button.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        button.Pressed += onPressed;
        container.AddChild(button);
    }
    
    private async void OnShowMessageBox()
    {
        var messageBoxData = new MessageBoxData(
            "提示", 
            "这是一个简单的消息框示例。\n它使用我们的UI框架创建。",
            false,
            (result) => {
                GD.Print("消息框关闭了，结果: " + result);
            }
        );
        
        // 使用UIManager显示消息框
        await UIManager.Instance.ShowUI(UIType.Notification, messageBoxData);
    }
    
    private async void OnShowConfirmBox()
    {
        var confirmBoxData = new MessageBoxData(
            "确认操作", 
            "您确定要执行此操作吗?\n此操作无法撤销。",
            true,
            (result) => {
                if (result) {
                    GD.Print("用户确认了操作");
                } else {
                    GD.Print("用户取消了操作");
                }
            }
        );
        
        // 使用UIManager显示确认框
        await UIManager.Instance.ShowUI(UIType.DialogueBox, confirmBoxData);
    }
    
    private void OnRegisterEvent()
    {
        // 注册一个UI事件
        UIManager.Instance.RegisterEvent("custom_event", (data) => {
            GD.Print("收到自定义事件，数据: " + data);
            
            // 显示一个通知
            var notificationData = new MessageBoxData(
                "事件通知", 
                "收到事件: " + data,
                false
            );
            _ = UIManager.Instance.ShowUI(UIType.Notification, notificationData); // 使用丢弃操作符
        });
        
        GD.Print("事件已注册，现在可以点击'触发事件'按钮");
    }
    
    private void OnTriggerEvent()
    {
        // 触发一个UI事件
        UIManager.Instance.TriggerEvent("custom_event", "这是来自按钮的事件数据");
    }
    
    private async void OnShowPopup()
    {
        // 使用弹窗队列系统显示多个弹窗
        for (int i = 1; i <= 3; i++)
        {
            int popupIndex = i; // 创建副本以在闭包中使用正确的值
            var popupData = new MessageBoxData(
                $"弹窗 #{popupIndex}", 
                $"这是第 {popupIndex} 个弹窗，会按顺序显示。",
                false,
                (result) => {
                    GD.Print($"弹窗 #{popupIndex} 关闭了");
                }
            );
            
            await UIManager.Instance.ShowPopup(UIType.Notification, popupData);
        }
    }
    
    private void OnCloseAllUIs()
    {
        // 关闭所有打开的UI
        UIManager.Instance.CloseAllUIs();
    }
}