using Godot;
using System;
using System.Collections.Generic;

public enum SpellType
{
    Projectile,     // 投射物（如魔法飞弹）
    Modifier,       // 修饰符（如增加伤害、改变速度）
    Trigger,        // 触发器（如碰撞触发、定时触发）
    Static,         // 静态效果（如光环、护盾）
    Multicast,      // 多重施法（如二重、三重施法）
    Utility         // 功能性（如挖掘、治疗）
}

public abstract partial class Spell : Resource
{
    [Export] public string Name { get; set; } = "未命名法术";
    [Export] public string Description { get; set; } = "";
    [Export] public Texture2D Icon { get; set; }
    [Export] public SpellType Type { get; set; }
    [Export] public float ManaCost { get; set; } = 10f;
    [Export] public float CastDelay { get; set; } = 0.1f;
    [Export] public float Cooldown { get; set; } = 0.5f;
    
    // 法术特性（用于修饰符影响）
    public Dictionary<string, float> Properties { get; set; } = new Dictionary<string, float>();
    
    // 每个法术都需要实现的施法方法
    public abstract void Cast(SpellCaster caster, List<Spell> spellStack);
    
    // 用于修饰符法术影响其他法术
    public virtual void ModifySpell(Spell target) { }
    
    public Spell()
    {
        // 初始化默认属性
        Properties["damage"] = 0;
        Properties["speed"] = 1;
        Properties["lifetime"] = 1;
        Properties["spread"] = 0;
        Properties["size"] = 1;
    }
}