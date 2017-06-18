using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject[,] tiles;
    public const int WIDTH = 10;
    public const int HEIGHT = 10;

    ObjectContainer objContainer;
    public GameObject groundTile;
    public GameObject wallTile;
    public GameObject doorPrefab;
    public GameObject keyPrefab;
    GameObject player;


    void Start()
    {
        objContainer = GameObject.Find("ObjectContainer").GetComponent<ObjectContainer>();
        player = GameObject.Find("Player");
        GenerateLevel();
    }


    void Update()
    {

    }


    void GenerateLevel()
    {
        tiles = new GameObject[WIDTH, HEIGHT];
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                GameObject newTile;
                float nr = Random.Range(0f, 10f);
                if (nr < 9f)
                {
                    newTile = Instantiate(groundTile, new Vector3(x * 1f, y * 1f, 0), Quaternion.identity) as GameObject;
                }
                else
                {
                    newTile = Instantiate(wallTile, new Vector3(x * 1f, y * 1f, 0), Quaternion.identity) as GameObject;
                }
                tiles[x, y] = newTile;
            }
        }
        AdjustSpritesAndHitboxes();


        PlaceDoor(new Vector2(5, 5), Color.red);
        PlaceKey(new Vector2(0, 1), Color.red);
        PlaceKey(new Vector2(0, 5), Color.yellow);
    }

    void AdjustSpritesAndHitboxes()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                GameObject currTile = tiles[x, y];
                if (currTile.tag == "Block")
                {
                    WallTile wallScript = currTile.GetComponent<WallTile>();
                    wallScript.player = player;
                    wallScript.AdjustSpriteAndHitbox(tiles, new Vector2(x,y));
                }

            }
        }
    }


    void PlaceDoor(Vector2 pos, Color color)
    {
        GameObject newDoor = Instantiate(doorPrefab, pos, Quaternion.identity) as GameObject;
        LockedDoorScript doorScript = newDoor.GetComponent<LockedDoorScript>();
        doorScript.player = player;
        doorScript.SetColor(color);
        doorScript.AdjustSprite(tiles, pos);
    }


    void PlaceKey(Vector2 pos, Color color)
    {
        GameObject newKey = Instantiate(keyPrefab, pos, Quaternion.identity) as GameObject;
        KeyScript keyScript = newKey.GetComponent<KeyScript>();
        keyScript.player = player;
        keyScript.SetColor(color);

        //doorScript.SetColor(color);
        // doorScript.AdjustSprite(tiles, pos);
    }
}
