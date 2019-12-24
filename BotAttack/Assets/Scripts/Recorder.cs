using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Recorder : MonoBehaviour
{

    private StreamWriter botWriter;
    private StreamWriter waveWriter;

    private GeneticAlgorithm GA;

    public Recorder() { }

    // Start is called before the first frame update
    void Start()
    {

        GA = GameObject.Find("CoreGame").GetComponent<GeneticAlgorithm>();

        //Instantiate the bot records file
        botWriter = new StreamWriter("BotRecords.txt", true);
        botWriter.WriteLine("Selection Function Testing");
        botWriter.Flush();
        
        //Instantiate the wave records file
        waveWriter = new StreamWriter("WaveRecords.csv", true);
        waveWriter.WriteLine("Wave,Spawnpoint,Bot Number,Damage Dealt,Time Alive,Escaped");
        waveWriter.Flush();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    public void recordSelectionFunction(List<BotData> bots, float selector, BotData parent)
    {
        foreach(BotData bot in bots)
        {
            botWriter.Write("Bot Genome: " + bot.getGenome().ElementAt(0));
            for(int i = 1; i < bot.getGenome().Length; ++i)
            {
                botWriter.Write(", " + bot.getGenome().ElementAt(i));
            }
            botWriter.WriteLine();
            botWriter.WriteLine("Bot Cumulative Fitness: " + bot.getCumulativeValue());
        }
        botWriter.WriteLine("Random Number: " + selector);
        botWriter.WriteLine("Selected Parent Genome: " + parent.getGenome().ElementAt(0));
        for(int i = 1; i < parent.getGenome().Length; ++i)
        {
            botWriter.Write(", " + parent.getGenome().ElementAt(i));
        }
    }

    public void recordFitnessFunction(Bot bot)
    {

        botWriter.WriteLine("Damage Dealt:      " + bot.getDamageDealt());
        botWriter.WriteLine("Damage Multiplier: " + GA.getDamageMultiplier());
        botWriter.WriteLine("Time Alive:        " + bot.getTimeAlive());
        botWriter.WriteLine("Time Multiplier:   " + GA.getTimeMultiplier());
        if (bot.isEscaped())
        {
            botWriter.WriteLine("Escaped:       True");
        }
        else
        {
            botWriter.WriteLine("Escaped:       False");
        }
        botWriter.WriteLine("Escape Multiplier: " + GA.getEscapeMultiplier());

        botWriter.WriteLine("Fitness Value:     " + GA.FitnessFunction(bot));
        

    }

    
    public void recordBot(int[] genome)
    {
        GameObject CoreGame = GameObject.Find("CoreGame");
        botWriter.Write("Elite Bot:,");
        botWriter.Write("Wave " + CoreGame.GetComponent<Game>().GetWaveNumber());
        foreach(int gene in genome)
        {
            botWriter.Write("," + gene);
        }

        botWriter.Write(botWriter.NewLine);
        botWriter.Flush();
    }
    
    public void recordParents(List<BotData> parents, int crossover)
    {
        int count = 0;
        foreach (BotData bot in parents)
        {
            botWriter.Write("Parent" + count + ":,");

            foreach(int gene in bot.getGenome())
            {
                botWriter.Write(gene + ",");
            }
            
            ++count;
            botWriter.Write(botWriter.NewLine);
        }

        botWriter.WriteLine("Crossover Point:," + crossover);
        botWriter.Flush();
    }

    public void recordChildren(List<BotData> children)
    {
        int count = 0;
        foreach (BotData bot in children)
        {
            botWriter.Write("Child" + count + ":,");

            foreach (int gene in bot.getGenome())
            {
                botWriter.Write(gene + ",");
            }

            ++count;
            botWriter.Write(botWriter.NewLine);
        }


        botWriter.Flush();
    }

    public void recordMutated(List<BotData> mutatated)
    {
        int count = 0;
        foreach (BotData bot in mutatated)
        {
            botWriter.Write("Mutated" + count + ":,");

            foreach (int gene in bot.getGenome())
            {
                botWriter.Write(gene + ",");
            }

            ++count;
            botWriter.Write(botWriter.NewLine);
        }


        botWriter.Flush();
    }
    */
    public void recordWave(int wave, int spawnpoint, int botNumber, int damageDealt, float timeAlive, int escaped)
    {
        waveWriter.WriteLine(wave + "," + spawnpoint + "," + botNumber + "," + damageDealt + "," + timeAlive + "," + escaped);
        waveWriter.Flush();
    }
    
    public StreamWriter getBotWriter()
    {
        return botWriter;
    }
    
    public StreamWriter getWaveWriter()
    {
        return waveWriter;
    }

}
