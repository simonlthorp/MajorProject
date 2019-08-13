using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{

    public float speed;
    public float fireRate;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if(muzzlePrefab != null)
        {
            GameObject muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            ParticleSystem psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();

            if(psMuzzle != null)
            {
                Destroy(muzzleVFX, psMuzzle.main.duration);
            }
            else
            {
                ParticleSystem psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }

        }
        Destroy(gameObject, 3.0f); //Destroy the bullet after 3 seconds
    }

    // Update is called once per frame
    void Update()
    {
        
        if(speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("Error: Projectile does not have a speed");
        }

    }

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.tag == "bot")
        {

            speed = 0;

            ContactPoint cp = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, cp.normal);
            Vector3 pos = cp.point;

            if (hitPrefab != null)
            {
                GameObject hitVFX = Instantiate(hitPrefab, pos, rot);
                ParticleSystem psHit = hitVFX.GetComponent<ParticleSystem>();

                if (psHit != null)
                {
                    Destroy(hitVFX, psHit.main.duration);
                }
                else
                {
                    ParticleSystem psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitVFX, psChild.main.duration);
                }
            }

            Destroy(gameObject);

        }
        
    }

}
