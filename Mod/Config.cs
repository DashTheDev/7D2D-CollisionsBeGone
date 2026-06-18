namespace CollisionsBeGone;

public class CollisionsBeGoneConfig
{
    public bool IsEnabled { get; set; } = true;

#if DEBUG
    public bool IsDebug { get; set; } = true;
#else
    public bool IsDebug { get; set; }
#endif

    public bool DebugTranspilers { get; set; }
    public bool DebugColliders { get; set; }
    public bool DisablePlayerCollisions { get; set; } = true;
    public bool DisablePlayerVehicleCollisions { get; set; } = true;
    public bool PreventVehicleExitOnOverlap { get; set; } = true;
}