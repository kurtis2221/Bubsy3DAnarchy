using Godot;
using System;

public partial class MineZone : Area3D
{
    [Export]
    public CpuParticles3D emitter;

    [Export]
    public AudioStreamPlayer3D snd;

    public override void _Ready()
    {
        BodyEntered += MineZone_BodyEntered;
    }

    private void MineZone_BodyEntered(Node3D body)
    {
        if (body != PlayerControl.inst || PlayerControl.inst.block) return;
        PlayerControl.inst.Damage();
        Explosion();
    }

    private void Explosion()
    {
        emitter.Emitting = true;
        emitter.Emitting = false;
        snd.Play();
    }
}
