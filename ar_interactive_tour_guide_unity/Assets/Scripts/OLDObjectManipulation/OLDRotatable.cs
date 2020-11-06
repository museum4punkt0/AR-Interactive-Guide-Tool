using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLDRotatable : MonoBehaviour
{

    Vector3 dirBefore, posProjected;
    public float distanceOfInteractionPlane = 1.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        var first = false;
        if (Input.GetMouseButtonDown(0))
        {

            first = true;
        }

        if (Input.GetMouseButton(0))
        {
        
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            posProjected = ray.GetPoint(distanceOfInteractionPlane);
           
            var dir = posProjected -  transform.position;
            if(first)
            {
                first = false;
                dirBefore = dir;
            }

            var difQ = Quaternion.FromToRotation(dirBefore, dir );

            transform.rotation = difQ * transform.rotation;

            dirBefore  = dir;
        }
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(posProjected, .02f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(posProjected, transform.position);
          
    }
}
