using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float turningSpeed = 90f;
    private ParticleSystem dustParticles;
    private Rigidbody2D rb;
    private GameObject scoreTextGO;
    public float torque = 4;
    public float jumpForce = 10f;
    public float speedMultiplier = 2f;
    public float slowMultiplier = 0.15f;
    private bool isGrounded = false;
    public GameObject GameManagerGO;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dustParticles = transform.Find("Dust Particles").GetComponent<ParticleSystem>();
        scoreTextGO = GameObject.FindGameObjectWithTag("ScoreTextTag");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            // Rotate object to the left
            rb.AddTorque(torque);
        }
        if (Input.GetKey(KeyCode.D))
        {
            // Rotate object to the right
            rb.AddTorque(-torque);

        }
        if (Input.GetKey(KeyCode.W) && isGrounded)
        {
            rb.velocity *= speedMultiplier;
        }
        // Slow down
        if (Input.GetKey(KeyCode.S) && isGrounded)
        {
            rb.velocity *= slowMultiplier;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Jumping");
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            dustParticles.Play();
        }
        
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
            dustParticles.Stop();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BonusItem"))
        {
            scoreTextGO.GetComponent<GameScore>().Score += 20;
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Finish"))
        {
            GameManagerGO.GetComponent<GameManager>().SetGameManagerState(GameManager.GameManagerState.Victory);
            gameObject.SetActive(false);
        }
    }
}
