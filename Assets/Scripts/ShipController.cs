using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
    [SerializeField]
    private int score = 0;
    
    public int life = 3;
    
    public int stars = 0;
    
    void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        // Debug.Log(scoreText.name);
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Life: " + life;
        scoreText.text += "\nScore: " + score;
        scoreText.text += "\nNumber of Stars: " + stars;
        scoreText.text += "\n" + Setting.defaultDisplayName;
        // if(life <= 0){
        //     Destroy(gameObject);
        //     FindObjectOfType<ShipCollision>().GetComponent<ShipCollision>().ChangeToGameOverScene();
        // }
    }
    
    public void UpdateScore(int score){
        this.score += score;
        // scoreText.text = "Score: " + this.score + "\n" + Setting.defaultDisplayName;
    }
    
    public int GetScore(){
        return score;
    }
}
