using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform firePoint;
    
    public ShipController shipController;
    void Start()
    {
        // shipController = GetComponent<ShipController>();
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            EnableLaser();
        }
        if(Input.GetKey(KeyCode.J))
        {
            
        }
        if(Input.GetKeyUp(KeyCode.J))
        {
            DisableLaser();
        }
        
    }
    
    void EnableLaser(){
        lineRenderer.enabled = true;
    }
    
    void UpdateLaser(){
        
    }
    void DisableLaser(){
        lineRenderer.enabled = false;
    }
    
    void OnTriggerEnter2D(Collider2D other){
        HandleTrigger(other);
    }
    
    void OnTriggerStay2D(Collider2D other) {
        HandleTrigger(other);
    }
    
    void HandleTrigger(Collider2D other){
        if(!lineRenderer.enabled){
            return;
        }
        if(other.gameObject.CompareTag("Asteroid"))
        {
            shipController.UpdateScore(Setting.asteroidScore);
            Destroy(other.gameObject);
        }
    }
    
    // void OnCollisionEnter2D(Collision2D other){
    //     Debug.Log("Hit asteroid with collision");
    //     if(other.gameObject.CompareTag("Asteroid"))
    //     {
    //         // Destroy(other.gameObject);
    //         Debug.Log("Hit asteroid with collision");  
    //     }
    // }
}
