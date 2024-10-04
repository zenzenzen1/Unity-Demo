using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    [SerializeField] protected Vector3 targetPosition;
    
    [SerializeField] float rotationSpeed = 432f;
    

    
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        Debug.Log(targetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        OnMove();
        // LookAtTarget();
    }
    
    private void OnMove(){
        
        // Vector3 movement = Vector2.zero;
        // if (Input.GetKey(KeyCode.A) && transform.position.x > Setting.minX)
        // {
        //     transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        // }
        // if (Input.GetKey(KeyCode.D) && transform.position.x < Setting.maxX)
        // {
        //     transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        // }
        // if (Input.GetKey(KeyCode.W) && transform.position.y < Setting.maxY)
        // {
        //     transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        // }
        // if(Input.GetKey(KeyCode.S) && transform.position.y > Setting.minY){
        //     transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
        // }
        Rotate();
    }
    
    protected virtual void Rotate(){
        // Vector3 diff = transform.position - transform.parent.position;
        // diff.Normalize();
        // float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        // transform.parent.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if(transform.position.x < Setting.minX && horizontal < Setting.centerX || transform.position.x > Setting.maxX && horizontal > Setting.centerX
            || transform.position.y < Setting.minY && vertical < Setting.centerY || transform.position.y > Setting.maxY && vertical > Setting.centerY
        ){
            return;
        }
        Debug.Log(horizontal + " " + vertical);
        Vector2 movementDirection = new Vector2(horizontal, vertical);
        
        float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        movementDirection.Normalize();
        transform.Translate(inputMagnitude * speed * Time.deltaTime * movementDirection, Space.World);
        if(movementDirection != Vector2.zero){
            // float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    
}
