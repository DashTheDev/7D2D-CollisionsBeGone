namespace CollisionsBeGone;

public class CollisionsBeGoneConfig
{
    public bool IsEnabled = true;

#if DEBUG
    public bool IsDebug = true;
#else
    public bool IsDebug;
#endif

    public bool DisablePlayerCollisions = true;
}