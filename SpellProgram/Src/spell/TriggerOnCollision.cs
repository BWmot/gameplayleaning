using Godot;
using System;
using System.Collections.Generic;

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