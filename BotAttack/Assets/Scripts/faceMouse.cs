using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceMouse : MonoBehaviour
{
    public float maximumLength;
    public Camera cam;

    private float angle;
    private float mPointX;
    private float mPointY;
    private float oPointX;
    private float oPointY;
    

    // Update is called once per frame
    void Update()
    {
        
        mPointX = Input.mousePosition.x;
        mPointY = Input.mousePosition.y;
        oPointX = Screen.width/2;
        oPointY = Screen.height/2;

        angle = Mathf.Atan2(mPointY - oPointY, mPointX - oPointX) * Mathf.Rad2Deg - 90.0f;

        this.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        
    }

    public Quaternion getRotation()
    {
        
        Quaternion rotation;
        Vector3 direction;
        Vector3 mousePos;
        

        mousePos = Input.mousePosition;

        direction = mousePos - gameObject.transform.position;
        rotation = Quaternion.LookRotation(direction);

        Debug.Log(rotation.ToString());
        return rotation;
    }
}
