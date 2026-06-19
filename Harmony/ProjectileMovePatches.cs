using System.Collections.Generic;
using HarmonyLib;

namespace CollisionsBeGone;

/// <summary>
/// Patches things like arrows to ignore player hitboxes if config allows.
/// </summary>
[HarmonyPatch(typeof(ProjectileMoveScript))]
public class ProjectileMovePatches
{
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ProjectileMoveScript.Fire))]
    public static IEnumerable<CodeInstruction> FireTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = [.. instructions];
        GeneralUtility.LogTranspilerBefore(nameof(ProjectileMovePatches), codes);

        // Arg 1 for method (_hmOverride - arg 4) is already loaded, don't need to load any args
        // Call method: CollisionUtility.GetProjectileHitMask(maskOverride)
        CodeInstruction getProjectileHitMaskInstruction = new(ReadableOpCodes.CallMethod, AccessTools.Method(typeof(CollisionUtility), nameof(CollisionUtility.GetProjectileHitMask)));

        // Codes 27 to 30 is _hmOverride == 0 ? 80 /*0x50*/ : _hmOverride;
        codes.RemoveRange(27, 4);
        codes.Insert(27, getProjectileHitMaskInstruction);

        GeneralUtility.LogTranspilerAfter(nameof(ProjectileMovePatches), codes);
        return codes;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ProjectileMoveScript.checkCollision))]
    public static IEnumerable<CodeInstruction> CheckCollisionTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = [.. instructions];
        GeneralUtility.LogTranspilerBefore(nameof(ProjectileMovePatches), codes);

        // Call method: CollisionUtility.GetProjectileLayerMask()
        CodeInstruction getProjectileLayerMaskInstruction = new(ReadableOpCodes.CallMethod, AccessTools.Method(typeof(CollisionUtility), nameof(CollisionUtility.GetProjectileLayerMask)));

        // Code 101 is -538750997 (the layer mask)
        codes.RemoveAt(101);
        codes.Insert(101, getProjectileLayerMaskInstruction);

        GeneralUtility.LogTranspilerAfter(nameof(ProjectileMovePatches), codes);
        return codes;
    }
}