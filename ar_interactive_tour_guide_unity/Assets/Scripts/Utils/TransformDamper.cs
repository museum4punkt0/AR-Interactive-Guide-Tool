using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// the integration as NetworkBehaviour here looks hacky
// right now, there is no clean way to distinguish between guide and visitor AR scenes though
// and we need damping only on the visitor's side

public class TransformDamper : NetworkBehaviour
{
    [SerializeField] Transform goal;
    Quaternion lastRotation;

    void Start()
    {
        lastRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        if (!isServer)
            transform.localRotation = Quaternion.Lerp(lastRotation, goal.localRotation, 0.2f);
        else transform.localRotation = goal.localRotation;

        lastRotation = transform.localRotation;
    }
}
