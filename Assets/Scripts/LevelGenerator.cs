using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public List<RoomContainer> roomContainers = new List<RoomContainer>();
    public const int WIDTH = 70;
    public const int HEIGHT = 70;
    public const float MIN_SPLIT_PERCENTAGE = 0.4f;
    public const float MAX_SPLIT_PERCENTAGE = 1f - MIN_SPLIT_PERCENTAGE;
    public const float STOP_SPLITTING_CHANCE = 0.4f;

    public List<Room> rooms = new List<Room>();
    public const float MIN_ROOM_PADDING = 0.05f;
    public const float MAX_ROOM_PADDING = 0.2f;
    public const int MIN_ROOM_WIDTH = 10;
    public const int MIN_ROOM_HEIGHT = 10;

    public GameObject[,] tiles;
   

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
        GenerateRoomContainers();
        GenerateRooms();
        GenerateLevel();
    }


    void Update()
    {

    }


    void GenerateRoomContainers()
    {
        RoomContainer mainContainer = new RoomContainer(0, 0, WIDTH, HEIGHT, 0, roomContainers);
        int maxTreeDepth = 4;
        mainContainer.Split(true, maxTreeDepth, roomContainers);
    }

    void GenerateRooms()
    {
        foreach (RoomContainer rc in roomContainers)
        {
            if (rc.isLastNode)
            {
                Debug.Log("found last node room container");
                Room newRoom = new Room(rc.x, rc.y, rc.width, rc.height);
                rooms.Add(newRoom);
            }
        }
    }


    void GenerateLevel()
    {
        tiles = new GameObject[WIDTH, HEIGHT];
        for (int i = 0; i < rooms.Count; i++)
        {
            Room currRoom = rooms[i];
            for (int x = currRoom.x; x < currRoom.x + currRoom.width; x++)
            {
                for (int y = currRoom.y; y < currRoom.y + currRoom.height; y++)
                {
                    GameObject newTile;
                    if (x != currRoom.x && y != currRoom.y && x != currRoom.x + currRoom.width -1 && y != currRoom.y + currRoom.height - 1 && tiles[x, y] == null)
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
                if (currTile != null && currTile.tag == "Block")
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
    }

    public class RoomContainer
    {
        // (x, y) indicates top left corner
        public int x;
        public int y;
        public int width;
        public int height;

        public int treeDepth;
        public bool isLastNode;
        public RoomContainer l_child;
        public RoomContainer r_child;

        public bool isSplitHorizontally;

        public RoomContainer(int x, int y, int width, int height, int treeDepth, List<RoomContainer> roomContainers)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;

            this.treeDepth = treeDepth;
            this.isLastNode = true;
            roomContainers.Add(this);
        }

        public void Split( bool horizontal, int maxTreeDepth, List<RoomContainer> roomContainers)
        {
            int newTreeDepth = this.treeDepth + 1;
            this.isSplitHorizontally = horizontal;
            this.isLastNode = false;

            // create two new roomcontainers
            if (horizontal)
            {
                int split_x = this.x + FindRandomSplit(this.width);

                int left_w = split_x - this.x;
                int right_w = this.width - left_w;

                l_child = new RoomContainer(this.x, this.y, left_w, this.height, newTreeDepth, roomContainers);
                r_child = new RoomContainer(split_x, this.y, right_w, this.height, newTreeDepth, roomContainers);
            }
            else
            {
                int split_y = this.y + FindRandomSplit(this.height);

                int left_h = split_y - this.y;
                int right_h = this.height - left_h;

                l_child = new RoomContainer(this.x, this.y, this.width, left_h, newTreeDepth, roomContainers);
                r_child = new RoomContainer(this.x, split_y, this.width, right_h, newTreeDepth, roomContainers);
            }

            // recursively create more sub rooms in newly created rooms if maxTreeDepth hasn't been reached yet
            // if this room was split horizontally, split the next rooms vertically (and vice versa)
            // Somtimes it will randomly stop splitting a room and leave it bigger.
            if (newTreeDepth < maxTreeDepth)
            {
                if (Random.Range(0, 1f) > (STOP_SPLITTING_CHANCE * newTreeDepth) + (2 * -STOP_SPLITTING_CHANCE))
                {
                    l_child.Split(!horizontal, maxTreeDepth, roomContainers);
                }
                if (Random.Range(0, 1f) > (STOP_SPLITTING_CHANCE * newTreeDepth) + (2 * -STOP_SPLITTING_CHANCE))
                {
                    r_child.Split(!horizontal, maxTreeDepth, roomContainers);
                }
            }
        }

        // Returns a random point on SplittingLine to split it on.
        public int FindRandomSplit(int splittingLine)
        {
            float min = MIN_SPLIT_PERCENTAGE * (float)splittingLine;
            float max = MAX_SPLIT_PERCENTAGE * (float)splittingLine;
            float random = Random.Range(min, max);
            return Mathf.RoundToInt(random);
        }

        // debugging bois
        public void Print()
        {
            Debug.Log("RoomContainer Values \n x = " + this.x + ", y = " + this.y + ", width = " + this.width + ", height = " + this.height + ", treeDepth = " + this.treeDepth + ", isLastNode = " + this.isLastNode);
        }

    }

    public class Room
    {
        // (x, y) indicates top left corner
        public int x;
        public int y;
        public int width;
        public int height;

        public Room(int container_x, int container_y, int container_w, int container_h)
        {
            int leftPadding = RandomPadding(container_w);
            int rightPadding = RandomPadding(container_w);
            int topPadding = RandomPadding(container_h);
            int bottomPadding = RandomPadding(container_h);

            this.x = container_x + leftPadding;
            this.y = container_y + topPadding;
            this.width = Mathf.Max(container_w - (leftPadding + rightPadding), MIN_ROOM_WIDTH);
            this.height = Mathf.Max(container_h - (topPadding + bottomPadding), MIN_ROOM_HEIGHT);

            this.Print();
        }

        public int RandomPadding(int length)
        {
            float min = MIN_ROOM_PADDING * (float)length;
            float max = MAX_ROOM_PADDING * (float)length;
            float random = Random.Range(min, max);
            return Mathf.RoundToInt(random);
        }

        // debugging bois
        public void Print()
        {
            Debug.Log("Room Values \n x = " + this.x + ", y = " + this.y + ", width = " + this.width + ", height = " + this.height);
        }
    }
}
