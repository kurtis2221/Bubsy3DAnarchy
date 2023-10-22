using Godot;
using System;

public partial class DeathZone : Area3D
{
	public override void _Ready()
	{
        BodyEntered += DeathZone_BodyEntered;
	}

    private void DeathZone_BodyEntered(Node3D body)
    {
        if (body != PlayerControl.inst) return;
        SetBlockSignals(true);
        PlayerControl.inst.Damage();
        SetBlockSignals(false);
    }
}
