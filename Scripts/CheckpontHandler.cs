using Godot;
using System;
using System.Collections.Generic;

public partial class CheckpontHandler : Node3D
{
    public static CheckpontHandler inst;

    static List<Node3D> checkpoints;
    static int curr_idx;

    public static Vector3 curr_cp_pos;

    public override void _Ready()
    {
        inst = this;
        curr_idx = 0;
        checkpoints = new List<Node3D>();
        foreach (Node n in GetChildren())
        {
            checkpoints.Add((Node3D)n);
        }
        curr_cp_pos = checkpoints[curr_idx].GlobalPosition;
    }

    public void LoadNext()
    {
        curr_idx++;
        PlayerControl tmp = PlayerControl.inst;
        if (curr_idx < checkpoints.Count)
        {
            tmp.PlaySound(PlayerControl.PlayerSndType.Check);
            Node3D next = checkpoints[curr_idx];
            curr_cp_pos = checkpoints[curr_idx].GlobalPosition;
            next.ProcessMode = ProcessModeEnum.Inherit;
            next.Visible = true;
        }
        else
        {
            tmp.EndGame();
        }
    }
}
