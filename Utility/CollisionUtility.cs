using UnityEngine;

namespace CollisionsBeGone;

public class CollisionUtility
{
    public const int DefaultCollisionLayer = 0;
    public const int PhysicsCollisionLayer = 21;

    // For some reason, remote players spawn in with physicsBody collision set to layer 0 on join but layer 21 after death.
    // Layer 21 breaks the ignore collision. So this method resets the physicsBody back to what it was when they joined the game.
    public static void ResetPlayerPhysicsBodyCollisionLayersToDefault(EntityPlayer player)
    {
        Utils.SetLayerRecursively(player.gameObject, DefaultCollisionLayer);
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
        GeneralUtility.LogLine($"Disabled player collisions between {playerA.PlayerDisplayName} and {playerB.PlayerDisplayName}");
    }

    public static void IgnoreCollisionsBetweenPlayerAndAllOtherPlayers(EntityPlayer player)
    {
        foreach (EntityPlayer otherPlayer in GameManager.Instance.World.Players.list)
        {
            if (player.entityId == otherPlayer.entityId)
            {
                continue;
            }

            if (otherPlayer.IsDead())
            {
                continue;
            }

            IgnoreCollisionsBetweenPlayers(player, otherPlayer);
        }
    }
}