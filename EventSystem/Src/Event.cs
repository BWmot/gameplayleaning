using System;

namespace GameplayLearning.EventSystem
{
    // 事件基类
    public class Event
    {
        // 事件名称，用于标识事件类型
        public string Name { get; private set; }
        
        // 事件创建时间
        public DateTime CreatedAt { get; private set; }
        
        public Event(string name)
        {
            Name = name;
            CreatedAt = DateTime.Now;
        }
    }
    
    // 带数据的泛型事件类
    public class Event<T> : Event
    {
        // 事件携带的数据
        public T Data { get; private set; }
        
        public Event(string name, T data) : base(name)
        {
            Data = data;
        }
    }
}