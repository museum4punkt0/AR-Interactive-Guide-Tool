using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscilator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        oc = transform.GetComponent<Renderer>().material.GetColor("_EmissionColor");
    }
    public float osc;
    public float lowbound = 0.2f;
    public float upperbound = 0.8f;
    public float intensity;
    public float speed=2;
    public Color oc;

    public Color c;
    // Update is called once per frame
    void Update()
    {
        osc = Mathf.Clamp(Mathf.Sin(Time.realtimeSinceStartup*speed),lowbound,upperbound);
        c = new Color(oc.r, oc.g, oc.b)*intensity*osc;
        transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", c);
    }
}
