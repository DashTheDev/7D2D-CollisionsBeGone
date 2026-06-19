using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace CollisionsBeGone;

[HarmonyPatch(typeof(EntityVehicle), nameof(EntityVehicle.SetVehicleDriven))]
public class VehicleSetDrivenPatches
{
    private static void Postfix(EntityVehicle __instance)
    {
        if (!CollisionsBeGoneMod.Config.PreventVehicleExitOnOverlap)
        {
            return;
        }

        if (__instance.AttachedMainEntity == null)
        {
            VehicleOverlapTrigger.Detach(__instance);
        }
        else if (!__instance.isEntityRemote)
        {
            VehicleOverlapTrigger.Attach(__instance);
        }
    }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = [.. instructions];
        GeneralUtility.LogTranspilerBefore(nameof(VehicleSetDrivenPatches), codes);

        List<CodeInstruction> replacementInstructions =
        [
            // Prepare arg1: this (entityVehicle)
            new(ReadableOpCodes.LoadArgument0),

            // Call method: CollisionUtility.GetVehicleCollisionLayer(vehicle)
            new(ReadableOpCodes.CallMethod, AccessTools.Method(typeof(CollisionUtility), nameof(CollisionUtility.GetVehicleCollisionLayer)))
        ];

        // Code 53 is load int constant 21 (Physics layer)
        codes.RemoveAt(53);
        codes.InsertRange(53, replacementInstructions);

        GeneralUtility.LogTranspilerAfter(nameof(VehicleSetDrivenPatches), codes);
        return codes;
    }
}