using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Recorder : MonoBehaviour
{

    private StreamWriter botWriter;
    private StreamWriter waveWriter;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate the bot records file
        botWriter = new StreamWriter("BotRecords.csv", true);
        botWriter.WriteLine("Wave,Number,Damage Dealt,Time Alive");

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
            recordBot("SP1-2", i, 10, 25);
        }
        */
    }

    public void recordBot(string wave, int botNumber, int damageDealt, int timeAlive)
    {

        botWriter.WriteLine(wave + "," + botNumber + "," + damageDealt + "," + timeAlive);
        
    }

    public void recordWave(string wave, int damageDealt, int enemiesKilled, int waveTime)
    {
        botWriter.WriteLine(wave + "," + damageDealt + "," + waveTime);
    }

}
