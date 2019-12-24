using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour{

    public Text damageText;
    private GameObject coreGame;

    // Start is called before the first frame update
    void Start()
    {
        coreGame = GameObject.Find("CoreGame");
    }

    // Update is called once per frame
    void Update()
    {

        damageText.text = "DAMAGE TAKEN: " + coreGame.GetComponent<Game>().getDamageTaken();

    }
}
