using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollider : MonoBehaviour
{
    EnemyMovement enemyMovement;
    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered Triggered");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered Triggered");
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        enemyMovement = GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            Debug.Log("Entered Triggered");
            enemyMovement.Flip(180, "horizontal");
        }

    }
}
