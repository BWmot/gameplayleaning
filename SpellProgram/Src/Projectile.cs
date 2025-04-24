using Godot;
using System;
using System.Collections.Generic;

public partial class Projectile : Area2D
{
    public Vector2 Direction { get; set; } = Vector2.Right;
    public float Speed { get; set; } = 300f;
    public float Damage { get; set; } = 10f;
    public float Lifetime { get; set; } = 3f;
    
    private float _timer = 0;
    
    public override void _Ready()
    {
        // 连接碰撞信号
        BodyEntered += OnBodyEntered;
    }
    
    public override void _Process(double delta)
    {
        // 移动投射物
        Position += Direction.Normalized() * Speed * (float)delta;
        
        // 寿命计时
        _timer += (float)delta;
        if (_timer >= Lifetime)
        {
            QueueFree();
        }
    }
    
    // 碰撞处理
    private void OnBodyEntered(Node2D body)
    {
        // 对目标造成伤害
        if (body is IDamageable damageable)
        {
            damageable.TakeDamage(Damage);
        }
        
        // 碰撞后销毁
        QueueFree();
    }
    
    // 应用属性
    public void ApplyProperties(Dictionary<string, float> properties)
    {
        if (properties.ContainsKey("damage"))
            Damage = properties["damage"];
            
        if (properties.ContainsKey("speed"))
            Speed = properties["speed"];
            
        if (properties.ContainsKey("lifetime"))
            Lifetime = properties["lifetime"];
            
        // 应用其他属性...
    }
}

// 伤害接口
public interface IDamageable
{
    void TakeDamage(float amount);
}