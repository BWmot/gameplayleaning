using Godot;
using System;
using System.Collections.Generic;

// 基础投射物法术
public partial class ProjectileSpell : Spell
{
    [Export] public PackedScene ProjectileScene { get; set; }
    
    public ProjectileSpell()
    {
        Type = SpellType.Projectile;
        Name = "魔法飞弹";
        Description = "发射一枚魔法飞弹";
        ManaCost = 10;
        
        // 设置基础属性
        Properties["damage"] = 10;
        Properties["speed"] = 300;
        Properties["lifetime"] = 3;
    }
    
    public override void Cast(SpellCaster caster, List<Spell> spellStack)
    {
        // 应用所有修饰符
        Dictionary<string, float> finalProperties = new Dictionary<string, float>(Properties);
        
        foreach (var spell in spellStack)
        {
            if (spell.Type == SpellType.Modifier)
            {
                spell.ModifySpell(this);
            }
        }
        
        // 生成投射物
        caster.SpawnProjectile(ProjectileScene, caster.CastDirection, finalProperties);
    }
}

// 修饰符法术示例
public partial class DamageModifier : Spell
{
    [Export] public float DamageMultiplier { get; set; } = 1.5f;
    
    public DamageModifier()
    {
        Type = SpellType.Modifier;
        Name = "增伤";
        Description = "增加法术伤害";
        ManaCost = 5;
    }
    
    public override void Cast(SpellCaster caster, List<Spell> spellStack)
    {
        // 修饰符法术不直接施放
    }
    
    public override void ModifySpell(Spell target)
    {
        // 修改目标法术的伤害属性
        if (target.Properties.ContainsKey("damage"))
        {
            target.Properties["damage"] *= DamageMultiplier;
        }
    }
}

// 触发型法术示例
public partial class TriggerOnCollision : Spell
{
    [Export] public PackedScene TriggerEffect { get; set; }
    
    public TriggerOnCollision()
    {
        Type = SpellType.Trigger;
        Name = "碰撞触发";
        Description = "法术碰撞时触发效果";
        ManaCost = 15;
    }
    
    public override void Cast(SpellCaster caster, List<Spell> spellStack)
    {
        // 创建带有触发器的投射物
        // 实际实现需要更复杂的逻辑来保存后续法术并在触发时释放
    }
}