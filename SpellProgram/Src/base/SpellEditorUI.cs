using Godot;
using System;

public partial class SpellEditorUI : CanvasLayer
{
    private SpellCaster _caster;
    private Wand _wand;
    private SpellDebugUI _debugUI;

    public override void _Ready()
    {
       
    }

    public void SetTargets(Wand wand, SpellCaster caster)
    {
        _wand = wand;
        _caster = caster;
        _debugUI.SetTargets(wand, caster);
    }
}
