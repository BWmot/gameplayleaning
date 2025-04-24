using Godot;
using System;
using System.Collections.Generic;

public partial class SpellCaster : Node2D
{
    [Export] public Wand CurrentWand { get; set; }
    
    // 施放法术的方向
    public Vector2 CastDirection { get; set; } = Vector2.Right;
    
    // 尝试施放当前法杖的法术
    public void CastSpell()
    {
        if (CurrentWand != null)
        {
            CurrentWand.TryCast(this);
        }
    }
    
    // 生成投射物的方法
    public void SpawnProjectile(PackedScene projectileScene, Vector2 direction, Dictionary<string, float> properties)
    {
        if (projectileScene == null)
        {
            GD.PrintErr("错误：ProjectileScene 为空！请确保在编辑器中分配了场景资源。");
            return;
        }
        
        var projectile = projectileScene.Instantiate<Node2D>();
        GetTree().Root.AddChild(projectile);
        projectile.GlobalPosition = GlobalPosition;
        
        // 设置投射物属性
        if (projectile is Projectile p)
        {
            p.Direction = direction;
            p.ApplyProperties(properties);
        }
    }
}