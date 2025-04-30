using Godot;
using System;
using System.Threading.Tasks;
using UIFramework.Enums;

namespace UIFramework.Examples
{
    public partial class MessageBox : UIBase
    {
        private Label _titleLabel;
        private Label _messageLabel;
        private Button _okButton;
        private Button _cancelButton;
        
        // 弹窗回调事件
        public Action<bool> OnResult;
        
        public override UIFramework.Enums.UILayer Layer { get; protected set; } = UIFramework.Enums.UILayer.Popup;
        
        public override void _Ready()
        {
            base._Ready();
            
            // 获取UI控件引用
            _titleLabel = GetNode<Label>("Panel/VBoxContainer/TitleLabel");
            _messageLabel = GetNode<Label>("Panel/VBoxContainer/MessageLabel");
            _okButton = GetNode<Button>("Panel/VBoxContainer/HBoxContainer/OKButton");
            _cancelButton = GetNode<Button>("Panel/VBoxContainer/HBoxContainer/CancelButton");
            
            // 注册按钮事件
            _okButton.Pressed += () => HandleResult(true);
            _cancelButton.Pressed += () => HandleResult(false);
        }
        
        public void SetupMessageBox(string title, string message, bool showCancelButton = true)
        {
            _titleLabel.Text = title;
            _messageLabel.Text = message;
            _cancelButton.Visible = showCancelButton;
        }
        
        private async void HandleResult(bool result)
        {
            // 触发回调
            OnResult?.Invoke(result);
            
            // 关闭弹窗
            await HideWithAnimation();
        }
        
        public override void SetData(object data)
        {
            if (data is MessageBoxData messageBoxData)
            {
                SetupMessageBox(
                    messageBoxData.Title,
                    messageBoxData.Message,
                    messageBoxData.ShowCancelButton
                );
                
                OnResult = messageBoxData.Callback;
            }
        }
    }
    
    // 消息框数据类
    public class MessageBoxData
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool ShowCancelButton { get; set; } = true;
        public Action<bool> Callback { get; set; }
        
        public MessageBoxData(string title, string message, bool showCancelButton = true, Action<bool> callback = null)
        {
            Title = title;
            Message = message;
            ShowCancelButton = showCancelButton;
            Callback = callback;
        }
    }
}