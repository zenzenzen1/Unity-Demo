using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidbody;
    
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        myRigidbody.velocity = new Vector2 (moveSpeed, 0);
        // myRigidbody.velocity = new Vector2 (0, moveSpeed / 2);
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        moveSpeed = -moveSpeed;
        FlipEnemyFacing();
    }

    void FlipEnemyFacing()
    {
        transform.localScale = new Vector2 (-Mathf.Sign(myRigidbody.velocity.x), 1f);
    }
    
    public void Flip(float angle, string direction){
        transform.Rotate(0, angle, 0);
        if(direction == "horizontal"){
            moveSpeed = -moveSpeed;
        }
    }
}
