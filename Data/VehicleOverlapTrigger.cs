using System;
using UnityEngine;

namespace CollisionsBeGone;

public class VehicleOverlapTrigger : MonoBehaviour
{
    public EntityVehicle Vehicle { get; private set; }
    public int CollidersInTriggerCount { get; private set; }

    public static VehicleOverlapTrigger Attach(EntityVehicle vehicle)
    {
        Detach(vehicle);

        GameObject rootObj = new(nameof(VehicleOverlapTrigger));
        rootObj.transform.SetParent(vehicle.vehicleRB.transform);
        rootObj.transform.localPosition = Vector3.zero;
        rootObj.transform.localRotation = Quaternion.identity;

        VehicleOverlapTrigger component = rootObj.AddComponent<VehicleOverlapTrigger>();
        component.Vehicle = vehicle;

        foreach (Collider col in vehicle.vehicleRB.GetComponentsInChildren<Collider>())
        {
            if (col.isTrigger || col.bounds.size == Vector3.zero)
            {
                continue;
            }

            if (col is not BoxCollider box)
            {
                continue;
            }

            GameObject colObj = new($"Trigger_{col.name}");
            colObj.transform.SetParent(col.transform);
            colObj.transform.localPosition = Vector3.zero;
            colObj.transform.localRotation = Quaternion.identity;
            colObj.transform.localScale = Vector3.one;

            BoxCollider trigger = colObj.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center = box.center;
            trigger.size = box.size;

            VehicleOverlapTriggerPart triggerPart = colObj.AddComponent<VehicleOverlapTriggerPart>();
            triggerPart.Parent = component;

            if (CollisionsBeGoneMod.Config.DebugColliders)
            {
                GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                visual.transform.SetParent(colObj.transform);
                visual.transform.localPosition = box.center;
                visual.transform.localRotation = Quaternion.identity;
                visual.transform.localScale = box.size;
                GameObject.Destroy(visual.GetComponent<Collider>());
            }
        }

        GeneralUtility.LogLine("Attached VehicleOverlapTrigger");
        return component;
    }

    public static void Detach(EntityVehicle vehicle)
    {
        foreach (VehicleOverlapTriggerPart part in vehicle.vehicleRB.GetComponentsInChildren<VehicleOverlapTriggerPart>())
        {
            Destroy(part.gameObject);
        }
            
        VehicleOverlapTrigger triggerComponent = vehicle.vehicleRB.GetComponentInChildren<VehicleOverlapTrigger>();

        if (triggerComponent != null)
        {
            Destroy(triggerComponent.gameObject);
            GeneralUtility.LogLine("Detached VehicleOverlapTrigger");
        }
    }

    internal void TriggerEntered(Collider otherCollider)
    {
        if (!IsRemotePlayerCollider(otherCollider))
        {
            return;
        }

        CollidersInTriggerCount++;
    }

    internal void TriggerExited(Collider otherCollider)
    {
        if (!IsRemotePlayerCollider(otherCollider))
        {
            return;
        }

        CollidersInTriggerCount = Math.Max(0, CollidersInTriggerCount - 1);
    }

    private bool IsRemotePlayerCollider(Collider collider)
    {
        if (!CollisionUtility.IsOnRemotePlayerLayer(collider.gameObject))
        {
            return false;
        }

        if (collider.GetComponentInParent<EntityPlayerLocal>() != null)
        {
            return false;
        }

        return true;
    }
}

public class VehicleOverlapTriggerPart : MonoBehaviour
{
    public VehicleOverlapTrigger Parent { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        Parent?.TriggerEntered(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Parent?.TriggerExited(other);
    }
}