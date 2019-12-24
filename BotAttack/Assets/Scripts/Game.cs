using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game : MonoBehaviour
{

    [SerializeField] private bool RANDOM = true;
    private bool EndGame = false;
    private const int NUMBER_OF_CHILDREN = 2;
    private const int TOTAL_WAVES = 100;
    public const int WAVE_SIZE = 10;
    private const int NUMBER_OF_ELITES = 2;

    public GameObject sp1;
    public GameObject sp2;
    public GameObject sp3;
    public GameObject sp4;
    public GameObject bot;
    public GameObject WaveText;
    public GameObject GameOverText;

    public AudioSource gameMusic;
    public AudioSource explosionSFX;
    public AudioSource shotSFX;
    public AudioSource raySXF;
    public AudioSource gameOverMusic;

    private List<BotData> sp1Wave = new List<BotData>();
    private List<BotData> sp2Wave = new List<BotData>();
    private List<BotData> sp3Wave = new List<BotData>();
    private List<BotData> sp4Wave = new List<BotData>();

    private BotData eliteSP1;
    private BotData eliteSP2;
    private BotData eliteSP3;
    private BotData eliteSP4;

    private int waveNumber = 1;

    private int damageTaken = 0;
    private int botsKilled = 0;

    
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

        if (RANDOM || waveNumber == 1)
        {
            numberOfBots = WAVE_SIZE * 4;
        }
        else
        {
            numberOfBots = (WAVE_SIZE * 4) + (NUMBER_OF_ELITES * 4);
        }
        

        PlayWaveText();

        SpawnWave(sp1);
        SpawnWave(sp2);
        SpawnWave(sp3);
        SpawnWave(sp4);
    }

    // Update is called once per frame
    void Update()
    {

        if (numberOfBots <= 0 && waveNumber < TOTAL_WAVES)
        {
            NewWave();
        }
        else if(numberOfBots <= 0 && waveNumber >= TOTAL_WAVES)
        {
            if (!EndGame)
            {
                Instantiate(GameOverText, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion());
                gameMusic.Stop();
                gameOverMusic.Play();
                Recorder recorder = GameObject.Find("CoreGame").GetComponent<Recorder>();
                //recorder.getBotWriter().Close();
                recorder.getWaveWriter().Close();
                StartCoroutine("Quit");
            }
            EndGame = true;
        }

    }

    private void NewWave()
    {
        ++waveNumber;

        PlayWaveText();
        
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
            numberOfBots = (WAVE_SIZE * 4) + (NUMBER_OF_ELITES * 4);
            
            //Fitness Function
            //Done in Bot.Die();

            //Selection Function
            UseSelectionFunction();

            //Recombination Function
            UseRecombinationFunction();

            //Mutation Function
            UseMutationFunction();

            //Elitism
            AddEliteBots();

            //Spawn waves
            SpawnWave(sp1);
            SpawnWave(sp2);
            SpawnWave(sp3);
            SpawnWave(sp4);

            GeneticAlgorithm GA = this.GetComponent<GeneticAlgorithm>();

            GA.botListSP1.Clear();
            GA.botListSP2.Clear();
            GA.botListSP3.Clear();
            GA.botListSP4.Clear();
            GA.eliteBots.Clear();

        }
        
    }

    private void UseSelectionFunction()
    {
        List<BotData> bots = new List<BotData>();
        GeneticAlgorithm GA = this.GetComponent<GeneticAlgorithm>();

        bots = GA.botListSP1;
        parentsSP1 = GA.SelectionFunction(bots, 1);

        bots = GA.botListSP2;
        parentsSP2 = GA.SelectionFunction(bots, 2);

        bots = GA.botListSP3;
        parentsSP3 = GA.SelectionFunction(bots, 3);

        bots = GA.botListSP4;
        parentsSP4 = GA.SelectionFunction(bots, 4);

    }

    private void UseRecombinationFunction()
    {
        GeneticAlgorithm GA = this.GetComponent<GeneticAlgorithm>();
        List<BotData> children = new List<BotData>();

        for(int i = 0; i < WAVE_SIZE/2; ++i)
        {
            
            for(int j = 0; j < NUMBER_OF_CHILDREN; ++j)
            {
                children = GA.RecombinationFunction(parentsSP1);
                sp1Wave.Add(children.ElementAt(j));
            }
        }

        for (int i = 0; i < WAVE_SIZE / 2; ++i)
        {
            
            for (int j = 0; j < NUMBER_OF_CHILDREN; ++j)
            {
                children = GA.RecombinationFunction(parentsSP2);
                sp2Wave.Add(children.ElementAt(j));
            }
        }

        for (int i = 0; i < WAVE_SIZE / 2; ++i)
        {
            
            for (int j = 0; j < NUMBER_OF_CHILDREN; ++j)
            {
                children = GA.RecombinationFunction(parentsSP3);
                sp3Wave.Add(children.ElementAt(j));
            }
        }

        for (int i = 0; i < WAVE_SIZE / 2; ++i)
        {
            
            for (int j = 0; j < NUMBER_OF_CHILDREN; ++j)
            {
                children = GA.RecombinationFunction(parentsSP4);
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

    //Adds elite bots to the beginning and the end of the wave
    private void AddEliteBots()
    {
        GeneticAlgorithm GA = this.GetComponent<GeneticAlgorithm>();

        sp1Wave.Insert(0, GA.eliteBots.ElementAt(0));
        sp1Wave.Add(GA.eliteBots.ElementAt(0));

        sp2Wave.Insert(0, GA.eliteBots.ElementAt(1));
        sp2Wave.Add(GA.eliteBots.ElementAt(1));

        sp3Wave.Insert(0, GA.eliteBots.ElementAt(2));
        sp3Wave.Add(GA.eliteBots.ElementAt(2));

        sp4Wave.Insert(0, GA.eliteBots.ElementAt(3));
        sp4Wave.Add(GA.eliteBots.ElementAt(3));
    }

    private void SpawnWave(GameObject spawnPoint)
    {
        
        if(RANDOM || waveNumber == 1)
        {
            StartCoroutine(SpawnBot(spawnPoint, 0)); //Spawn without added elites
        }
        else
        {
            StartCoroutine(SpawnBot(spawnPoint, NUMBER_OF_ELITES)); //Spawn with added elites
        }
        
    }

    /*
     * Spawnbot - Spawns bots a random distance from the supplied spawn point
     *      - repeats for amount of times specified by WAVE_SIZE
     *      - Delays for time specified by spawnDelay
     *      - Creates a spawn position a random distance between -10 and 10 from the spawn point
     *      - Spawns a bot at the spawn spoint
     */
    IEnumerator SpawnBot(GameObject spawnPoint, int numberOfElites)
    {

        for (int i = 0; i < WAVE_SIZE + numberOfElites; ++i)
        {
            yield return new WaitForSeconds(spawnDelay);
            //Vector3 pos = new Vector3(spawnPoint.transform.position.x + Random.Range(-10.0f, 10.0f), spawnPoint.transform.position.y + Random.Range(-5.0f, 5.0f), spawnPoint.transform.position.z);
            Vector3 pos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
            GameObject b = Instantiate(bot, pos, spawnPoint.transform.rotation);

            if (spawnPoint == sp1)
            {

                if (!RANDOM && GetWaveNumber() > 1)
                {
                    b.GetComponent<Bot>().setGAGenome(GetWave(1), sp1Bot);
                }

                b.GetComponent<Bot>().setBotNumber(sp1Bot);
                ++sp1Bot;
            }
            if (spawnPoint == sp2)
            {

                if (!RANDOM && GetWaveNumber() > 1)
                {
                    b.GetComponent<Bot>().setGAGenome(GetWave(2), sp2Bot);
                }

                b.GetComponent<Bot>().setBotNumber(sp2Bot);
                ++sp2Bot;
            }
            if (spawnPoint == sp3)
            {

                if (!RANDOM && GetWaveNumber() > 1)
                {
                    b.GetComponent<Bot>().setGAGenome(GetWave(3), sp3Bot);
                }

                b.GetComponent<Bot>().setBotNumber(sp3Bot);
                ++sp3Bot;
            }
            if (spawnPoint == sp4)
            {

                if (!RANDOM && GetWaveNumber() > 1)
                {
                    b.GetComponent<Bot>().setGAGenome(GetWave(4), sp4Bot);
                }

                b.GetComponent<Bot>().setBotNumber(sp4Bot);
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

    public void incrementBotsKilled()
    {
        ++botsKilled;
    }

    public int getBotsKilled()
    {
        return botsKilled;
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

    private void PlayWaveText()
    {
        GameObject t = Instantiate(WaveText, new Vector3(0.0f, 0.0f, 0.0f), new Quaternion());
        Destroy(t, 0.85f);
    }

    IEnumerator Quit()
    {
        yield return new WaitForSeconds(5.0f);
        Application.Quit();
    }

}
