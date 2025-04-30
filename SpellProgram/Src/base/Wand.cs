using Godot;
using System;
using System.Collections.Generic;

public partial class Wand : Node2D
{
    [Export] public float ManaMax { get; set; } = 100f;
    [Export] public float ManaRechargeRate { get; set; } = 10f;
    [Export] public float CastDelay { get; set; } = 0.1f;
    [Export] public float RechargeTime { get; set; } = 0.5f;
    [Export] public int SpellSlots { get; set; } = 6;
    [Export] public bool IsShuffled { get; set; } = false; // 是否随机施放法术
    [Export] public bool AlwaysCast { get; set; } = false; // 是否有固定施放的法术
    
    public float CurrentMana { get; private set; }
    public List<Spell> Spells { get; private set; } = new List<Spell>();
    public List<Spell> AlwaysCastSpells { get; private set; } = new List<Spell>();
    
    private bool _isCasting = false;
    private float _castTimer = 0;
    private float _rechargeTimer = 0;
    private int _currentSpellIndex = 0;
    
    // 初始化法杖
    public override void _Ready()
    {
        CurrentMana = ManaMax;
    }
    
    // 游戏循环
    public override void _Process(double delta)
    {
        // 恢复魔法值
        CurrentMana = Mathf.Min(CurrentMana + ManaRechargeRate * (float)delta, ManaMax);
        
        // 处理施法和冷却计时器
        if (_castTimer > 0)
        {
            _castTimer -= (float)delta;
        }
        
        if (_rechargeTimer > 0)
        {
            _rechargeTimer -= (float)delta;
        }
    }
    
    // 尝试施放法术
    public void TryCast(SpellCaster caster)
    {
        if (_isCasting || _rechargeTimer > 0 || Spells.Count == 0)
            return;
            
        _isCasting = true;
        
        // 法杖循环的核心逻辑
        CastSequence(caster);
    }
    
    private void CastSequence(SpellCaster caster)
    {
        // 创建一个法术栈用于处理法术间的相互作用
        List<Spell> spellStack = new List<Spell>();
        List<Spell> spellsToProcess = new List<Spell>();
        
        // 添加总是施放的法术
        spellsToProcess.AddRange(AlwaysCastSpells);
        
        // 如果法杖是随机施放的，则随机打乱法术顺序
        if (IsShuffled)
        {
            spellsToProcess.AddRange(Spells);
            Shuffle(spellsToProcess);
        }
        else
        {
            // 从当前索引开始，按顺序添加法术
            for (int i = 0; i < Spells.Count; i++)
            {
                int index = (_currentSpellIndex + i) % Spells.Count;
                spellsToProcess.Add(Spells[index]);
            }
            
            // 更新下一次施法的起始索引
            _currentSpellIndex = (_currentSpellIndex + 1) % Spells.Count;
        }
        
        // 处理法术栈
        for (int i = 0; i < spellsToProcess.Count; i++)
        {
            Spell spell = spellsToProcess[i];
            
            // 检查魔法值是否足够
            if (CurrentMana < spell.ManaCost)
                continue;
                
            // 消耗魔法值
            CurrentMana -= spell.ManaCost;
            
            // 根据法术类型处理
            switch (spell.Type)
            {
                case SpellType.Projectile:
                case SpellType.Static:
                case SpellType.Utility:
                    // 直接施放的法术
                    spell.Cast(caster, spellStack);
                    break;
                    
                case SpellType.Modifier:
                    // 修饰符法术影响之后的法术
                    spellStack.Add(spell);
                    break;
                    
                case SpellType.Trigger:
                    // 触发器需要特殊处理，存储下来等待触发条件
                    spellStack.Add(spell);
                    break;
                    
                case SpellType.Multicast:
                    // 多重施法，处理后面的多个法术
                    // 这里简化处理，实际上需要更复杂的逻辑
                    spellStack.Add(spell);
                    break;
            }
        }
        
        // 施法结束，设置冷却时间
        _castTimer = CastDelay;
        _rechargeTimer = RechargeTime;
        _isCasting = false;
    }
    
    // 打乱法术顺序的辅助方法
    private void Shuffle<T>(List<T> list)
    {
        Random random = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    
    // 添加法术到法杖
    public bool AddSpell(Spell spell)
    {
        if (Spells.Count >= SpellSlots)
            return false;
            
        Spells.Add(spell);
        return true;
    }
    
    // 移除法术
    public bool RemoveSpell(int index)
    {
        if (index < 0 || index >= Spells.Count)
            return false;
            
        Spells.RemoveAt(index);
        return true;
    }
}