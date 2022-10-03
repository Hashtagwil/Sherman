using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawnManager : MonoBehaviour
{
    

    [Tooltip("List of powerUp prefabs available to spawn, drag them here")]
    public GameObject[] spawningPool;
    [Tooltip("Start spawning power up randomly after this Delay")]
    public float startDelay = 2.0f;
    public float spawnIntervalMin = 1.0f;
    public float spawnIntervalMax = 5.0f;

    // Limit boundary, should be set based on the position of the map.
    
    [Tooltip("If powerUp position is over -1 -> 0..1..2..etc, it won't be displayed on the map. It will be under the Terrain layer.")]
    public float positionZ = -1;

    public GameObject topBoundary;
    public GameObject bottomBoundary;
    public GameObject leftBoundary;
    public GameObject rightBoundary;

    private float topYBoundary;
    private float bottomYBoundary;
    private float leftXBoundary;
    private float rightXBoundary;
    private float offset = 0.5f;
    private int maxAttemp = 100;
    private float powerUpRadius = 0.5f;

    public NetGame netGame;

    private int spawnId = 0;

    void Start()
    {
        SetMapBoundaries();
    }

    public void StartSpawnRandomPowerUps()
    {
        Invoke("SpawnRandomPowerUps", startDelay);
    }

    void SpawnRandomPowerUps()
    {
        //Random power ups and delay interval
        int index = Random.Range(0, spawningPool.Length);
        float nextInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);

        netGame.SpawnPowerUpClientRpc(index, GetRandomPosition());

        Invoke("SpawnRandomPowerUps", nextInterval);
    }

    public void InstantiatePowerUp(int index, Vector3 position)
    {
        GameObject powerUp = Instantiate(spawningPool[index], position, spawningPool[index].transform.rotation);
        powerUp.name = powerUp.name + spawnId;
        spawnId++;
    }


    // Used a for loop because it would attemp indefinitely in a while loop or recursive method (tried it),
    // causing a stackOverFlow exception or outOfMemoryException
    Vector3 GetRandomPosition()
    {
        bool isOverlapping = true;
        float posX = 1.0f; // Default position XY if maxAttemp is reach
        float posY = 1.0f;

        for (int attemp = 0; attemp < maxAttemp && isOverlapping; attemp++) 
        {
            posX = Random.Range(leftXBoundary, rightXBoundary);
            posY = Random.Range(bottomYBoundary, topYBoundary);
            Vector2 position = new Vector2(posX, posY);
            isOverlapping = (Physics2D.OverlapCircleAll(position, powerUpRadius).Length != 0) ? true : false;
        }

        return new Vector2(posX, posY); 
    }

    void SetMapBoundaries()
    {
        // The x and y value of boundaries are the center value of the gameObject of 1unit, so an offset of 0.5unit is needed  
        topYBoundary = topBoundary.transform.position.y - offset;
        bottomYBoundary = bottomBoundary.transform.position.y + offset;
        leftXBoundary = leftBoundary.transform.position.x + offset;
        rightXBoundary = rightBoundary.transform.position.x - offset;
    }
}
