using Godot;
using System;
using System.Text;
using System.Collections.Generic;

public partial class SpellDebugUI : CanvasLayer
{
    // 使用导出变量引用场景中的UI组件
    [Export] private Label _wandStatsLabel;
    [Export] private Label _spellQueueLabel;
    [Export] private Label _activeEffectsLabel;

    // 数据更新开关
    [Export] private bool _updateWandStats = true;
    [Export] private bool _updateSpellQueue = true;
    [Export] private bool _updateActiveEffects = true;

    // 引用到需要监视的对象
    private Wand _targetWand;
    private SpellCaster _targetCaster;

    // 上次施放的法术和效果
    private List<string> _lastCastEffects = new List<string>();
    private float _effectDisplayTime = 3.0f; // 效果显示持续时间
    private float _effectTimer = 0;

    // 不再需要在代码中创建UI
    public override void _Ready()
    {
        // 如果在编辑器中未指定引用，可以尝试通过路径查找
        if (_wandStatsLabel == null)
            _wandStatsLabel = GetNode<Label>("Control/WandStatsLabel");

        if (_spellQueueLabel == null)
            _spellQueueLabel = GetNode<Label>("Control/SpellQueueLabel");

        if (_activeEffectsLabel == null)
            _activeEffectsLabel = GetNode<Label>("Control/ActiveEffectsLabel");

        // 如果上面的方式找不到，可能是因为场景结构不同，尝试另一种路径
        if (_wandStatsLabel == null)
            _wandStatsLabel = GetNode<Label>("%WandStatsLabel");

        if (_spellQueueLabel == null)
            _spellQueueLabel = GetNode<Label>("%SpellQueueLabel");

        if (_activeEffectsLabel == null)
            _activeEffectsLabel = GetNode<Label>("%ActiveEffectsLabel");

        // 检查是否成功找到节点
        if (_wandStatsLabel == null || _spellQueueLabel == null || _activeEffectsLabel == null)
        {
            GD.PrintErr("SpellDebugUI: 无法找到UI标签节点，请确保节点路径正确或在编辑器中分配引用");
            _updateWandStats = false;
            _updateSpellQueue = false;
            _updateActiveEffects = false;
        }
    }

    public override void _Process(double delta)
    {
        if (_targetWand != null && _targetCaster != null)
        {
            // 更新法杖状态和法术队列
            if (_updateWandStats)
                UpdateWandStats();

            if (_updateSpellQueue)
                UpdateSpellQueue();
        }

        // 更新效果显示计时器
        if (_lastCastEffects.Count > 0 && _updateActiveEffects)
        {
            _effectTimer -= (float)delta;
            if (_effectTimer <= 0)
            {
                _lastCastEffects.Clear();
                UpdateActiveEffects();
            }
        }
    }

    // 设置要监视的法杖和施法者
    public void SetTargets(Wand wand, SpellCaster caster)
    {
        _targetWand = wand;
        _targetCaster = caster;
    }

    // 更新法杖基础数据显示
    private void UpdateWandStats()
    {
        var sb = new StringBuilder();
        sb.AppendLine("【法杖状态】");
        sb.AppendLine($"当前魔法值: {_targetWand.CurrentMana:F1} / {_targetWand.ManaMax:F1}");
        sb.AppendLine($"魔法恢复速率: {_targetWand.ManaRechargeRate:F1}/秒");
        sb.AppendLine($"施法延迟: {_targetWand.CastDelay:F2}秒");
        sb.AppendLine($"充能时间: {_targetWand.RechargeTime:F2}秒");
        sb.AppendLine($"法术栏位: {_targetWand.Spells.Count} / {_targetWand.SpellSlots}");
        sb.AppendLine($"随机施放: {(_targetWand.IsShuffled ? "是" : "否")}");

        if (_wandStatsLabel != null)
        {
            _wandStatsLabel.Text = sb.ToString();
        }
        else
        {
            GD.PrintErr("WandStatsLabel 未找到，无法更新法杖状态");
        }
    }

    // 更新法术队列显示
    private void UpdateSpellQueue()
    {
        var sb = new StringBuilder();
        sb.AppendLine("【法术队列】");

        if (_targetWand.Spells.Count == 0)
        {
            sb.AppendLine("没有法术");
        }
        else
        {
            for (int i = 0; i < _targetWand.Spells.Count; i++)
            {
                var spell = _targetWand.Spells[i];
                sb.AppendLine($"{i + 1}. {spell.Name} [{spell.Type}] - {spell.ManaCost:F1}魔法");
            }
        }

        if (_targetWand.AlwaysCastSpells.Count > 0)
        {
            sb.AppendLine("\n【固定施放】");
            for (int i = 0; i < _targetWand.AlwaysCastSpells.Count; i++)
            {
                var spell = _targetWand.AlwaysCastSpells[i];
                sb.AppendLine($"{i + 1}. {spell.Name}");
            }
        }

        if (_spellQueueLabel != null)
        {
            _spellQueueLabel.Text = sb.ToString();
        }
        else
        {
            GD.PrintErr("SpellQueueLabel 未找到，无法更新法术队列");
        }
    }

    // 记录法术效果
    public void LogSpellCast(Spell spell, Dictionary<string, float> properties)
    {
        _lastCastEffects.Clear();
        _lastCastEffects.Add($"施放法术: {spell.Name}");

        foreach (var prop in properties)
        {
            _lastCastEffects.Add($"{prop.Key}: {prop.Value:F2}");
        }

        _effectTimer = _effectDisplayTime;
        UpdateActiveEffects();
    }

    // 更新当前效果显示
    private void UpdateActiveEffects()
    {
        var sb = new StringBuilder();
        sb.AppendLine("【当前法术效果】");

        if (_lastCastEffects.Count == 0)
        {
            sb.AppendLine("无活跃效果");
        }
        else
        {
            foreach (var effect in _lastCastEffects)
            {
                sb.AppendLine(effect);
            }
        }

        if (_activeEffectsLabel != null)
        {
            _activeEffectsLabel.Text = sb.ToString();
        }
        else
        {
            GD.PrintErr("ActiveEffectsLabel 未找到，无法更新当前法术效果");
        }
    }
}