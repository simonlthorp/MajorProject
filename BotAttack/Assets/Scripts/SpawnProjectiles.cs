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

    // Start is called before the first frame update
    void Start()
    {
        effectToSpawn = vfx[0];
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire1") && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / effectToSpawn.GetComponent<ProjectileMove>().fireRate;
            spawnVFX();
            //Debug.Log("fire pressed");
        }

    }

    void spawnVFX()
    {
        GameObject vfx;
        
        if(firePoint != null)
        {
            vfx = Instantiate(effectToSpawn, firePoint.transform.position, firePoint.transform.rotation);
            

            if(faceMouse != null)
            {
                //vfx.transform.eulerAngles = new Vector3(00.0f, 00.0f, rotationObject.transform.rotation.z);
                //vfx.transform.localRotation = faceMouse.getRotation();

            }

        }
        else
        {
            Debug.Log("Error: Fire Point does not exist");
        }
    }

}
