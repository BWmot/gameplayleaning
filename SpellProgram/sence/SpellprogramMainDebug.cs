using Godot;
using System;

public partial class SpellprogramMainDebug : Node2D
{
    [Export] public PackedScene WandScene;
    [Export] public PackedScene ProjectileScene;
    
    private SpellCaster _player;
    private Wand _testWand;
    private SpellDebugUI _debugUI;
    
    public override void _Ready()
    {
        // 创建玩家施法者
        _player = new SpellCaster();
        AddChild(_player);

        // 创建测试法杖
        _testWand = new Wand()
        {
            ManaMax = 200,
            ManaRechargeRate = 20,
            SpellSlots = 8,
            IsShuffled = false
        };
        AddChild(_testWand);

        // 设置玩家的法杖
        _player.CurrentWand = _testWand;

        // 创建一些法术并添加到法杖
        var projectileSpell = new ProjectileSpell()
        {
            ProjectileScene = ProjectileScene
        };

        var damageModifier = new DamageModifier();

        // 添加法术到法杖
        _testWand.AddSpell(damageModifier); // 先添加修饰符
        _testWand.AddSpell(projectileSpell); // 再添加投射物

        // 创建调试UI
        _debugUI = new SpellDebugUI();
        _debugUI.Name = "DebugUI";
        AddChild(_debugUI);
        _debugUI.SetTargets(_testWand, _player);

        // 输出提示
        GD.Print("法术系统测试场景已加载。按空格键释放法术。");
    }
    
    public override void _Process(double delta)
    {
        // 更新施法方向为鼠标指向
        if (_player != null)
        {
            Vector2 mousePos = GetGlobalMousePosition();
            _player.CastDirection = (mousePos - _player.GlobalPosition).Normalized();
        }
        
        // 空格键施法
        if (Input.IsActionJustPressed("ui_select")) // 空格键
        {
            _player?.CastSpell();
            GD.Print("尝试施放法术");
        }
    }
}