using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public GameObject sp1;
    public GameObject sp2;
    public GameObject sp3;
    public GameObject sp4;
    public GameObject bot;

    private int damageTaken = 0;

    private const int WAVE_SIZE = 1;
    [SerializeField] private float spawnDelay;

    // Start is called before the first frame update
    void Start()
    {
        Wave(sp1);
        Wave(sp2);
        Wave(sp3);
        Wave(sp4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Wave(GameObject spawnPoint)
    {
        
        StartCoroutine(SpawnBot(spawnPoint));
        
    }

    IEnumerator SpawnBot(GameObject spawnPoint)
    {
        for (int i = 0; i < WAVE_SIZE; ++i)
        {
            yield return new WaitForSeconds(spawnDelay);
            Instantiate(bot, spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    public void DamagePlayer()
    {
        ++damageTaken;
    }

    public int getDamageTaken()
    {
        return damageTaken;
    }

}
