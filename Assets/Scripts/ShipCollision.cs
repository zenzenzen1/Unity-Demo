using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipCollision : MonoBehaviour
{
    public GameObject Ship;
    private ShipController shipController;
    
    void Start()
    {
        shipController = Ship.GetComponent<ShipController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Asteroid"))
        {
            if(Ship.GetComponent<ShipController>().life > 1)
            {
                Ship.GetComponent<ShipController>().life -= 1;
            }
            else{
                Destroy(gameObject);
                ChangeToGameOverScene();
            }
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("Star"))
        {
            Ship.GetComponent<ShipController>().UpdateScore(Setting.starScore);
            shipController.stars += 1;
            Destroy(other.gameObject);
        }
    }
    
    public void ChangeToGameOverScene(){
        string text = "Game Over\nScore: " + Ship.GetComponent<ShipController>().GetScore();
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
        
        
        // var scoreText = GameObject.Find("GameOverScoreText").GetComponent<TextMeshProUGUI>();
        // scoreText.text = text;
        
        // var scoreText = GameObject.FindGameObjectWithTag("GameOverScoreText").GetComponent<TextMeshProUGUI>();
        // Debug.Log(scoreText.name);
        
        PlayerPrefs.SetInt("score", Ship.GetComponent<ShipController>().GetScore());
    }
}
