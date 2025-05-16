using System;
using System.Collections.Generic;
using Godot;

namespace GameplayLearning.EventSystem
{
    public partial class EventBus : Node
    {
        // 单例实例
        private static EventBus _instance;
        
        // 事件处理器字典，键是事件名称，值是对应的处理器列表
        private Dictionary<string, List<Delegate>> _eventHandlers = new Dictionary<string, List<Delegate>>();
        
        // 获取单例实例
        public static EventBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventBus();
                }
                return _instance;
            }
        }
        
        // 私有构造函数，防止外部实例化
        private EventBus()
        {
            Name = "EventBus";
        }
        
        // 订阅无参数事件
        public void Subscribe(string eventName, Action handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = new List<Delegate>();
            }
            
            _eventHandlers[eventName].Add(handler);
        }
        
        // 订阅带参数事件
        public void Subscribe<T>(string eventName, Action<T> handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = new List<Delegate>();
            }
            
            _eventHandlers[eventName].Add(handler);
        }
        
        // 取消订阅无参数事件
        public void Unsubscribe(string eventName, Action handler)
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
        
        // 取消订阅带参数事件
        public void Unsubscribe<T>(string eventName, Action<T> handler)
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
        
        // 发布无参数事件
        public void Publish(string eventName)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                // 创建一个副本以防在迭代过程中有修改操作
                var handlers = new List<Delegate>(_eventHandlers[eventName]);
                
                foreach (var handler in handlers)
                {
                    if (handler is Action action)
                    {
                        action();
                    }
                }
            }
        }
        
        // 发布带参数事件
        public void Publish<T>(string eventName, T eventData)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                // 创建一个副本以防在迭代过程中有修改操作
                var handlers = new List<Delegate>(_eventHandlers[eventName]);
                
                foreach (var handler in handlers)
                {
                    if (handler is Action<T> action)
                    {
                        action(eventData);
                    }
                }
            }
        }
        
        // 清除所有事件订阅
        public void ClearAllSubscriptions()
        {
            _eventHandlers.Clear();
        }
        
        // 清除特定事件的所有订阅
        public void ClearEventSubscriptions(string eventName)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers.Remove(eventName);
            }
        }
    }
}