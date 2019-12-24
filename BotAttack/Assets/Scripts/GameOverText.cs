using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverText : MonoBehaviour
{

    public float x = 0.000000001f;
    public float y = 0.000000001f;
    public float z = 0.000000001f;

    // Start is called before the first frame update
    void Start()
    {

        this.transform.SetParent(GameObject.Find("Canvas").transform, false);
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(x, y, z);
    }
}
