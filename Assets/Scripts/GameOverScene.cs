using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scene1 : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    
    public void Awake(){
        // var v = FindFirstObjectByType<ShipController>().GetScore().ToString();
        var score = PlayerPrefs.GetInt("score");
        scoreText.text = "Score: " + score;
        scoreText.text += "\n" + Setting.defaultDisplayName;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
