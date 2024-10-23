using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectPlayerCollision : MonoBehaviour
{
    public GameObject GameManagerGO;
    public GameObject playerGO;
    public AudioClip looseSound;
    public GameObject crashEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.gameObject.transform;
            var crash = Instantiate(crashEffect, player.position, player.rotation);
            crash.GetComponent<ParticleSystem>().Play();
            playerGO.SetActive(false);

            AudioSource.PlayClipAtPoint(looseSound, transform.position);
            //Change game mangaer state to game over 
            GameManagerGO.GetComponent<GameManager>().SetGameManagerState(GameManager.GameManagerState.GameOver);
        }
       
    }
}
