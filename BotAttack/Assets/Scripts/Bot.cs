using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bot : MonoBehaviour
{

    public const int GENOME_SIZE = 10;

    private float spawnTime;
    private float deathTime;
    private int damageDealtToPlayer;
    private float timeAlive;
    private bool escaped;
    private Vector3 botPosition;

    private int[] genome = new int[GENOME_SIZE];
    public int genomePositition = 0;

    //Movement Variables
    public float speed;
    public float distanceToWaypoint;
    public bool escaping = false;
    public Vector3 closestSpawn;
    private bool foundClosest = false;

    //Attack Particle System
    public LineRenderer lr;
    public ParticleSystem ps;
    public GameObject explodePrefab;

    //Spawn Points
    public GameObject sp1;
    public GameObject sp2;
    public GameObject sp3;
    public GameObject sp4;

    private GameObject spawnPoint;
    private int spawnPointNumber;
    private int waveNumber;

    private BotData botData;

    private bool isDamaging = false;
    private float elapsedTime;
    private const float DAMAGE_INTERVAL = 0.3f;

    private GameObject CoreGame;

    //Awake is called before start
    private void Awake()
    {
        sp1 = GameObject.Find("SpawnPoint01");
        sp2 = GameObject.Find("SpawnPoint02");
        sp3 = GameObject.Find("SpawnPoint03");
        sp4 = GameObject.Find("SpawnPoint04");

        CoreGame = GameObject.Find("CoreGame");

        spawnTime = Time.fixedTime;

        spawnPoint = findClosestSpawn();

        ps.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        //Set the bot's genome
        if (CoreGame.GetComponent<Game>().isRandom() || CoreGame.GetComponent<Game>().GetWaveNumber() == 1)
        {
            for (int i = 0; i < genome.Length; ++i)
            {
                genome[i] = Random.Range(1, 48);
            }
        }
        //else
        //{
        //    setGAGenome(CoreGame.GetComponent<Game>().GetWave(spawnPointNumber), 1 /* BOT NUMBER IN THE WAVE */);
        //}
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!escaping)
        {
            //Debug.Log("Moving");
            MoveBot(genome[genomePositition]);
        }
        else
        {
            if (!foundClosest)
            {
                findClosestSpawnPosition();
            }
            BotEscape();
        }

        if (isDamaging)
        {
            DamagePlayer();
        }

        elapsedTime += Time.deltaTime;

    }

    private void MoveBot(int section)
    {

        Vector3 moveToPosition;
        moveToPosition = convertSection(section);

        if (Vector3.Distance(moveToPosition, transform.position) < distanceToWaypoint)
        {
            genomePositition++;
        }

        //Bot has ended it's flightpath and now attempts escape
        if(genomePositition >= genome.Length)
        {
            //Debug.Log("Escaping");
            escaping = true;

        }

        if (!escaping)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveToPosition, Time.deltaTime * speed);
        }
        
    }

    private Vector3 findClosestSpawnPosition()
    {
        Vector3 closest = sp1.transform.position;
        spawnPointNumber = 1;

        if(Vector3.Distance(transform.position, closest) > Vector3.Distance(transform.position, sp2.transform.position))
        {
            closest = sp2.transform.position;
            spawnPointNumber = 2;
        }

        if (Vector3.Distance(transform.position, closest) > Vector3.Distance(transform.position, sp3.transform.position))
        {
            closest = sp3.transform.position;
            spawnPointNumber = 3;
        }

        if (Vector3.Distance(transform.position, closest) > Vector3.Distance(transform.position, sp4.transform.position))
        {
            closest = sp4.transform.position;
            spawnPointNumber = 4;
        }

        foundClosest = true;

        closestSpawn = closest;

        return closest;
    }

    //private int findClosestSpawnNumber()
    //{
    //    int sp = 0;

    //    Vector3 closest = sp1.transform.position;

    //    if (Vector3.Distance(transform.position, closest) > Vector3.Distance(transform.position, sp2.transform.position))
    //    {
    //        closest = sp2.transform.position;
    //    }

    //    if (Vector3.Distance(transform.position, closest) > Vector3.Distance(transform.position, sp3.transform.position))
    //    {
    //        closest = sp3.transform.position;
    //    }

    //    if (Vector3.Distance(transform.position, closest) > Vector3.Distance(transform.position, sp4.transform.position))
    //    {
    //        closest = sp4.transform.position;
    //    }

    //    return sp;
    //}

    private GameObject findClosestSpawn()
    {
        GameObject closestSpawn = sp1;
        spawnPointNumber = 1;

        if (Vector3.Distance(transform.position, closestSpawn.transform.position) > Vector3.Distance(transform.position, sp2.transform.position))
        {
            closestSpawn = sp2;
            spawnPointNumber = 2;
        }

        if (Vector3.Distance(transform.position, closestSpawn.transform.position) > Vector3.Distance(transform.position, sp3.transform.position))
        {
            closestSpawn = sp3;
            spawnPointNumber = 3;
        }

        if (Vector3.Distance(transform.position, closestSpawn.transform.position) > Vector3.Distance(transform.position, sp4.transform.position))
        {
            closestSpawn = sp4;
            spawnPointNumber = 4;
        }

        return closestSpawn;
    }

    private void StopBot()
    {
        speed = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {

        //When colliding with planet:
        //  - Play attack particle system
        //  - Damage Player
        if(collision.gameObject.tag == "planet")
        {
            
            lr.enabled = true;
            ps.Play();
            
            isDamaging = true;

        }

        /*
         * When colliding with bullet:
         *      - Play explosion VFX
         *      - Kill the bot with the Die() function
         */
        if (collision.gameObject.tag == "bullet")
        {
            if (explodePrefab != null)
            {

                GameObject explodeVFX = Instantiate(explodePrefab, this.transform.position, Quaternion.identity);
                ParticleSystem psExplode = explodeVFX.GetComponent<ParticleSystem>();

                if (psExplode != null)
                {
                    Destroy(explodeVFX, psExplode.main.duration);
                }
                else
                {
                    ParticleSystem psChild = explodeVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(explodeVFX, psChild.main.duration);
                }

            }
            else
            {
                Debug.Log("Explosion Particle System Not Assigned");
            }

            Die();
            //Destroy(gameObject);

        }

    }

    private void OnCollisionExit(Collision collision)
    {

        //When exiting collision with planet
        //  - Stop attack particles systems
        if (collision.gameObject.tag == "planet")
        {

            lr.enabled = false;
            ps.Stop();

            isDamaging = false;

        }
    }

    /*
     * DamagePlayer - Deals damage to the player in time increments equal to DAMAGE_INTERVAL
     *      - Updates the game UI
     *      - increment the damageDealtToPlayer variable
     */
    private void DamagePlayer()
    {

        if(elapsedTime >= DAMAGE_INTERVAL)
        {
            CoreGame.GetComponent<Game>().DamagePlayer();
            ++damageDealtToPlayer;
            elapsedTime = elapsedTime % DAMAGE_INTERVAL;
        }

    }

    /*
     * Die - Kills the bot
     *      - Sets the time alive variable
     *      - Destroy the bot
     */
    private void Die()
    {
        deathTime = Time.fixedTime;

        timeAlive = deathTime - spawnTime;

        CoreGame.GetComponent<Game>().DecrementNumberOfBots();

        botData = new BotData(CoreGame.GetComponent<GeneticAlgorithm>().FitnessFunction(this), genome);

        if(spawnPointNumber == 1)
        {
            CoreGame.GetComponent<GeneticAlgorithm>().botListSP1.Add(botData);
        }

        if (spawnPointNumber == 2)
        {
            CoreGame.GetComponent<GeneticAlgorithm>().botListSP2.Add(botData);
        }

        if (spawnPointNumber == 3)
        {
            CoreGame.GetComponent<GeneticAlgorithm>().botListSP3.Add(botData);
        }

        if (spawnPointNumber == 4)
        {
            CoreGame.GetComponent<GeneticAlgorithm>().botListSP4.Add(botData);
        }

        Destroy(gameObject);

    }

    /*
     * convertSection - Converts one of 48 sections of the screen to game coordinates
     * eg: input int 1 returns Vector3(-35,25,0)
     */
    private Vector3 convertSection(int section)
    {
        //Sections 1 to 48 converted into a position vector
        Vector3 position = new Vector3(0, 0, 0);

        if(section == 1)
        {
            position = new Vector3(-35, 25, 0);
        }
        else if(section == 2)
        {
            position = new Vector3(-25, 25, 0);
        }
        else if (section == 3)
        {
            position = new Vector3(-15, 25, 0);
        }
        else if (section == 4)
        {
            position = new Vector3(-5, 25, 0);
        }
        else if (section == 5)
        {
            position = new Vector3(5, 25, 0);
        }
        else if (section == 6)
        {
            position = new Vector3(15, 25, 0);
        }
        else if (section == 7)
        {
            position = new Vector3(25, 25, 0);
        }
        else if (section == 8)
        {
            position = new Vector3(35, 25, 0);
        }
        else if (section == 9)
        {
            position = new Vector3(-35, 15, 0);
        }
        else if (section == 10)
        {
            position = new Vector3(-25, 15, 0);
        }
        else if (section == 11)
        {
            position = new Vector3(-15, 15, 0);
        }
        else if (section == 12)
        {
            position = new Vector3(-5, 15, 0);
        }
        else if (section == 13)
        {
            position = new Vector3(5, 15, 0);
        }
        else if (section == 14)
        {
            position = new Vector3(15, 15, 0);
        }
        else if (section == 15)
        {
            position = new Vector3(25, 15, 0);
        }
        else if (section == 16)
        {
            position = new Vector3(35, 15, 0);
        }
        else if (section == 17)
        {
            position = new Vector3(-35, 5, 0);
        }
        else if (section == 18)
        {
            position = new Vector3(-25, 5, 0);
        }
        else if (section == 19)
        {
            position = new Vector3(-15, 5, 0);
        }
        else if (section == 20)
        {
            position = new Vector3(-5, 5, 0);
        }
        else if (section == 21)
        {
            position = new Vector3(5, 5, 0);
        }
        else if (section == 22)
        {
            position = new Vector3(15, 5, 0);
        }
        else if (section == 23)
        {
            position = new Vector3(25, 5, 0);
        }
        else if (section == 24)
        {
            position = new Vector3(35, 5, 0);
        }
        else if (section == 25)
        {
            position = new Vector3(-35, -5, 0);
        }
        else if (section == 26)
        {
            position = new Vector3(-25, -5, 0);
        }
        else if (section == 27)
        {
            position = new Vector3(-15, -5, 0);
        }
        else if (section == 28)
        {
            position = new Vector3(-5, -5, 0);
        }
        else if (section == 29)
        {
            position = new Vector3(5, -5, 0);
        }
        else if (section == 30)
        {
            position = new Vector3(15, -5, 0);
        }
        else if (section == 31)
        {
            position = new Vector3(25, -5, 0);
        }
        else if (section == 32)
        {
            position = new Vector3(35, -5, 0);
        }
        else if (section == 33)
        {
            position = new Vector3(-35, -15, 0);
        }
        else if (section == 34)
        {
            position = new Vector3(-25, -15, 0);
        }
        else if (section == 35)
        {
            position = new Vector3(-15,-15, 0);
        }
        else if (section == 36)
        {
            position = new Vector3(-5, -15, 0);
        }
        else if (section == 37)
        {
            position = new Vector3(5, -15, 0);
        }
        else if (section == 38)
        {
            position = new Vector3(15, -15, 0);
        }
        else if (section == 39)
        {
            position = new Vector3(25, -15, 0);
        }
        else if (section == 40)
        {
            position = new Vector3(35, -15, 0);
        }
        else if (section == 41)
        {
            position = new Vector3(-35, -25, 0);
        }
        else if (section == 42)
        {
            position = new Vector3(-25, -25, 0);
        }
        else if (section == 43)
        {
            position = new Vector3(-15, -25, 0);
        }
        else if (section == 44)
        {
            position = new Vector3(-5, -25, 0);
        }
        else if (section == 45)
        {
            position = new Vector3(5, -25, 0);
        }
        else if (section == 46)
        {
            position = new Vector3(15, -25, 0);
        }
        else if (section == 47)
        {
            position = new Vector3(25, -25, 0);
        }
        else if (section == 48)
        {
            position = new Vector3(35, -25, 0);
        }


        return position;
    }

    /*
     * BotEscape - Moves the bot towards the closest spawn point
     * Once arriving at the spawn point:
     *      sets escaped to true
     *      calls the die function
     */
    void BotEscape()
    {
        //Debug.Log("Trying to escape to: " + closestSpawn.x + ", " + closestSpawn.y + ", " + closestSpawn.z);
       
        transform.position = Vector3.MoveTowards(transform.position, closestSpawn, Time.deltaTime * speed);

        if (Vector3.Distance(closestSpawn, transform.position) < distanceToWaypoint)
        {
            Die();
        }
    }

    public void setGAGenome(List<BotData> botDataList, int botNumber)
    {

        for (int i = 0; i < GENOME_SIZE; ++i)
        {
            genome[i] = botDataList.ElementAt(botNumber).getGenomeElement(i);
        }

    }

    public int getDamageDealt()
    {
        return damageDealtToPlayer;
    }

    public float getTimeAlive()
    {
        return timeAlive;
    }

    public bool isEscaped()
    {
        return escaped;
    }

    public void setWaveNumber(int wave)
    {
        waveNumber = wave;
    }

}
