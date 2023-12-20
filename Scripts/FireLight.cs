using Godot;
using System;

public partial class FireLight : OmniLight3D
{
    const float min_light = 1;
    const float max_light = 4;
    const float light_speed = 0.3f;

    bool dir = false;

    public override void _PhysicsProcess(double delta)
    {
        float energy = LightEnergy;
        energy += dir ? light_speed : -light_speed;
        if (dir && energy >= max_light) dir = false;
        else if (!dir && energy <= min_light) dir = true;
        LightEnergy = energy;
    }
}
