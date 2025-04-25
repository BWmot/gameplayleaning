using Godot;
using System;

/// <summary>
/// 这是一个2dTopDown视角的玩家控制器
/// </summary>
public partial class player2Dcontroller : Node2D
{
    [Export]
    public float speed = 200f; // 移动速度

    private Vector2 velocity = new Vector2(); // 速度向量

    public override void _Process(double delta)
    {
        // 获取输入
        velocity.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        velocity.Y = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");

        // 归一化速度向量
        if (velocity.Length() > 1)
        {
            velocity = velocity.Normalized();
        }

        // 移动角色
        Position += velocity * speed * (float)delta;
    }


}
