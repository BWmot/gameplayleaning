using Godot;
using System;
using System.Collections.Generic;

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