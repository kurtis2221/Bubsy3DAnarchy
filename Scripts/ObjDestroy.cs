using Godot;
using System;

public partial class ObjDestroy : Node3D, IntObj
{
    [Export]
    public Node3D[] obj_list;

    public void Activate()
    {
        foreach (Node3D obj in obj_list)
        {
            obj.QueueFree();
        }
        QueueFree();
    }
}
