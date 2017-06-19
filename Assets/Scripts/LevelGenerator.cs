using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject[,] rooms;
    public const int STARTING_WIDTH = 100;
    public const int STARTING_HEIGHT = 100;
    public const float MIN_SPLIT_PERCENTAGE = 0.2f;
    public const float MAX_SPLIT_PERCENTAGE = 1f - MIN_SPLIT_PERCENTAGE;

    ObjectContainer objContainer;
    public GameObject groundTile;
    public GameObject wallTile;
    public GameObject doorPrefab;
    public GameObject keyPrefab;
    GameObject player;
	CaveGenerator caveGenerator;

    void Start()
    {
        objContainer = GameObject.Find("ObjectContainer").GetComponent<ObjectContainer>();
        player = GameObject.Find("Player");
		caveGenerator = new CaveGenerator(45);
        GenerateRoomContainers();
    }


    void Update()
    {

    }


    void GenerateRoomContainers()
    {
        RoomContainer mainContainer = new RoomContainer(0, 0, STARTING_WIDTH, STARTING_HEIGHT, 0);
        int maxTreeDepth = 3;
        mainContainer.Split(true, maxTreeDepth);
    }


    void PlaceDoor(Vector2 pos, Color color)
    {
        GameObject newDoor = Instantiate(doorPrefab, pos, Quaternion.identity) as GameObject;
        LockedDoorScript doorScript = newDoor.GetComponent<LockedDoorScript>();
        doorScript.player = player;
        doorScript.SetColor(color);

       // doorScript.AdjustSprite(tiles, pos);
    }


    void PlaceKey(Vector2 pos, Color color)
    {
        GameObject newKey = Instantiate(keyPrefab, pos, Quaternion.identity) as GameObject;
        KeyScript keyScript = newKey.GetComponent<KeyScript>();
        keyScript.player = player;
        keyScript.SetColor(color);

		// keyScript.AdjustSprite(tiles, pos);
	}


	public class RoomContainer
    {
        // (x, y) indicates top left corner
        public int x;
        public int y;
        public int width;
        public int height;

        public int treeDepth;
        public RoomContainer l_child;
        public RoomContainer r_child;

        public bool isSplitHorizontally;

        public RoomContainer(int x, int y, int width, int height, int treeDepth)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.treeDepth = treeDepth;
            PrintRoomContainer();
        }

        public void Split(bool horizontal, int maxTreeDepth)
        {
            int newTreeDepth = this.treeDepth + 1;
            this.isSplitHorizontally = horizontal;

            // create two new roomcontainers
            if (horizontal)
            {
                int split_x = this.x + Mathf.RoundToInt(FindRandomSplit(this.width));

                int left_w = split_x - this.x;
                int right_w = this.width - left_w;

                l_child = new RoomContainer(this.x, this.y, left_w, this.height, newTreeDepth);
                r_child = new RoomContainer(split_x, this.y, right_w, this.height, newTreeDepth);
            }
            else
            {
                int split_y = this.y + Mathf.RoundToInt(FindRandomSplit(this.height));

                int left_h = split_y - this.y;
                int right_h = this.height - left_h;

                l_child = new RoomContainer(this.x, this.y, this.width, left_h, newTreeDepth);
                r_child = new RoomContainer(this.x, split_y, this.width, right_h, newTreeDepth);
            }

            // recursively create more sub rooms in newly created rooms if maxTreeDepth hasn't been reached yet
            // if this room was split horizontally, split the next rooms vertically (and vice versa)
            if (newTreeDepth < maxTreeDepth)
            {
                l_child.Split(!horizontal, maxTreeDepth);
                r_child.Split(!horizontal, maxTreeDepth);
            }
        }

        public bool IsSplit()
        {
            return (this.l_child != null && this.r_child != null);
        }

        // Returns a random point on SplittingLine to split it on.
        public float FindRandomSplit(int splittingLine)
        {
            float min = MIN_SPLIT_PERCENTAGE * splittingLine;
            float max = MAX_SPLIT_PERCENTAGE * splittingLine;
            float random = Random.Range(min, max);
            return random;
        }

        // debugging bois
        public void PrintRoomContainer()
        {
            Debug.Log("x = " + this.x + ", y = " + this.y + ", width = " + this.width + ", height = " + this.height + ", treeDepth = " + this.treeDepth);
        }

    }
}
