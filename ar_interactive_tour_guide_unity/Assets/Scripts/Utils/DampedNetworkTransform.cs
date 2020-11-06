using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// TODO this does not yet send position (not needed atm)
// TODO low prio: a lot of code duplication.
// Cries for a generic DampedSyncVar or the like
// but NetworkBehaviours cannot be generic so this requires some extra thought

public class DampedNetworkTransform : NetworkBehaviour
{
    [Header("Server Settings")]
    public float rotationSendThreshold = 0.01f;
    public float scaleSendThreshold = 0.01f;

    [Header("Client Settings")]
    public float rotationDampThreshold = 0.5f;
    public float rotationDampSpeed = 3f;
    public float scaleDampThreshold = 0.05f;
    public float scaleDampSpeed = 3f;

    // Server
    Quaternion lastSentRotation;
    Vector3 lastSentScale;

    // Client
    Coroutine dampRotation;
    Coroutine dampScale;

    void Start()
    {
        if (isServer)
        {
            SendRotation(transform.localRotation);
            SendScale(transform.localScale);
        }
    }

    private void LateUpdate()
    {
        if (isServer)
        {
            if (Quaternion.Angle(transform.localRotation, lastSentRotation) > rotationSendThreshold)
                SendRotation(transform.localRotation);

            if (Vector3.Distance(transform.localScale, lastSentScale) > scaleSendThreshold)
                SendScale(transform.localScale);
        }
    }

    void SendRotation(Quaternion rotation)
    {
        lastSentRotation = rotation;
        RpcSendRotation(rotation);
    }

    void SendScale(Vector3 scale)
    {
        lastSentScale = scale;
        RpcSendScale(scale);
    }

    [ClientRpc]
    public void RpcSendRotation(Quaternion rotation)
    {
        if(!isServer)
        {
            if (dampRotation != null)
                StopCoroutine(dampRotation);
            dampRotation = StartCoroutine(DampToRotation(rotation, rotationDampSpeed, rotationDampThreshold));
        }
    }

    [ClientRpc]
    public void RpcSendScale(Vector3 scale)
    {
        if (!isServer)
        {
            if (dampScale != null)
                StopCoroutine(dampScale);
            dampScale = StartCoroutine(DampToScale(scale, scaleDampSpeed, scaleDampThreshold));
        }
    }

    IEnumerator DampToRotation(Quaternion goal, float speed, float angleThreshold)
    {
        // making sure external changes in localRotation do not influence while loop condition
        Quaternion dampedRotation = transform.localRotation;

        while(Quaternion.Angle(dampedRotation, goal) > angleThreshold)
        {
            dampedRotation = Quaternion.Lerp(dampedRotation, goal, speed * Time.deltaTime);
            transform.localRotation = dampedRotation;
            yield return null;
        } 
    }

    IEnumerator DampToScale(Vector3 goal, float speed, float threshold)
    {
        Vector3 dampedScale = transform.localScale;

        while(Vector3.Distance(dampedScale, goal) > threshold)
        {
            dampedScale = Vector3.Lerp(dampedScale, goal, speed * Time.deltaTime);
            transform.localScale = dampedScale;
            yield return null;
        }
    }
}
