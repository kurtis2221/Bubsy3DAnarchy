using Godot;
using System;

public partial class CameraControl : Node3D
{
    [Export]
    public Node3D player;

    Vector3 rot_player, rot_camera;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        rot_player = player.RotationDegrees;
        rot_camera = RotationDegrees;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        InputEventMouseMotion motion = @event as InputEventMouseMotion;
        if (motion != null)
        {
            Vector2 vel = motion.Relative;
            rot_player.Y -= vel.X * 0.2f;
            rot_player.Y %= 360f;
            rot_camera.X -= vel.Y * 0.2f;
            rot_camera.X = Mathf.Clamp(rot_camera.X, -90f, 90f);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        player.RotationDegrees = rot_player;
        RotationDegrees = rot_camera;
    }
}
