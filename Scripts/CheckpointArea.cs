using Godot;
using System;

public partial class CheckpointArea : Area3D
{
    [Export]
    public bool savepos;

    [Export]
    public bool first;

    [Export]
    public Node3D trigger;

    public override void _Ready()
    {
        if (!first)
        {
            ProcessMode = ProcessModeEnum.Disabled;
            Visible = false;
        }
        BodyEntered += CheckpointArea_BodyEntered;
    }

    private void CheckpointArea_BodyEntered(Node3D body)
    {
        PlayerControl tmp = PlayerControl.inst;
        if (body != tmp) return;
        SetBlockSignals(true);
        if (savepos) tmp.last_cp = GlobalPosition;
        CheckpontHandler.inst.LoadNext();
        if (trigger != null) ((IntObj)trigger).Activate();
        QueueFree();
    }
}
