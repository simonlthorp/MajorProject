using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bot : MonoBehaviour
{

    public const int GENOME_SIZE = 20;
    private const int NUMBER_OF_SECTIONS = 165;

    private float spawnTime;
    private float deathTime;
    private int damageDealtToPlayer;
    private float timeAlive;
    private bool escaped = false;
    private Vector3 botPosition;
    private bool isElite = false;

    private int[] genome = new int[GENOME_SIZE];
    public int genomePositition = 0;

    //Movement Variables
    public float speed;
    public float distanceToWaypoint;
    public bool escaping = false;
    public Vector3 closestSpawn;
    private Vector3 escapePosition;
    private bool foundClosest = false;
    private bool colliding = false;
    private float collisionTime;

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
    private int spawnPointRecord;
    private int waveNumber;
    private int botNumber;

    private BotData botData;

    private bool isDamaging = false;
    private float elapsedTime;
    private const float DAMAGE_INTERVAL = 0.25f;

    private GameObject CoreGame;
    private AudioSource raySFX;

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

        raySFX = CoreGame.GetComponent<Game>().raySXF;

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
                genome[i] = Random.Range(1, NUMBER_OF_SECTIONS);
            }
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!escaping)
        {

            if(genomePositition >= genome.Length)
            {
                MoveBot(0);//escape
            }
            else
            {
                MoveBot(genome[genomePositition]);
            }
            
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
        if (!escaping)
        {
            closestSpawn = closest;
            spawnPointRecord = spawnPointNumber;
        }
        else
        {
            escapePosition = closest;
        }
        

        return closest;
    }

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

        spawnPointRecord = spawnPointNumber;

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
            ps.Play();  //Pulse particle system
            
            isDamaging = true;

            raySFX.Play(); //Ray particle system

            //Marks bot as colliding
            //Sets the current time as collision time
            if (!colliding)
            {
                colliding = true;
                collisionTime = Time.time;
            }

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

        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "planet")
        {
            //Allows a bot to collide with the planet for 1 second
            //After 1 second, the genomePosition of the bot is incremented by 1
            if (colliding && collisionTime + 1.0f < Time.time)
            {
                colliding = false;
                genomePositition++;
            }

            //Marks bot as colliding
            //Sets the current time as collision time
            if (!colliding)
            {
                colliding = true;
                collisionTime = Time.time;
            }
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

            if (colliding)
            {
                colliding = false;
            }

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
     *      - Records the data from the bot
     *      - Creates a BotData object using the genome from the bot
     *          - Adds the BotData object to a list corresponding to the spawnpoint of the bot
     *      - Destroy the bot
     */
    private void Die()
    {
        Recorder recorder = GameObject.Find("CoreGame").GetComponent<Recorder>();

        deathTime = Time.fixedTime;

        timeAlive = deathTime - spawnTime;

        int didEscape = 0;

        if (escaped)
        {
            didEscape = 1;
        }

        CoreGame.GetComponent<Game>().DecrementNumberOfBots();

        botData = new BotData(CoreGame.GetComponent<GeneticAlgorithm>().FitnessFunction(this), genome);

        recorder.recordWave(CoreGame.GetComponent<Game>().GetWaveNumber(), spawnPointRecord, botNumber, damageDealtToPlayer, timeAlive, didEscape);

        if (spawnPointRecord == 1)
        {
            CoreGame.GetComponent<GeneticAlgorithm>().botListSP1.Add(botData);
        }

        if (spawnPointRecord == 2)
        {
            CoreGame.GetComponent<GeneticAlgorithm>().botListSP2.Add(botData);
        }

        if (spawnPointRecord == 3)
        {
            CoreGame.GetComponent<GeneticAlgorithm>().botListSP3.Add(botData);
        }

        if (spawnPointRecord == 4)
        {
            CoreGame.GetComponent<GeneticAlgorithm>().botListSP4.Add(botData);
        }

        Destroy(gameObject);

    }

    /*
     * convertSection - Converts one of 165 sections of the screen to game coordinates
     * eg: input int 1 returns Vector3(-35,25,0)
     */
    private Vector3 convertSection(int section)
    {
        //Sections 1 to 48 converted into a position vector
        Vector3 position = new Vector3(0, 0, 0);

        /*
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
        */

        if (section == 1)
        {
            position = new Vector3(-35, 25, 0);
        }
        else if (section == 2)
        {
            position = new Vector3(-30, 25, 0);
        }
        else if (section == 3)
        {
            position = new Vector3(-25, 25, 0);
        }
        else if (section == 4)
        {
            position = new Vector3(-20, 25, 0);
        }
        else if (section == 5)
        {
            position = new Vector3(-15, 25, 0);
        }
        else if (section == 6)
        {
            position = new Vector3(-10, 25, 0);
        }
        else if (section == 7)
        {
            position = new Vector3(-5, 25, 0);
        }
        else if (section == 8)
        {
            position = new Vector3(0, 25, 0);
        }
        else if (section == 9)
        {
            position = new Vector3(5, 25, 0);
        }
        else if (section == 10)
        {
            position = new Vector3(10, 25, 0);
        }
        else if (section == 11)
        {
            position = new Vector3(15, 25, 0);
        }
        else if (section == 12)
        {
            position = new Vector3(20, 25, 0);
        }
        else if (section == 13)
        {
            position = new Vector3(25, 25, 0);
        }
        else if (section == 14)
        {
            position = new Vector3(30, 25, 0);
        }
        else if (section == 15)
        {
            position = new Vector3(35, 25, 0);
        }
        else if (section == 16)
        {
            position = new Vector3(-35, 20, 0);
        }
        else if (section == 17)
        {
            position = new Vector3(-30, 20, 0);
        }
        else if (section == 18)
        {
            position = new Vector3(-25, 20, 0);
        }
        else if (section == 19)
        {
            position = new Vector3(-20, 20, 0);
        }
        else if (section == 20)
        {
            position = new Vector3(-15, 20, 0);
        }
        else if (section == 21)
        {
            position = new Vector3(-10, 20, 0);
        }
        else if (section == 22)
        {
            position = new Vector3(-5, 20, 0);
        }
        else if (section == 23)
        {
            position = new Vector3(0, 20, 0);
        }
        else if (section == 24)
        {
            position = new Vector3(5, 20, 0);
        }
        else if (section == 25)
        {
            position = new Vector3(10, 20, 0);
        }
        else if (section == 26)
        {
            position = new Vector3(15, 20, 0);
        }
        else if (section == 27)
        {
            position = new Vector3(20, 20, 0);
        }
        else if (section == 28)
        {
            position = new Vector3(25, 20, 0);
        }
        else if (section == 29)
        {
            position = new Vector3(30, 20, 0);
        }
        else if (section == 30)
        {
            position = new Vector3(35, 20, 0);
        }
        else if (section == 31)
        {
            position = new Vector3(-35, 15, 0);
        }
        else if (section == 32)
        {
            position = new Vector3(-30, 15, 0);
        }
        else if (section == 33)
        {
            position = new Vector3(-25, 15, 0);
        }
        else if (section == 34)
        {
            position = new Vector3(-20, 15, 0);
        }
        else if (section == 35)
        {
            position = new Vector3(-15, 15, 0);
        }
        else if (section == 36)
        {
            position = new Vector3(-10, 15, 0);
        }
        else if (section == 37)
        {
            position = new Vector3(-5, 15, 0);
        }
        else if (section == 38)
        {
            position = new Vector3(0, 15, 0);
        }
        else if (section == 39)
        {
            position = new Vector3(5, 15, 0);
        }
        else if (section == 40)
        {
            position = new Vector3(10, 15, 0);
        }
        else if (section == 41)
        {
            position = new Vector3(15, 15, 0);
        }
        else if (section == 42)
        {
            position = new Vector3(20, 15, 0);
        }
        else if (section == 43)
        {
            position = new Vector3(25, 15, 0);
        }
        else if (section == 44)
        {
            position = new Vector3(30, 15, 0);
        }
        else if (section == 45)
        {
            position = new Vector3(35, 15, 0);
        }
        else if (section == 46)
        {
            position = new Vector3(-35, 10, 0);
        }
        else if (section == 47)
        {
            position = new Vector3(-30, 10, 0);
        }
        else if (section == 48)
        {
            position = new Vector3(-25, 10, 0);
        }
        else if (section == 49)
        {
            position = new Vector3(-20, 10, 0);
        }
        else if (section == 50)
        {
            position = new Vector3(-15, 10, 0);
        }
        else if (section == 51)
        {
            position = new Vector3(-10, 10, 0);
        }
        else if (section == 52)
        {
            position = new Vector3(-5, 10, 0);
        }
        else if (section == 53)
        {
            position = new Vector3(0, 10, 0);
        }
        else if (section == 54)
        {
            position = new Vector3(5, 10, 0);
        }
        else if (section == 55)
        {
            position = new Vector3(10, 10, 0);
        }
        else if (section == 56)
        {
            position = new Vector3(15, 10, 0);
        }
        else if (section == 57)
        {
            position = new Vector3(20, 10, 0);
        }
        else if (section == 58)
        {
            position = new Vector3(25, 10, 0);
        }
        else if (section == 59)
        {
            position = new Vector3(30, 10, 0);
        }
        else if (section == 60)
        {
            position = new Vector3(35, 10, 0);
        }
        else if (section == 61)
        {
            position = new Vector3(-35, 5, 0);
        }
        else if (section == 62)
        {
            position = new Vector3(-30, 5, 0);
        }
        else if (section == 63)
        {
            position = new Vector3(-25, 5, 0);
        }
        else if (section == 64)
        {
            position = new Vector3(-20, 5, 0);
        }
        else if (section == 65)
        {
            position = new Vector3(-15, 5, 0);
        }
        else if (section == 66)
        {
            position = new Vector3(-10, 5, 0);
        }
        else if (section == 67)
        {
            position = new Vector3(-5, 5, 0);
        }
        else if (section == 68)
        {
            position = new Vector3(0, 5, 0);
        }
        else if (section == 69)
        {
            position = new Vector3(5, 5, 0);
        }
        else if (section == 70)
        {
            position = new Vector3(10, 5, 0);
        }
        else if (section == 71)
        {
            position = new Vector3(15, 5, 0);
        }
        else if (section == 72)
        {
            position = new Vector3(20, 5, 0);
        }
        else if (section == 73)
        {
            position = new Vector3(25, 5, 0);
        }
        else if (section == 74)
        {
            position = new Vector3(30, 5, 0);
        }
        else if (section == 75)
        {
            position = new Vector3(35, 5, 0);
        }
        else if (section == 76)
        {
            position = new Vector3(-35, 0, 0);
        }
        else if (section == 77)
        {
            position = new Vector3(-30, 0, 0);
        }
        else if (section == 78)
        {
            position = new Vector3(-25, 0, 0);
        }
        else if (section == 79)
        {
            position = new Vector3(-20, 0, 0);
        }
        else if (section == 80)
        {
            position = new Vector3(-15, 0, 0);
        }
        else if (section == 81)
        {
            position = new Vector3(-10, 0, 0);
        }
        else if (section == 82)
        {
            position = new Vector3(-5, 0, 0);
        }
        else if (section == 83)
        {
            position = new Vector3(0, 0, 0);
        }
        else if (section == 84)
        {
            position = new Vector3(5, 0, 0);
        }
        else if (section == 85)
        {
            position = new Vector3(10, 0, 0);
        }
        else if (section == 86)
        {
            position = new Vector3(15, 0, 0);
        }
        else if (section == 87)
        {
            position = new Vector3(20, 0, 0);
        }
        else if (section == 88)
        {
            position = new Vector3(25, 0, 0);
        }
        else if (section == 89)
        {
            position = new Vector3(30, 0, 0);
        }
        else if (section == 90)
        {
            position = new Vector3(35, 0, 0);
        }
        else if (section == 91)
        {
            position = new Vector3(-35, -5, 0);
        }
        else if (section == 92)
        {
            position = new Vector3(-30, -5, 0);
        }
        else if (section == 93)
        {
            position = new Vector3(-25, -5, 0);
        }
        else if (section == 94)
        {
            position = new Vector3(-20, -5, 0);
        }
        else if (section == 95)
        {
            position = new Vector3(-15, -5, 0);
        }
        else if (section == 96)
        {
            position = new Vector3(-10, -5, 0);
        }
        else if (section == 97)
        {
            position = new Vector3(-5, -5, 0);
        }
        else if (section == 98)
        {
            position = new Vector3(-0, -5, 0);
        }
        else if (section == 99)
        {
            position = new Vector3(5, -5, 0);
        }
        else if (section == 100)
        {
            position = new Vector3(10, -5, 0);
        }
        else if (section == 101)
        {
            position = new Vector3(15, -5, 0);
        }
        else if (section == 102)
        {
            position = new Vector3(20, -5, 0);
        }
        else if (section == 103)
        {
            position = new Vector3(25, -5, 0);
        }
        else if (section == 104)
        {
            position = new Vector3(30, -5, 0);
        }
        else if (section == 105)
        {
            position = new Vector3(35, -5, 0);
        }
        else if (section == 106)
        {
            position = new Vector3(-35, -10, 0);
        }
        else if (section == 107)
        {
            position = new Vector3(-30, -10, 0);
        }
        else if (section == 108)
        {
            position = new Vector3(-25, -10, 0);
        }
        else if (section == 109)
        {
            position = new Vector3(-20, -10, 0);
        }
        else if (section == 110)
        {
            position = new Vector3(-15, -10, 0);
        }
        else if (section == 111)
        {
            position = new Vector3(-10, -10, 0);
        }
        else if (section == 112)
        {
            position = new Vector3(-5, -10, 0);
        }
        else if (section == 113)
        {
            position = new Vector3(0, -10, 0);
        }
        else if (section == 114)
        {
            position = new Vector3(5, -10, 0);
        }
        else if (section == 115)
        {
            position = new Vector3(10, -10, 0);
        }
        else if (section == 116)
        {
            position = new Vector3(15, -10, 0);
        }
        else if (section == 117)
        {
            position = new Vector3(20, -10, 0);
        }
        else if (section == 118)
        {
            position = new Vector3(25, -10, 0);
        }
        else if (section == 119)
        {
            position = new Vector3(30, -10, 0);
        }
        else if (section == 120)
        {
            position = new Vector3(35, -10, 0);
        }
        else if (section == 121)
        {
            position = new Vector3(-35, -15, 0);
        }
        else if (section == 122)
        {
            position = new Vector3(-30, -15, 0);
        }
        else if (section == 123)
        {
            position = new Vector3(-25, -15, 0);
        }
        else if (section == 124)
        {
            position = new Vector3(-20, -15, 0);
        }
        else if (section == 125)
        {
            position = new Vector3(-15, -15, 0);
        }
        else if (section == 126)
        {
            position = new Vector3(-10, -15, 0);
        }
        else if (section == 127)
        {
            position = new Vector3(-5, -15, 0);
        }
        else if (section == 128)
        {
            position = new Vector3(0, -15, 0);
        }
        else if (section == 129)
        {
            position = new Vector3(5, -15, 0);
        }
        else if (section == 130)
        {
            position = new Vector3(10, -15, 0);
        }
        else if (section == 131)
        {
            position = new Vector3(15, -15, 0);
        }
        else if (section == 132)
        {
            position = new Vector3(20, -15, 0);
        }
        else if (section == 133)
        {
            position = new Vector3(25, -15, 0);
        }
        else if (section == 134)
        {
            position = new Vector3(30, -15, 0);
        }
        else if (section == 135)
        {
            position = new Vector3(35, -15, 0);
        }
        else if (section == 136)
        {
            position = new Vector3(-35, -20, 0);
        }
        else if (section == 137)
        {
            position = new Vector3(-30, -20, 0);
        }
        else if (section == 138)
        {
            position = new Vector3(-25, -20, 0);
        }
        else if (section == 139)
        {
            position = new Vector3(-20, -20, 0);
        }
        else if (section == 140)
        {
            position = new Vector3(-15, -20, 0);
        }
        else if (section == 141)
        {
            position = new Vector3(-10, -20, 0);
        }
        else if (section == 142)
        {
            position = new Vector3(-5, -20, 0);
        }
        else if (section == 143)
        {
            position = new Vector3(0, -20, 0);
        }
        else if (section == 144)
        {
            position = new Vector3(5, -20, 0);
        }
        else if (section == 145)
        {
            position = new Vector3(10, -20, 0);
        }
        else if (section == 146)
        {
            position = new Vector3(15, -20, 0);
        }
        else if (section == 147)
        {
            position = new Vector3(20, -20, 0);
        }
        else if (section == 148)
        {
            position = new Vector3(25, -20, 0);
        }
        else if (section == 149)
        {
            position = new Vector3(30, -20, 0);
        }
        else if (section == 150)
        {
            position = new Vector3(35, -20, 0);
        }
        else if (section == 151)
        {
            position = new Vector3(-35, -25, 0);
        }
        else if (section == 152)
        {
            position = new Vector3(-30, -25, 0);
        }
        else if (section == 153)
        {
            position = new Vector3(-25, -25, 0);
        }
        else if (section == 154)
        {
            position = new Vector3(-20, -25, 0);
        }
        else if (section == 155)
        {
            position = new Vector3(-15, -25, 0);
        }
        else if (section == 156)
        {
            position = new Vector3(-10, -25, 0);
        }
        else if (section == 157)
        {
            position = new Vector3(-5, -25, 0);
        }
        else if (section == 158)
        {
            position = new Vector3(0, -25, 0);
        }
        else if (section == 159)
        {
            position = new Vector3(5, -25, 0);
        }
        else if (section == 160)
        {
            position = new Vector3(10, -25, 0);
        }
        else if (section == 161)
        {
            position = new Vector3(15, -25, 0);
        }
        else if (section == 162)
        {
            position = new Vector3(20, -25, 0);
        }
        else if (section == 163)
        {
            position = new Vector3(25, -25, 0);
        }
        else if (section == 164)
        {
            position = new Vector3(30, -25, 0);
        }
        else if (section == 165)
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
        transform.position = Vector3.MoveTowards(transform.position, escapePosition, Time.deltaTime * speed);

        if (Vector3.Distance(escapePosition, transform.position) < distanceToWaypoint)
        {
            escaped = true;
            Die();
        }
    }

    public void setGAGenome(List<BotData> botDataList, int botNumber)
    {

        for (int i = 0; i < GENOME_SIZE; ++i)
        {
            genome[i] = botDataList.ElementAt(botNumber).getGenomeElement(i);
        }

        if (botDataList.ElementAt(botNumber).getElite())
        {
            isElite = true;
        }

        //Recorder recorder = GameObject.Find("CoreGame").GetComponent<Recorder>();
        //recorder.recordBot(genome);

    }

    public int getDamageDealt()
    {
        return damageDealtToPlayer;
    }

    public void setDamageDealt(int damage)
    {
        damageDealtToPlayer = damage;
    }

    public float getTimeAlive()
    {
        return timeAlive;
    }

    public void setTimeAlive(float time)
    {
        timeAlive = time;
    }

    public bool isEscaped()
    {
        return escaped;
    }

    public void setEscaped(bool didEscape)
    {
        escaped = didEscape;
    }

    public void setWaveNumber(int wave)
    {
        waveNumber = wave;
    }

    public void setBotNumber(int botNumber)
    {
        this.botNumber = botNumber;
    }

    public int[] getGenome()
    {
        return genome;
    }

}
