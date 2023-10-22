using Godot;
using System;

public partial class UVOffsetScript : MeshInstance3D
{
    [Export]
    public Vector3 dir;

    StandardMaterial3D mat;

    public override void _Ready()
    {
        mat = (StandardMaterial3D)GetActiveMaterial(0);
    }

    public override void _PhysicsProcess(double delta)
    {
        mat.Uv1Offset += dir;
    }
}
