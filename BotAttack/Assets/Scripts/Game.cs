using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game : MonoBehaviour
{

    [SerializeField] private bool RANDOM = true;
    private const int NUMBER_OF_CHILDREN = 2;

    public GameObject sp1;
    public GameObject sp2;
    public GameObject sp3;
    public GameObject sp4;
    public GameObject bot;

    private List<BotData> sp1Wave = new List<BotData>();
    private List<BotData> sp2Wave = new List<BotData>();
    private List<BotData> sp3Wave = new List<BotData>();
    private List<BotData> sp4Wave = new List<BotData>();

    private int waveNumber = 1;

    private int damageTaken = 0;

    public const int WAVE_SIZE = 2;
    [SerializeField] private float spawnDelay;

    //Bot spawning numbers
    private int sp1Bot = 0;
    private int sp2Bot = 0;
    private int sp3Bot = 0;
    private int sp4Bot = 0;

    //Parents
    private List<BotData> parentsSP1;
    private List<BotData> parentsSP2;
    private List<BotData> parentsSP3;
    private List<BotData> parentsSP4;

    //The number of bots lefts in the scene
    private int numberOfBots;

    // Start is called before the first frame update
    void Start()
    {
        numberOfBots = WAVE_SIZE * 4;

        SpawnWave(sp1);
        SpawnWave(sp2);
        SpawnWave(sp3);
        SpawnWave(sp4);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(numberOfBots <= 0)
        {
            NewWave();
        }

    }

    private void NewWave()
    {
        ++waveNumber;

        sp1Bot = 0;
        sp2Bot = 0;
        sp3Bot = 0;
        sp4Bot = 0;

        if (RANDOM)
        {
            numberOfBots = WAVE_SIZE * 4;
            SpawnWave(sp1);
            SpawnWave(sp2);
            SpawnWave(sp3);
            SpawnWave(sp4);
        }
        else
        {
            numberOfBots = WAVE_SIZE * 4;
            ///TODO - add bots to sp1Wave list
            ///TODO - Fitness Function
            //Done in Bot.Die();

            ///TODO - Selection Function
            UseSelectionFunction();

            ///TODO - Recombination Function
            UseRecombinationFunction();

            ///TODO - Mutation Function
            UseMutationFunction();

            ///TODO - Elitism

            ///TODO - Spawn waves
            SpawnWave(sp1);
            SpawnWave(sp2);
            SpawnWave(sp3);
            SpawnWave(sp4);
        }
        
    }

    private void UseSelectionFunction()
    {
        List<BotData> bots = new List<BotData>();
        GeneticAlgorithm GA = this.GetComponent<GeneticAlgorithm>();

        bots = GA.botListSP1;
        parentsSP1 = GA.SelectionFunction(bots);

        bots = GA.botListSP2;
        parentsSP2 = GA.SelectionFunction(bots);

        bots = GA.botListSP3;
        parentsSP3 = GA.SelectionFunction(bots);

        bots = GA.botListSP4;
        parentsSP4 = GA.SelectionFunction(bots);

    }

    private void UseRecombinationFunction()
    {
        GeneticAlgorithm GA = this.GetComponent<GeneticAlgorithm>();
        List<BotData> children = new List<BotData>();

        for(int i = 0; i < WAVE_SIZE/2; ++i)
        {
            children = GA.RecombinationFunction(parentsSP1);
            for(int j = 0; j < NUMBER_OF_CHILDREN; ++j)
            {
                sp1Wave.Add(children.ElementAt(j));
            }
        }

        for (int i = 0; i < WAVE_SIZE / 2; ++i)
        {
            children = GA.RecombinationFunction(parentsSP2);
            for (int j = 0; j < NUMBER_OF_CHILDREN; ++j)
            {
                sp2Wave.Add(children.ElementAt(j));
            }
        }

        for (int i = 0; i < WAVE_SIZE / 2; ++i)
        {
            children = GA.RecombinationFunction(parentsSP3);
            for (int j = 0; j < NUMBER_OF_CHILDREN; ++j)
            {
                sp3Wave.Add(children.ElementAt(j));
            }
        }

        for (int i = 0; i < WAVE_SIZE / 2; ++i)
        {
            children = GA.RecombinationFunction(parentsSP4);
            for (int j = 0; j < NUMBER_OF_CHILDREN; ++j)
            {
                sp4Wave.Add(children.ElementAt(j));
            }
        }
    }

    private void UseMutationFunction()
    {
        GeneticAlgorithm GA = this.GetComponent<GeneticAlgorithm>();

        sp1Wave = GA.MutationFunction(sp1Wave);
        sp2Wave = GA.MutationFunction(sp2Wave);
        sp3Wave = GA.MutationFunction(sp3Wave);
        sp4Wave = GA.MutationFunction(sp4Wave);
    }

    private void SpawnWave(GameObject spawnPoint)
    {
        
        StartCoroutine(SpawnBot(spawnPoint));
        
    }

    /*
     * Spawnbot - Spawns bots at the supplied spawn point
     *      - repeats for amount of times specified by WAVE_SIZE
     *      - Delays for time specified by spawnDelay
     *      - Spawns a bot at the spawn spoint
     */
    IEnumerator SpawnBot(GameObject spawnPoint)
    {
        for (int i = 0; i < WAVE_SIZE; ++i)
        {
            yield return new WaitForSeconds(spawnDelay);
            
            
            
            if(spawnPoint == sp1)
            {
                GameObject b = Instantiate(bot, spawnPoint.transform.position, spawnPoint.transform.rotation);

                if (!RANDOM && GetWaveNumber() > 1)
                {
                    b.GetComponent<Bot>().setGAGenome(GetWave(1), sp1Bot);
                }
                
                ++sp1Bot;
            }
            if (spawnPoint == sp2)
            {
                GameObject b = Instantiate(bot, spawnPoint.transform.position, spawnPoint.transform.rotation);

                if (!RANDOM && GetWaveNumber() > 1)
                {
                    b.GetComponent<Bot>().setGAGenome(GetWave(2), sp2Bot);
                }

                ++sp2Bot;
            }
            if (spawnPoint == sp3)
            {
                GameObject b = Instantiate(bot, spawnPoint.transform.position, spawnPoint.transform.rotation);

                if (!RANDOM && GetWaveNumber() > 1)
                {
                    b.GetComponent<Bot>().setGAGenome(GetWave(3), sp3Bot);
                }

                ++sp3Bot;
            }
            if (spawnPoint == sp4)
            {
                GameObject b = Instantiate(bot, spawnPoint.transform.position, spawnPoint.transform.rotation);

                if (!RANDOM && GetWaveNumber() > 1)
                {
                    b.GetComponent<Bot>().setGAGenome(GetWave(4), sp4Bot);
                }

                ++sp4Bot;
            }

            bot.GetComponent<Bot>().setWaveNumber(waveNumber);
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

    //Reduce the number of bots left on the scene by 1
    public void DecrementNumberOfBots()
    {
        --numberOfBots;
    }

    /*
     * GetWave - returns a list of bots with gene data
     *      - Takes an int from 1-4 as a spawnpoint
     *      - Returns the wave corresponding to the provided spawnpoint
     */
    public List<BotData> GetWave(int spawnPoint)
    {
        if(spawnPoint == 1)
        {
            return sp1Wave;
        }
        else if(spawnPoint == 2)
        {
            return sp2Wave;
        }
        else if(spawnPoint == 3)
        {
            return sp3Wave;
        }
        else if (spawnPoint == 4)
        {
            return sp4Wave;
        }
        return new List<BotData>();
    }

    public int GetBotSpawnNumber(int spawnPoint)
    {
        if (spawnPoint == 1)
        {
            return sp1Bot;
        }
        else if (spawnPoint == 2)
        {
            return sp2Bot;
        }
        else if (spawnPoint == 3)
        {
            return sp3Bot;
        }
        else if (spawnPoint == 4)
        {
            return sp4Bot;
        }
        return 0;
    }

    public int GetWaveNumber()
    {
        return waveNumber;
    }

    public bool isRandom()
    {
        return RANDOM;
    }

}
