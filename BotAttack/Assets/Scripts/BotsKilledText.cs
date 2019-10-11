using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotsKilledText : MonoBehaviour
{
    public Text botsKilledText;
    private GameObject coreGame;

    // Start is called before the first frame update
    void Start()
    {
        coreGame = GameObject.Find("CoreGame");
    }

    // Update is called once per frame
    void Update()
    {

        botsKilledText.text = "DAMAGE: " + coreGame.GetComponent<Game>().getBotsKilled();

    }
}
