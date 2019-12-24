using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveText : MonoBehaviour
{

    public float x = 0.1f;
    public float y = 0.1f;
    public float z = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject game = GameObject.Find("CoreGame");
        this.transform.SetParent(GameObject.Find("Canvas").transform, false);
        this.GetComponent<Text>().text = "WAVE " + game.GetComponent<Game>().GetWaveNumber();

        //Destroy(this, 1.04f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale -= new Vector3(x, y, z);
    }
}
