using System.Collections.Generic;
using HarmonyLib;

namespace CollisionsBeGone;

/// <summary>
/// Patches things like bullets to ignore player hitboxes if config allows.
/// </summary>
[HarmonyPatch(typeof(ItemActionRanged), nameof(ItemActionRanged.fireShot))]
public class RangedFireShotPatch
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> codes = [.. instructions];
        CollisionsBeGoneMod.Instance.Logger.LogTranspilerBefore(nameof(RangedFireShotPatch), codes);

        // Arg 1 for method (this.hitmaskOverride - arg 0, field) is already loaded, don't need to load any args
        // Call method: CollisionUtility.GetBulletHitMask(maskOverride)
        CodeInstruction getBulletHitMaskInstruction = new(ReadableOpCodes.CallMethod, AccessTools.Method(typeof(CollisionUtility), nameof(CollisionUtility.GetBulletHitMask)));

        // Codes 40 to 44 is this.hitmaskOverride == 0 ? 8 : this.hitmaskOverride;
        codes.RemoveRange(40, 5);
        codes.Insert(40, getBulletHitMaskInstruction);

        // Call method: CollisionUtility.GetProjectileLayerMask()
        CodeInstruction getProjectileLayerMaskInstruction = new(ReadableOpCodes.CallMethod, AccessTools.Method(typeof(CollisionUtility), nameof(CollisionUtility.GetProjectileLayerMask)));

        // Code 101 (AFTER previous removal/insert) is -538750997 (the layer mask)
        codes.RemoveAt(101);
        codes.Insert(101, getProjectileLayerMaskInstruction);

        CollisionsBeGoneMod.Instance.Logger.LogTranspilerAfter(nameof(RangedFireShotPatch), codes);
        return codes;
    }
}