using System.Collections.Generic;
using HarmonyLib;

namespace CollisionsBeGone;

public class GeneralUtility
{
    public static void LogLine(string str)
    {
        if (!CollisionsBeGoneMod.IsDebug)
        {
            return;
        }

        Log.Out($"[{CollisionsBeGoneMod.ModInstance.Name}](v{CollisionsBeGoneMod.ModInstance.VersionString}) {str}");
    }

    public static void LogTranspilerBefore(string methodName, List<CodeInstruction> instructions)
    {
        LogTranspiler(methodName, true, instructions);
    }

    public static void LogTranspilerAfter(string methodName, List<CodeInstruction> instructions)
    {
        LogTranspiler(methodName, false, instructions);
    }

    private static void LogTranspiler(string methodName, bool isBefore, List<CodeInstruction> instructions)
    {
        if (!CollisionsBeGoneMod.IsDebug || !CollisionsBeGoneMod.Config.DebugTranspilers)
        {
            return;
        }

        string timingDescription = isBefore ? "BEFORE" : "AFTER";
        LogLine($"=== {methodName} Transpiler - {timingDescription} ===");

        for (int i = 0; i < instructions.Count; i++)
        {
            LogLine($" [{i}] {instructions[i].opcode} {instructions[i].operand}");
        }
    }
}