using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrickForScore : MonoBehaviour
{
    GameObject scoreTextGO;
    private Rigidbody2D rb;
    public int backFlips = 0;
    float flipTime = 0f;
    public int TotalFlipCount = 0;
    public bool gc = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scoreTextGO = GameObject.FindGameObjectWithTag("ScoreTextTag");
    }

    void Update()
    {
        // Only track rotation when the object is airborne
        CheckForBackflip();
    }
    private void CheckForBackflip()
    {
        if (!gc)
        {

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                //backflip
                flipTime += 1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                //front flip
                flipTime += 1;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            flipTime = 0;
        }
        if (flipTime >= 50)
        {
            TotalFlipCount += 1;
            scoreTextGO.GetComponent<GameScore>().Score += 200;
            flipTime = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
           /* Vector3 normal = collision.GetContact(0).normal;
            if(normal == Vector3.up)
            {*/
                gc = true;
           // }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            gc = false;
        }
    }
}
