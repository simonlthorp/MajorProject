using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    private float spawnTime;
    private float deathTime;
    private int damageDealtToPlayer;
    private Vector3 botPosition;

    private int[] genome = new int[3];
    public int genomePositition = 0;
    private float genomeValue = 0;

    //Movement Variables
    public float speed;
    public float distanceToWaypoint;
    public bool escaping = false;
    public Vector3 closestSpawn;
    private bool foundClosest = false;

    //Attack Particle System
    public LineRenderer lr;
    public ParticleSystem ps;

    //Spawn Points
    public GameObject sp1;
    public GameObject sp2;
    public GameObject sp3;
    public GameObject sp4;

    // Start is called before the first frame update
    void Start()
    {

        sp1 = GameObject.Find("SpawnPoint01");
        sp2 = GameObject.Find("SpawnPoint02");
        sp3 = GameObject.Find("SpawnPoint03");
        sp4 = GameObject.Find("SpawnPoint04");

        for(int i = 0; i<genome.Length; ++i)
        {
            genome[i] = Random.Range(1, 48);
        }

        spawnTime = Time.fixedTime;

        ps.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!escaping)
        {
            Debug.Log("Moving");
            MoveBot(genome[genomePositition]);
        }
        else
        {
            if (!foundClosest)
            {
                findClosestSpawn();
            }
            BotEscape();
        }
        

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
            Debug.Log("Escaping");
            escaping = true;

        }

        if (!escaping)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveToPosition, Time.deltaTime * speed);
        }
        

    }

    private Vector3 findClosestSpawn()
    {
        Vector3 closest = sp1.transform.position;

        if(Vector3.Distance(transform.position, closest) > Vector3.Distance(transform.position, sp2.transform.position))
        {
            closest = sp2.transform.position;
        }

        if (Vector3.Distance(transform.position, closest) > Vector3.Distance(transform.position, sp3.transform.position))
        {
            closest = sp3.transform.position;
        }

        if (Vector3.Distance(transform.position, closest) > Vector3.Distance(transform.position, sp4.transform.position))
        {
            closest = sp4.transform.position;
        }

        foundClosest = true;

        closestSpawn = closest;

        return closest;
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

            ///TODO - Damage player

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

        }
    }

    private void damagePlayer()
    {
        genomeValue += 1;
    }

    private void die()
    {
        deathTime = Time.fixedTime;

        genomeValue += deathTime - spawnTime;

    }

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

    void BotEscape()
    {
        Debug.Log("Trying to escape to: " + closestSpawn.x + ", " + closestSpawn.y + ", " + closestSpawn.z);
       
        transform.position = Vector3.MoveTowards(transform.position, closestSpawn, Time.deltaTime * speed);

        if (Vector3.Distance(closestSpawn, transform.position) < distanceToWaypoint)
        {
            Destroy(gameObject);
        }
    }

}
