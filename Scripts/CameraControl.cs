using Godot;
using System;

public partial class CameraControl : Node3D
{
    [Export]
    public Node3D player;

    public static float mouse_sens = 0.2f;

    Vector3 rot_player, rot_camera;

    public override void _Ready()
    {
        rot_player = player.RotationDegrees;
        rot_camera = RotationDegrees;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (MenuControl.pause) return;
        InputEventMouseMotion motion = @event as InputEventMouseMotion;
        if (motion != null)
        {
            Vector2 vel = motion.Relative;
            rot_player.Y -= vel.X * mouse_sens;
            rot_player.Y %= 360f;
            rot_camera.X -= vel.Y * mouse_sens;
            rot_camera.X = Mathf.Clamp(rot_camera.X, -90f, 90f);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        player.RotationDegrees = rot_player;
        RotationDegrees = rot_camera;
    }
}
