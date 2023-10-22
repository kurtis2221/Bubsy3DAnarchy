using Godot;
using System;

public partial class KaizoBlock : Area3D
{
    [Export]
    public Node3D target;

    public override void _Ready()
    {
        BodyEntered += KaizoBlock_BodyEntered;
        target.ProcessMode = ProcessModeEnum.Disabled;
        target.Visible = false;
    }

    private void KaizoBlock_BodyEntered(Node3D body)
    {
        if (body != PlayerControl.inst || PlayerControl.inst.Velocity.Y < 0 ||
            body.GlobalPosition.Y >= GlobalPosition.Y - 1.2f) return;
        target.ProcessMode = ProcessModeEnum.Inherit;
        target.Visible = true;
        PlayerControl.inst.PlaySound(PlayerControl.PlayerSndType.Block);
        PlayerControl.inst.Velocity -= new Vector3(0, 16.0f, 0);
        QueueFree();
    }
}
