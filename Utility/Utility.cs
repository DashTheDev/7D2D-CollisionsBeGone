using System.Media;
using UnityEngine;

namespace CollisionsBeGone;

public class Utility
{
    private const int DefaultCollisionLayer = 0;

    public static void LogLine(string str)
    {
        if (!CollisionsBeGoneMod.IsDebug)
        {
            return;
        }

        Log.Out($"[{CollisionsBeGoneMod.ModInstance.Name}](v{CollisionsBeGoneMod.ModInstance.VersionString}) {str}");
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
    }

    // For some reason, remote players spawn in with physicsBody collision set to layer 0 on join but layer 21 after death.
    // Layer 21 breaks the ignore collision. So this method resets the physicsBody back to what it was when they joined the game.
    public static void ResetPlayerPhysicsBodyCollisionLayersToDefault(EntityPlayer player)
    {
        if (player.emodel?.physicsBody?.colliders == null)
        {
            return;
        }

        foreach (IBodyColliderInstance collider in player.emodel.physicsBody.colliders)
        {
            if (collider.Transform == null || collider.Transform.gameObject?.layer == DefaultCollisionLayer)
            {
                continue;
            }

            collider.Transform.gameObject.layer = DefaultCollisionLayer;
        }
    }
}