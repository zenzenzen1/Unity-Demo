using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private TextMeshProUGUI text;
    private const string defaultText = "30_NguyenXuanTruong_Slot2";
    private const string Hazard_Tag = "Hazards", Exit_Tag = "Exit";
    [SerializeField] float runSpeed = 4f;
    [SerializeField] float jumpSpeed = 6.7f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;

    bool isAlive = true;

    void Start()
    {
        // text = GetComponent<TextMeshProUGUI>();
        text = GameObject.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        text.text = defaultText;
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        ClimbLadder();
        // Die();
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        Instantiate(bullet, gun.position, transform.rotation);
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if (value.isPressed)
        {
            // do stuff
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }
    private void Jump()
    {
        if (!isAlive || !myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }
        
        // Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
        // myRigidbody.velocity += jumpVelocityToAdd;
        myRigidbody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
        
        Vector2 movement = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            // movement.x = (transform.right * Time.deltaTime * -runSpeed).x;
            // movement += (Vector2)transform.position;
            // myRigidbody.MovePosition(movement);
            transform.position += new Vector3(-runSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            // movement.x = (transform.right * Time.deltaTime * runSpeed).x;
            // movement += (Vector2)transform.position;
            // myRigidbody.MovePosition(movement);
            transform.position += new Vector3(runSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            // movement.y = (transform.up * Time.deltaTime * runSpeed).y;
            // OnJump(new InputValue());
            Jump();
            
        
        }
        // if (Input.GetK(KeyCode.S))
        // {
        //     movement.y = (transform.up * Time.deltaTime * -runSpeed).y;
        // }


    }


    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void ClimbLadder()
    {
        if (!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }

        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag(Hazard_Tag))
        {
            gameObject.SetActive(false);
            text.text = "Game Over";
        }
        if (collider2D.CompareTag(Exit_Tag))
        {
            gameObject.SetActive(false);
            text.text = "Finish!";
        }
    }
}
