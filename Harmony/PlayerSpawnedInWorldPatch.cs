using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace CollisionsBeGone;

[HarmonyPatch(typeof(GameManager), nameof(GameManager.PlayerSpawnedInWorld))]
public class PlayerSpawnedInWorldPatch
{
    private static void Postfix(GameManager __instance, ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos, int _entityId)
    {
        Utility.LogLine($"GameManager.PlayerSpawnedInWorld {{ Type: {_respawnReason} }}");

        if (!CollisionsBeGoneMod.Config.DisablePlayerCollisions)
        {
            return;
        }

        EntityPlayer respawnedPlayer = __instance.World.GetEntity(_entityId) as EntityPlayer;

        if (respawnedPlayer == null)
        {
            return;
        }

        foreach (EntityPlayer foundPlayer in __instance.World.Players.list)
        {
            if (respawnedPlayer.entityId == foundPlayer.entityId)
            {
                continue;
            }

            if (foundPlayer.IsDead())
            {
                continue;
            }

            Utility.IgnoreCollisionsBetweenPlayers(respawnedPlayer, foundPlayer);
            Utility.ResetPlayerPhysicsBodyCollisionLayersToDefault(respawnedPlayer);
            Utility.LogLine($"Disabled player collisions between {respawnedPlayer.PlayerDisplayName} and {foundPlayer.PlayerDisplayName}");
        }
    }
}