using Godot;
using System;

public partial class ObjMove : Node3D, IntObj
{
    [Export]
    public float speed;

    [Export]
    public Vector3 target;

    [Export]
    public bool move_type;

    Vector3 pos_start, pos_end;
    bool state;

    [Export]
    public bool inactive;

    [Export]
    public bool drop;

    public override void _Ready()
    {
        SetPhysicsProcess(!inactive);
        pos_start = GlobalPosition;
        pos_end = pos_start + target;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!move_type)
        {
            if (state)
            {
                GlobalPosition = GlobalPosition.MoveToward(pos_start, speed);
                if (GlobalPosition.DistanceSquaredTo(pos_start) < 0.01)
                {
                    state = !state;
                }
            }
            else
            {
                GlobalPosition = GlobalPosition.MoveToward(pos_end, speed);
                if (GlobalPosition.DistanceSquaredTo(pos_end) < 0.01)
                {
                    state = !state;
                }
            }
        }
        else
        {
            GlobalPosition = GlobalPosition.MoveToward(pos_end, speed);
            if (GlobalPosition.DistanceSquaredTo(pos_end) < 0.01)
            {
                GlobalPosition = pos_start;
                if (drop)
                {
                    inactive = true;
                    SetPhysicsProcess(false);
                }
            }
        }
    }

    public void Activate()
    {
        if (!inactive) return;
        inactive = false;
        SetPhysicsProcess(true);
    }
}
