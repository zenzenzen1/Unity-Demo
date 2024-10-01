using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
    [SerializeField]
    private int score = 0;
    
    void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        // Debug.Log(scoreText.name);
        scoreText.text = "Score: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void UpdateScore(){
        score += 1;
        scoreText.text = "Score: " + score;
    }
}
