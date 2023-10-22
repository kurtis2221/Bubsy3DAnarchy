using Godot;
using System;

public partial class ObjRotate : Node3D, IntObj
{
    [Export]
    public float speed;

    [Export]
    public Vector3 axis;

    [Export]
    public bool inactive;

    public override void _Ready()
    {
        SetPhysicsProcess(!inactive);
    }

    public override void _PhysicsProcess(double delta)
    {
        Rotate(axis, speed);
    }

    public void Activate()
    {
        if (!inactive) return;
        inactive = false;
        SetPhysicsProcess(true);
    }
}
