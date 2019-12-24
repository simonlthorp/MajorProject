using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour
{

    public GameObject firePoint;
    public List<GameObject> vfx = new List<GameObject>();
    public faceMouse faceMouse;
    public GameObject rotationObject;

    private GameObject effectToSpawn;
    private float timeToFire = 0;
    private AudioSource shotSFX;

    // Start is called before the first frame update
    void Start()
    {
        effectToSpawn = vfx[0];
        shotSFX = GameObject.Find("CoreGame").GetComponent<Game>().shotSFX;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire1") && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / effectToSpawn.GetComponent<ProjectileMove>().fireRate;
            spawnVFX(); //Spawns the projectile

            shotSFX.Play();  //Plays the particle system 
        }

    }

    void spawnVFX()
    {
        GameObject vfx;
        
        if(firePoint != null)
        {
            vfx = Instantiate(effectToSpawn, firePoint.transform.position, firePoint.transform.rotation);
            
        }
        else
        {
            Debug.Log("Error: Fire Point does not exist");
        }
    }

}
