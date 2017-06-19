using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject[,] rooms;
    public const int STARTING_WIDTH = 100;
    public const int STARTING_HEIGHT = 100;
    public const float MIN_SPLIT_PERCENTAGE = 0.33f;
    public const float MAX_SPLIT_PERCENTAGE = 1f - MIN_SPLIT_PERCENTAGE;

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


    void GenerateRooms()
    {
        RoomContainer mainContainer = new RoomContainer(0, 0, STARTING_WIDTH, STARTING_HEIGHT);
        mainContainer.Split(true);
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

    public class RoomContainer
    {
        // (x, y) indicates top left corner
        public int x;
        public int y;
        public int width;
        public int height;
        public RoomContainer l_child;
        public RoomContainer r_child;

        public RoomContainer(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void Split(bool horizontal)
        {
            if (horizontal)
            {
                int split_x = Mathf.RoundToInt(FindRandomSplit(width));

                int left_w = split_x - this.x;
                int right_w = this.width - left_w;

                l_child = new RoomContainer(this.x, this.y, left_w, this.height);
                r_child = new RoomContainer(split_x, this.y, right_w, this.height);
            }
            else
            {
                int split_y = Mathf.RoundToInt(FindRandomSplit(height));

                int left_h = split_y - this.y;
                int right_h = this.height - left_h;

                l_child = new RoomContainer(this.x, this.y, this.width, left_h);
                r_child = new RoomContainer(this.x, split_y, this.width, right_h);
            }
        }

        public float FindRandomSplit(int splittingLine)
        {
            float min = MIN_SPLIT_PERCENTAGE * splittingLine;
            float max = MAX_SPLIT_PERCENTAGE * splittingLine;
            return Random.Range(min, max);
        }

    }
}
