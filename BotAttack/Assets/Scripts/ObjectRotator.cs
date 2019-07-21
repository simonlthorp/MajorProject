using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float rotationSpeedX;
    public float rotationSpeedY;
    public float rotationSpeedZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(rotationSpeedX, rotationSpeedY, rotationSpeedZ, Space.Self);
    }
}
