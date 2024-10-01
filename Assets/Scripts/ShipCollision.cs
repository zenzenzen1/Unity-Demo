using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollision : MonoBehaviour
{
    public GameObject Ship;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Asteroid"))
        {
            Destroy(other.gameObject);
            Destroy(Ship);
        }
        else if(other.gameObject.CompareTag("Star"))
        {
            Destroy(other.gameObject);
        }
    }
}
