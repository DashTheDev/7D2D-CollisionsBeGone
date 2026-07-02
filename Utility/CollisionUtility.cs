using UnityEngine;

namespace CollisionsBeGone;

public class CollisionUtility
{
    #region Constants

    private const int DefaultProjectileLayerMask = -538750997;
    private const int DefaultBulletHitMask = 8;
    private const int DefaultProjectileHitMask = 80;
    private const int PhysicsCollisionLayer = 21;
    private const int LocalPlayerCollisionLayer = 24;

    /// <summary>
    /// Usually 21 - Physics but in this mod we set remote players to the same as local player
    /// </summary>
    private const int RemotePlayerCollisionLayer = LocalPlayerCollisionLayer;

    #endregion

    #region Player Collision

    public static void SetPlayerLayerToRemotePlayerLayer(EntityPlayer player)
    {
        Utils.SetLayerRecursively(player.gameObject, RemotePlayerCollisionLayer);
    }

    public static void IgnoreCollisionsBetweenPlayers(EntityPlayer playerA, EntityPlayer playerB)
    {
        CharacterController? playerAController = playerA.PhysicsTransform?.gameObject?.GetComponent<CharacterController>();
        CharacterController? playerBController = playerB.PhysicsTransform?.gameObject?.GetComponent<CharacterController>();

        if (playerAController == null || playerBController == null)
        {
            return;
        }

        Physics.IgnoreCollision(playerAController, playerBController, true);

        if (playerB.isEntityRemote)
        {
            SetPlayerLayerToRemotePlayerLayer(playerB);
        }

        CollisionsBeGoneMod.Instance.Logger.LogLine($"Disabled player collisions between {playerA.PlayerDisplayName} and {playerB.PlayerDisplayName}");
    }

    public static void IgnoreCollisionsBetweenPlayerAndAllOtherPlayers(EntityPlayer player)
    {
        if (player.isEntityRemote)
        {
            SetPlayerLayerToRemotePlayerLayer(player);
        }

        foreach (EntityPlayer otherPlayer in GameManager.Instance.World.Players.list)
        {
            if (player.entityId == otherPlayer.entityId)
            {
                continue;
            }

            if (player.IsDead() || otherPlayer.IsDead())
            {
                continue;
            }

            IgnoreCollisionsBetweenPlayers(player, otherPlayer);
        }
    }

    #endregion

    #region Other Collision

    public static bool IsOnRemotePlayerLayer(GameObject gameObject)
    {
        return gameObject.layer == RemotePlayerCollisionLayer;
    }

    public static int GetVehicleCollisionLayer(EntityVehicle vehicle)
    {
        if (!CollisionsBeGoneMod.Instance.Config.DisablePlayerVehicleCollisions)
        {
            return PhysicsCollisionLayer;
        }

        if (vehicle.AttachedMainEntity == null || !vehicle.AttachedMainEntity.isEntityRemote)
        {
            return PhysicsCollisionLayer;
        }

        return RemotePlayerCollisionLayer;
    }

    public static int GetProjectileLayerMask()
    {
        int layerMask = DefaultProjectileLayerMask;

        if (CollisionsBeGoneMod.Instance.Config.DisablePlayerProjectileCollisions)
        {
            layerMask &= ~(1 << RemotePlayerCollisionLayer);
        }

        return layerMask;
    }

    public static int GetBulletHitMask(int maskOverride)
    {
        return GetProjectileHitMaskInternal(DefaultBulletHitMask, maskOverride);
    }

    public static int GetProjectileHitMask(int maskOverride)
    {
        return GetProjectileHitMaskInternal(DefaultProjectileHitMask, maskOverride);
    }

    private static int GetProjectileHitMaskInternal(int originalHitMask, int maskOverride)
    {
        int newHitMask = maskOverride == 0 ? originalHitMask : maskOverride;

        if (CollisionsBeGoneMod.Instance.Config.DisablePlayerProjectileCollisions)
        {
            newHitMask &= ~(1 << RemotePlayerCollisionLayer);
        }

        return newHitMask;
    }

    #endregion
}