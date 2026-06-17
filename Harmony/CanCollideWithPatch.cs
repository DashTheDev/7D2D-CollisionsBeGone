using HarmonyLib;

namespace CollisionsBeGone;

[HarmonyPatch(typeof(EntityAlive), nameof(EntityAlive.CanCollideWith))]
public class CanCollideWithPatch
{
    private static bool Prefix(EntityAlive __instance, Entity _other, ref bool __result)
    {
        if (!CollisionsBeGoneMod.Config.DisablePlayerCollisions)
        {
            return true;
        }

        if (__instance.IsDead() || __instance is not EntityPlayer)
        {
            return true;
        }

        if (_other.IsDead() || _other is not EntityPlayer)
        {
            return true;
        }

        __result = false;
        return false;
    }
}