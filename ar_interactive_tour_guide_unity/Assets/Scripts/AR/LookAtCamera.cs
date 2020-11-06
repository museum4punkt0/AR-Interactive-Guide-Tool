using UnityEngine;
using System.Collections;
 
public class LookAtCamera : MonoBehaviour
{
	Camera referenceCamera;
	public bool reverseFace = false;

	void Awake()
	{
		if (referenceCamera == null)
			referenceCamera = Camera.main;
	}

	//Orient the camera after all movement is completed this frame to avoid jittering
	void LateUpdate()
	{
		// rotates the object relative to the camera
		Vector3 targetPos = transform.position + referenceCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back);
		Vector3 targetOrientation = referenceCamera.transform.rotation * Vector3.up;
		transform.LookAt(targetPos, targetOrientation);
	}
}