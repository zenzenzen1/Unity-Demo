using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject instructionPanel;
    public GameObject aboutUsPanel;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Play(){
        SceneManager.LoadScene("PlayScene", LoadSceneMode.Single);
        
    }
    
    public void CloseInstructionButton(){
        instructionPanel.SetActive(false);
        menuPanel.SetActive(true);
        aboutUsPanel.SetActive(false);
    }
    
    public void ShowInstructionButton(){
        instructionPanel.SetActive(true);
        menuPanel.SetActive(false);
        aboutUsPanel.SetActive(false);
    }
    
    public void AboutButton(){
        aboutUsPanel.SetActive(true);
        menuPanel.SetActive(false);
        instructionPanel.SetActive(false);
    }
    
    public void CloseAboutButton(){
        aboutUsPanel.SetActive(false);
        menuPanel.SetActive(true);
        instructionPanel.SetActive(false);
    }
    
    public void BackToMenu()
    {
        Debug.Log("Back to menu");
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }
    
    public void Quit(){
        GameObject.FindObjectOfType<ShipCollision>().GetComponent<ShipCollision>().ChangeToGameOverScene();
    }
}
