using HarmonyLib;

namespace CollisionsBeGone;

[HarmonyPatch(typeof(Entity), nameof(Entity.SendDetach))]
public class EntitySendDetachPatch
{
    private static bool Prefix(Entity __instance)
    {
        if (!CollisionsBeGoneMod.Instance.Config.PreventVehicleExitOnOverlap)
        {
            return true;
        }

        if (__instance is not EntityPlayerLocal localPlayer)
        {
            return true;
        }

        if (localPlayer.AttachedToEntity is not EntityVehicle vehicle || vehicle.vehicleRB == null)
        {
            return true;
        }

        VehicleOverlapTrigger? vehicleOverlapTrigger = vehicle.vehicleRB.GetComponentInChildren<VehicleOverlapTrigger>();

        if (vehicleOverlapTrigger == null || vehicleOverlapTrigger.CollidersInTriggerCount == 0)
        {
            return true;
        }

        localPlayer.ShowTooltip(LocalisationUtility.GetMoveAwayMessage());
        CollisionsBeGoneMod.Instance.Logger.LogLine("Prevented player from exiting vehicle.");
        return false;
    }
}