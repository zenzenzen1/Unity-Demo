using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{

    private float MinX, MaxX, MinY, MaxY;
    
    // private Vector2 pos;
    
    [Header("Asteroid")]
    [SerializeField] readonly GameObject[] asteroids;

    void Start()
    {
        SetMinAndMax();
        SpawnObjects();
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
        int NumberOfObj = Random.Range(0, asteroids.Length);
        var position = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));
        GameObject obj = Instantiate(asteroids[NumberOfObj], position, Quaternion.identity);
        obj.transform.parent = transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
