using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoneSpawnerScript : MonoBehaviour 
{

    GameObject player;
    public GameObject stoneShadow;
    bool spawnStones;


    float stoneSpawnTimer;
    float stoneSpawnDelay = 0.43f;
    bool canSpawn;

    int stoneAmount = 3;

    List<GameObject> tiles = new List<GameObject>();

    void OnEnable()
    {
        player = GameObject.Find("Player");
        spawnStones = true;
        canSpawn = true;

        GameObject tileHolder = GameObject.Find("Level");
        int children = tileHolder.transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            GameObject tile = tileHolder.transform.GetChild(i).gameObject;
            if (tile.tag == "Ground")
            {
                tiles.Add(tile);
            }
        }

    }

    void OnDisable()
    {
        spawnStones = false;
        canSpawn = false;
    }



    void Update()
    {
        if (spawnStones)
        {
            if (canSpawn)
            {
                for (int i = 0; i < stoneAmount; i++)
                {
                    int randomTileIndex = Random.Range(0, tiles.Count);
                    Instantiate(stoneShadow, new Vector3(tiles[randomTileIndex].transform.position.x, tiles[randomTileIndex].transform.position.y, player.transform.position.z), Quaternion.identity);
                    canSpawn = false;
                }
                
            }
            stoneSpawnTimer += Time.deltaTime;
            if (stoneSpawnTimer >= stoneSpawnDelay)
            {
                stoneSpawnTimer = 0;
                canSpawn = true;
            }
        }
    }
}
