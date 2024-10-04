using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{

    private float MinX, MaxX, MinY, MaxY;
    
    [SerializeField] float timer = 0f;
    // private Vector2 pos;
    
    [SerializeField] float speed = 3f;
    
    [Header("Asteroid")]
    [SerializeField] GameObject[] asteroids;
    
    [SerializeField] float spawnTime = 1f;
    
    

    void Start()
    {
        SetMinAndMax();
        Spawn();
        var d = new Dictionary<string, string>
        {
            { "a", "b" },
            { "a", "c" }
        };
        Debug.Log(d["a"]);
    }
    void AsteroidMovement(){
        transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
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
        int NumberOfObj = Random.Range(0, asteroids.Length);
        var position = new Vector2(Random.Range(MinX, MaxX), MaxY);
        GameObject obj = Instantiate(asteroids[NumberOfObj], position, Quaternion.identity);
        obj.transform.parent = transform;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnObjects();
        AsteroidMovement();
    }
}
