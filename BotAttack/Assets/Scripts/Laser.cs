using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    public GameObject firepoint;
    public LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        lr.SetPosition(0, firepoint.transform.position);
        lr.SetPosition(1, new Vector3(0.0f, 0.0f, 0.0f));

    }
}
