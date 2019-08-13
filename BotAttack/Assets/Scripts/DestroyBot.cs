using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBot : MonoBehaviour
{

    public GameObject explodePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        //Debug.Log("Collision Detected");
        if(collision.gameObject.tag == "bullet")
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

            Destroy(gameObject);

        }
        

    }

}
