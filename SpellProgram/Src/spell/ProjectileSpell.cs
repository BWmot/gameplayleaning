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

        // 记录到调试UI (如果存在)
        SpellDebugUI debugUI = caster.GetNode<SpellDebugUI>("/root/spellprogram_main_debug/SpellDebugUI");
        if (debugUI != null)
        {
            debugUI.LogSpellCast(this, finalProperties);
        }

    }
}

