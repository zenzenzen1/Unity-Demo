using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour
{

    private float MinX, MaxX, MinY, MaxY;
    
    [SerializeField]
    private float speed = 3f;
    
    [SerializeField] float timer = 0f;
    
    // private Vector2 pos;
    
    [Header("Asteroid")]
    [SerializeField] GameObject[] stars;
    
    [SerializeField] float spawnTime = 4f;

    void Start()
    {
        SetMinAndMax();
    }

    private void SetMinAndMax()
    {
        Vector3 bounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        MinX = -bounds.x;
        MaxX = bounds.x;
        MinY = -bounds.y;
        MaxY = bounds.y;
    }


    private void SpawnObjects()
    {
        timer += Time.deltaTime;
        if(timer >= spawnTime){
            Spawn();
            timer = 0;
        }
    }
    
    private void Spawn(){
        int NumberOfObj = Random.Range(0, stars.Length);
        var position = new Vector2(Random.Range(MinX, MaxX), MaxY);
        GameObject obj = Instantiate(stars[NumberOfObj], position, Quaternion.identity);
        obj.transform.parent = transform;
    }
    
    // Update is called once per frame
    void Update()
    {
        SpawnObjects();
        MoveDown();
    }
    
    
    void MoveDown(){
        transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
    }
}