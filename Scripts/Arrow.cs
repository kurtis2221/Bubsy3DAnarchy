using Godot;
using System;

public partial class Arrow : Node3D
{
    public override void _PhysicsProcess(double delta)
    {
        LookAt(CheckpontHandler.curr_cp_pos);
    }
}
