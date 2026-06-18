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
        bool patched = false;
        List<CodeInstruction> codes = [.. instructions];

        GeneralUtility.LogTranspilerBefore(nameof(VehicleSetDrivenPatches), codes);

        for (int i = 0; i < codes.Count; i++)
        {
            // Only interested in subset of code (50 instructions in)
            if (i < 50)
            {
                continue;
            }

            if (codes[i].opcode != ReadableOpCodes.LoadSmallInt || Convert.ToInt32(codes[i].operand) != CollisionUtility.PhysicsCollisionLayer)
            {
                continue;
            }

            List<CodeInstruction> replacementInstructions =
            [
                // Prepare arg1: this (entityVehicle)
                new(ReadableOpCodes.LoadArgument0),

                // Call method: VehicleSetDrivenPatch.GetCollisionLayer(vehicle)
                new(ReadableOpCodes.CallMethod, AccessTools.Method(typeof(VehicleSetDrivenPatches), nameof(GetCollisionLayer)))
            ];

            codes.RemoveAt(i);
            codes.InsertRange(i, replacementInstructions);

            patched = true;
            break;
        }

        GeneralUtility.LogLine($"{nameof(VehicleSetDrivenPatches)} Transpiler patch {(patched ? "was" : "was NOT")} applied!");
        GeneralUtility.LogTranspilerAfter(nameof(VehicleSetDrivenPatches), codes);

        return codes;
    }

    private static int GetCollisionLayer(EntityVehicle vehicle)
    {
        if (!CollisionsBeGoneMod.Config.DisablePlayerVehicleCollisions)
        {
            return CollisionUtility.PhysicsCollisionLayer;
        }

        if (vehicle.AttachedMainEntity == null || !vehicle.AttachedMainEntity.isEntityRemote)
        {
            return CollisionUtility.PhysicsCollisionLayer;
        }

        return CollisionUtility.DefaultCollisionLayer;
    }
}