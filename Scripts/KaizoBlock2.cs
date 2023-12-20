using Godot;
using System;

public partial class KaizoBlock2 : Area3D
{
    [Export]
    public Node3D target;

    [Export]
    public MeshInstance3D mesh;

    [Export]
    public Material mat;

    public override void _Ready()
    {
        BodyEntered += KaizoBlock_BodyEntered;
        target.ProcessMode = ProcessModeEnum.Disabled;
    }

    private void KaizoBlock_BodyEntered(Node3D body)
    {
        if (body != PlayerControl.inst || PlayerControl.inst.Velocity.Y < 0 ||
            body.GlobalPosition.Y >= GlobalPosition.Y - 1.2f) return;
        target.ProcessMode = ProcessModeEnum.Inherit;
        mesh.MaterialOverride = mat;
        PlayerControl.inst.PlaySound(PlayerControl.PlayerSndType.Block);
        PlayerControl.inst.Velocity -= new Vector3(0, 16.0f, 0);
        QueueFree();
    }
}
