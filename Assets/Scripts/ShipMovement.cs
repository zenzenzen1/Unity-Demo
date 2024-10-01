using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    [SerializeField] protected Vector3 targetPosition;
    
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
        Vector2 movement = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
        if(Input.GetKey(KeyCode.S)){
            transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
        }
        
        // LookAtTarget();
    }
    
    protected virtual void LookAtTarget(){
        Vector3 diff = transform.position - transform.parent.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.parent.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }
    
    
}
