using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Will allow you to position the camera
// given a Marker and TrackedImage transform
// note that you MUST NOT scale the marker OR the trackedImage transform
// since Unity (for some reason I dont understand) does not apply scales to a camera

// ARSessionOrigin should allow you to do the same thing but it's very buggy
// and will often apply rotations wrongly

public class ARCameraPositioner : MonoBehaviour
{
    [SerializeField] bool doLerp;
    [SerializeField] float lerpDuration = 0.1f;


    public void MakeContentAppearAt(Transform content, Space contentSpace, Transform appearAt, Space appearAtSpace)
    {

        //var session = transform.parent.GetComponent<ARSessionOrigin>();
        //session.MakeContentAppearAt(content,appearAt.transform.position);
        //return;
        

        var appearAtMatrix = FromTransform(appearAt, appearAtSpace);
        var contentMatrix = FromTransform(content, contentSpace);

        Matrix4x4 newTransformMatrix = contentMatrix * Matrix4x4.Inverse(appearAtMatrix); // appearAt.worldToLocalMatrix;

        // SetFromMatrix(transform, newTransformMatrix);
        if (doLerp && gameObject.activeInHierarchy)
            StartCoroutine(LerpTo(newTransformMatrix));
        else
            SetFromMatrix(transform, newTransformMatrix);

    }

    private IEnumerator LerpTo(Matrix4x4 matrix)
    {
        Vector3 goalPosition = GetPosition(matrix);
        Quaternion goalRotation = matrix.rotation;
        Vector3 goalScale = matrix.lossyScale;

        float lerpParam = 0;

        while (lerpParam <= 1f)
        {
            var smoothedLerpParam = Mathf.SmoothStep(0, 1, lerpParam);
            transform.localPosition = Vector3.Lerp(transform.localPosition, goalPosition, smoothedLerpParam);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, goalRotation, smoothedLerpParam);
            transform.localScale = Vector3.Lerp(transform.localScale, goalScale, smoothedLerpParam);

            lerpParam += Time.deltaTime / lerpDuration;
            yield return null;
        }
    }

    Vector3 GetPosition(Matrix4x4 matrix)
    {
        return matrix.GetColumn(3);
    }

    void SetFromMatrix(Transform t, Matrix4x4 m)
    {
        t.localPosition = GetPosition(m);
        t.localRotation = m.rotation;
        //t.localScale = m.lossyScale;
        t.localScale = Vector3.one;
    }

    Matrix4x4 FromTransform(Transform t, Space space)
    {
        if (space == Space.Self)
            return Matrix4x4.TRS(t.localPosition, t.localRotation, t.localScale);
            //return Matrix4x4.TRS(t.localPosition, t.localRotation, Vector3.one);
        return Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
        //return Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
    }
}
