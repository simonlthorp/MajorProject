using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Recorder : MonoBehaviour
{

    private StreamWriter botWriter;
    private StreamWriter waveWriter;

    public Recorder() { }

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate the bot records file
        botWriter = new StreamWriter("BotRecords.csv", true);
        botWriter.WriteLine("Wave,Spawnpoint,Bot Number,Damage Dealt,Time Alive");

        //Instantiate the wave records file
        waveWriter = new StreamWriter("WaveRecords.csv", true);
        waveWriter.WriteLine("Wave,Damage Dealt,Wave Time");

    }

    // Update is called once per frame
    void Update()
    {
        /*
        for(int i = 0; i < 10; ++i)
        {
            recordBot(1,1, i, 10, 25);
        }
        */
    }

    public void recordBot(int wave, int spawnpoint, int botNumber, int damageDealt, float timeAlive)
    {
        botWriter.WriteLine(wave + "," + spawnpoint + "," + botNumber + "," + damageDealt + "," + timeAlive);
        botWriter.Flush();
    }

    public void recordWave(int wave, int damageDealt, int enemiesKilled, int waveTime)
    {
        botWriter.WriteLine(wave + "," + damageDealt + "," + waveTime);
        botWriter.Flush();
    }

}
