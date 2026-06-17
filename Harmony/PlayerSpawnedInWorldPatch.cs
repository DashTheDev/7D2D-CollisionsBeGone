using HarmonyLib;

namespace CollisionsBeGone;

[HarmonyPatch(typeof(GameManager), nameof(GameManager.PlayerSpawnedInWorld))]
public class PlayerSpawnedInWorldPatch
{
    private static void Postfix(GameManager __instance, ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos, int _entityId)
    {
        GeneralUtility.LogLine($"GameManager.PlayerSpawnedInWorld {{ Type: {_respawnReason} }}");

        if (!CollisionsBeGoneMod.Config.DisablePlayerCollisions)
        {
            return;
        }

        EntityPlayer respawnedPlayer = __instance.World.GetEntity(_entityId) as EntityPlayer;

        if (respawnedPlayer == null)
        {
            return;
        }

        if (respawnedPlayer.isEntityRemote)
        {
            CollisionUtility.ResetPlayerPhysicsBodyCollisionLayersToDefault(respawnedPlayer);
        }

        CollisionUtility.IgnoreCollisionsBetweenPlayerAndAllOtherPlayers(respawnedPlayer);
    }
}