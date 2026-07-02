using DashTheDev.SDTD.ModCore;

namespace CollisionsBeGone;

public class CollisionsBeGoneConfig : XmlModConfig
{
    public bool DebugColliders { get; set; }
    public bool DisablePlayerCollisions { get; set; } = true;
    public bool DisablePlayerVehicleCollisions { get; set; } = true;
    public bool DisablePlayerProjectileCollisions { get; set; } = true;
    public bool PreventVehicleExitOnOverlap { get; set; } = true;
}