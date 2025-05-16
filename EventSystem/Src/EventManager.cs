using Godot;
using System;

namespace GameplayLearning.EventSystem
{
    public static class EventManager
    {
        // 确保EventBus已经被初始化并添加到场景树中
        public static void Initialize(Node rootNode)
        {
            if (!EventBus.Instance.IsInsideTree())
            {
                rootNode.AddChild(EventBus.Instance);
            }
        }
        
        // 静态方法，简化调用
        public static void Subscribe(string eventName, Action handler)
        {
            EventBus.Instance.Subscribe(eventName, handler);
        }
        
        public static void Subscribe<T>(string eventName, Action<T> handler)
        {
            EventBus.Instance.Subscribe(eventName, handler);
        }
        
        public static void Unsubscribe(string eventName, Action handler)
        {
            EventBus.Instance.Unsubscribe(eventName, handler);
        }
        
        public static void Unsubscribe<T>(string eventName, Action<T> handler)
        {
            EventBus.Instance.Unsubscribe(eventName, handler);
        }
        
        public static void Publish(string eventName)
        {
            EventBus.Instance.Publish(eventName);
        }
        
        public static void Publish<T>(string eventName, T eventData)
        {
            EventBus.Instance.Publish(eventName, eventData);
        }
    }
}