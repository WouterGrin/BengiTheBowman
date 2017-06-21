using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static int IDCounter = 0;

    public List<RoomContainer> roomContainers = new List<RoomContainer>();
    public const int WIDTH = 80;
    public const int HEIGHT = 80;
    public const int MAX_TREE_DEPTH = 4;
    public const float MIN_SPLIT_PERCENTAGE = 0.4f;
    public const float MAX_SPLIT_PERCENTAGE = 1f - MIN_SPLIT_PERCENTAGE;
    public const float STOP_SPLITTING_CHANCE = 0.4f;

    public List<Room> rooms = new List<Room>();
    public const float MIN_ROOM_PADDING = 0.075f;
    public const float MAX_ROOM_PADDING = 0.175f;
    public const int MIN_ROOM_WIDTH = 7;
    public const int MIN_ROOM_HEIGHT = 7;


    public Dictionary<string, Path> pathDict = new Dictionary<string, Path>();
    public Dictionary<string, Room> roomDict = new Dictionary<string, Room>();
    public List<Color> knownColors = new List<Color>();


    public List<Path> paths = new List<Path>();
    public const int PATH_WIDTH = 3;

    public GameObject[,] tiles;
   
    ObjectContainer objContainer;
    public GameObject groundTile;
    public GameObject wallTile;
    public GameObject doorPrefab;
    public GameObject keyPrefab;
    public GameObject enemyPrefab;
    GameObject player;
	CaveGenerator caveGenerator;

	[Range(0, 100)]
	public int randomFillPercent;

	[Range(0, 5)]
	public int automataIterations;

	void Start()
    {
        knownColors.Add(Color.yellow);
        knownColors.Add(Color.black);
        knownColors.Add(Color.blue);
        knownColors.Add(Color.cyan);
        knownColors.Add(Color.gray);
        knownColors.Add(Color.green);
        knownColors.Add(Color.magenta);
        knownColors.Add(Color.red);
        knownColors.Add(Color.white);
        //???? idunno maine
        knownColors.Add(new Color(0.2f, 0.1f, 0.6f));
        knownColors.Add(new Color(0.6f, 0.2f, 0.1f));
        knownColors.Add(new Color(0.2f, 0.6f, 0.1f));
        knownColors.Add(new Color(0.2f, 0.9f, 0.5f));
        knownColors.Add(new Color(0.2f, 0.5f, 0.9f));
        knownColors.Add(new Color(0.5f, 0.9f, 0.2f));


        objContainer = GameObject.Find("ObjectContainer").GetComponent<ObjectContainer>();
        player = GameObject.Find("Player");
		caveGenerator = new CaveGenerator(randomFillPercent, automataIterations, groundTile, wallTile);
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
        
        mainContainer.Split(true, MAX_TREE_DEPTH, roomContainers);
    }

    void GenerateRooms()
    {
        foreach (RoomContainer rc in roomContainers)
        {
            if (rc.isLastNode)
            {
                Room newRoom = new Room(rc.x, rc.y, rc.width, rc.height);
                newRoom.id = rc.id;
                rooms.Add(newRoom);
                rc.room = newRoom;
                roomDict.Add(newRoom.id.ToString(), newRoom);
            }
        }
        foreach (RoomContainer rc in roomContainers)
        {
            if (!rc.isLastNode)
            {
                GeneratePath(rc);
            }
        }
        // TODO: remove this debugging part
        foreach (Room room in rooms)
        {
            room.PrintNeighbours();
        }
    }

    // generates a pathway between leftchild and rightchild of a roomcontainer.
    // this method will only be called after checking if the roomcontainer has l_ & r_child.
    void GeneratePath(RoomContainer roomContainer)
    {
        int left_x = roomContainer.l_child.center[0];
        int left_y = roomContainer.l_child.center[1];
        int right_x = roomContainer.r_child.center[0];
        int right_y = roomContainer.r_child.center[1];

        int distance = 0;

        if (roomContainer.isSplitHorizontally)
        {
            distance = (right_x - left_x);
        }
        else
        {
            distance = (right_y - left_y);
        }

        Path newPath = new Path(left_x, left_y, distance, roomContainer.isSplitHorizontally);

        List<RoomContainer> finalNodes = roomContainer.FindFinalNodesOnPath();

       
        finalNodes = newPath.RemoveContainersWithRoomsNotOnPath(finalNodes);
        bool twoRooms = true;
        // move path so it covers multiple rooms.
        if (finalNodes.Count < 2)
        {
            twoRooms = false;
        }

        while (!twoRooms)
        {
            if (newPath.isHorizontal)
            {
                newPath.y += 3;
            }
            else
            {
                newPath.x += 3;
            }

            finalNodes = roomContainer.FindFinalNodesOnPath();
            finalNodes = newPath.RemoveContainersWithRoomsNotOnPath(finalNodes);

            if (finalNodes.Count < 2)
            {
                twoRooms = false;
            }
            else
            {
                twoRooms = true;
            }
        }

        // resize path if it covers multiple rooms.
        if (finalNodes.Count > 2)
        {
            finalNodes = newPath.GetTwoClosestContainers(finalNodes);
            newPath.PlaceBetweenContainers(finalNodes);
        }

        paths.Add(newPath);

        if (finalNodes.Count == 2)
        {
            string pathID1 = finalNodes[0].id + "-" + finalNodes[1].id;
            string pathID2 = finalNodes[1].id + "-" + finalNodes[0].id;
            if (!pathDict.ContainsKey(pathID1))
                pathDict.Add(pathID1, newPath);
            if (!pathDict.ContainsKey(pathID2))
                pathDict.Add(pathID2, newPath);

            newPath.FillRoomNeighboursAfterPathCreation(finalNodes, pathDict);
        }
    }

    void GenerateLevel()
    {
        tiles = new GameObject[WIDTH, HEIGHT];


        float minX = float.MaxValue;
        float minY = float.MaxValue;
        Room startingRoom = null;
        for (int i = 0; i < rooms.Count; i++)
        {
            Room currRoom = rooms[i];
            if (currRoom.x < minX && currRoom.y < minY)
            {
                startingRoom = currRoom;
                minX = currRoom.x;
                minY = currRoom.y;
            }


            int[,] newCave = caveGenerator.GenerateCave(currRoom.width, currRoom.height);
            for (int x = currRoom.x; x < currRoom.x + currRoom.width; x++)
            {
                for (int y = currRoom.y; y < currRoom.y + currRoom.height; y++)
                {
                    if (x  < WIDTH && y < HEIGHT)
                    {
                        GameObject newTile;
                        if (x != currRoom.x && y != currRoom.y && x != currRoom.x + currRoom.width - 1 && y != currRoom.y + currRoom.height - 1 && tiles[x, y] == null && newCave[x - currRoom.x, y - currRoom.y] == 0)
                        {
                            newTile = Instantiate(groundTile, new Vector3(x * 1f, y * 1f, 0), Quaternion.identity) as GameObject;
                            tiles[x, y] = newTile;

                        }
                        else if (tiles[x, y] == null)
                        {
                            newTile = Instantiate(wallTile, new Vector3(x * 1f, y * 1f, 0), Quaternion.identity) as GameObject;
                            newTile.GetComponent<WallTile>().roomID = currRoom.id;
                            tiles[x, y] = newTile;
                        }
                    }
					 
                }
            }
        }

        foreach (Path path in paths)
        {
            for (int x = path.x; x < path.x + path.width; x++)
            {
                for (int y = path.y; y < path.y + path.height; y++)
                {

                    if ((x != path.x && y != path.y && x != path.x + path.width - 1 && y != path.y + path.height - 1) ||  tiles[x,y] != null)
                    {
                        Destroy(tiles[x, y]);
                        GameObject newTile = Instantiate(groundTile, new Vector3(x * 1f, y * 1f, 0), Quaternion.identity) as GameObject;
                        tiles[x, y] = newTile;
                    }
                }
            }
        }


        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            { 
                if (tiles[x, y] == null)
                {
                    GameObject newTile = Instantiate(wallTile, new Vector3(x * 1f, y * 1f, 0), Quaternion.identity) as GameObject;
                    tiles[x, y] = newTile;
                }
                
            }
        }
        player.transform.position = new Vector3(startingRoom.x + startingRoom.width / 2, startingRoom.y + startingRoom.height / 2, player.transform.position.z);



        PlaceDoorsAndKeys(startingRoom);

        AdjustSpritesAndHitboxes();

        PopulateRooms(startingRoom);
        //PlaceDoor(new Vector2(5, 5), Color.red);
        //PlaceKey(new Vector2(0, 1), Color.red);
        //PlaceKey(new Vector2(0, 5), Color.yellow);
    }

    void PlaceDoorsAndKeys(Room startingRoom)
    {
        Dictionary<string, bool> visitedRooms = new Dictionary<string, bool>();
        foreach (KeyValuePair<string, Room> entry in roomDict)
        {
            visitedRooms.Add(entry.Value.id.ToString(), false);
        }
        Room currentRoom = startingRoom;
        List<string> iterateList = AddPathsToList(startingRoom, null, visitedRooms);
        int t = 0;
        while (iterateList.Count > 0 && t < 100)
        {
            t++;
            Color pickedColor = PlaceKeyAtRandomLocationInRoom(currentRoom);
            int randomPathIndex = Random.Range(0, iterateList.Count);
            string pickedPath = iterateList[randomPathIndex];
            Path chosenPath = pathDict[pickedPath];
            PlaceDoorInPath(chosenPath, pickedColor);
            visitedRooms[currentRoom.id.ToString()] = true;
            currentRoom = GoThroughPath(currentRoom, pickedPath);
            iterateList.Remove(pickedPath);
            iterateList = AddPathsToList(currentRoom, iterateList, visitedRooms);
        }
        
        
    }

    Room GoThroughPath(Room currentRoom, string pathString)
    {
        char[] delimiter = new char[1] {'-'};
        string[] splitChars = pathString.Split(delimiter);
        Room destination = roomDict[splitChars[1]];
        return destination;
    }

    List<string> AddPathsToList(Room room, List<string> pathList, Dictionary<string, bool> _visitedRooms)
    {
        if (pathList == null)
            pathList = new List<string>();
        for (int i = 0; i < room.neighbours.Count; i++)
        {
            Room currRoom = room.neighbours[i]; 

            if (!_visitedRooms[currRoom.id.ToString()])
            {
                string pathID = room.id.ToString() + "-" + currRoom.id.ToString();
                pathList.Add(pathID);
            }
        }
        return pathList;
    }




    void PlaceDoorInPath(Path path, Color color)
    {
        bool foundTile = false;
        Vector2 foundTilePos = Vector2.zero;
        int skipTilesX = 0;
        int skipTilesY = 0;
        if (path.width > path.height)
            skipTilesX = (int)(path.width * 0.2f);
        if (path.height > path.width)
            skipTilesY = (int)(path.height * 0.2f);
        for (int x = path.x + skipTilesX; x < path.x + path.width && !foundTile; x++)
        {
            for (int y = path.y + skipTilesY; y < path.y + path.height && !foundTile; y++)
            {
                if (path.height > path.width)
                {
                    GameObject leftTile = tiles[x - 1, y];
                    GameObject rightTile = tiles[x + 1, y];
                    if (leftTile.tag == "Block" && rightTile.tag == "Block")
                    {
                        foundTile = true;
                        foundTilePos = new Vector2(x, y);
                    }
                }
                else
                {
                    GameObject upperTile = tiles[x, y + 1];
                    GameObject lowerTile = tiles[x, y - 1];
                    if (upperTile.tag == "Block" && lowerTile.tag == "Block")
                    {
                        foundTile = true;
                        foundTilePos = new Vector2(x, y);
                    }
                }
            }
        }
        if (foundTile)
        {
            PlaceDoor(foundTilePos, color);
        }

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

    Color PlaceKeyAtRandomLocationInRoom(Room room)
    {
        Color ranColor = new Color(0, 0, 0, 0);
        int iterationNr = 0;
        while (iterationNr < 1000)
        {
            iterationNr++;
            int randomPosX = Random.Range(room.x, room.x + room.width);
            int randomPosY = Random.Range(room.y, room.y + room.height);
            if (tiles[randomPosX, randomPosY].tag != "Block")
            {
                int colorIndex = Random.Range(0, knownColors.Count);
                ranColor = knownColors[colorIndex];
                knownColors.Remove(ranColor);
                PlaceKey(new Vector2(randomPosX, randomPosY), ranColor);
                break;
            }
        }
       
        return ranColor;
    }

    void PlaceDoor(Vector2 pos, Color color)
    {
        GameObject newDoor = Instantiate(doorPrefab, pos, Quaternion.identity) as GameObject;
        print(pos);
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
		// keyScript.AdjustSprite(tiles, pos);
	}

    void PlaceSkeleton(Vector2 pos, Room room)
    {
        GameObject newSkeleton = Instantiate(enemyPrefab, pos, Quaternion.identity) as GameObject;
        Skeleton skelScript = newSkeleton.GetComponent<Skeleton>();
        skelScript.player = player;
        skelScript.triggerRoom = room;
    }

    void PopulateRooms(Room startingRoom)
    {
        print(rooms.Count);
        for (int i = 0; i < rooms.Count; i++)
        {
            Room currRoom = rooms[i];
            if (currRoom.id != startingRoom.id)
            {
                float distanceToStartRoom = Vector2.Distance(new Vector2(currRoom.x, currRoom.y), new Vector2(startingRoom.x, startingRoom.y));
                print(distanceToStartRoom);
                for (int j = 0; j < distanceToStartRoom; j++)
                {
                    int randomPosX = Random.Range(currRoom.x, currRoom.x + currRoom.width);
                    int randomPosY = Random.Range(currRoom.y, currRoom.y + currRoom.height);

                    if (tiles[randomPosX, randomPosY].tag != "Block")
                    {
                        PlaceSkeleton(new Vector2(randomPosX, randomPosY), currRoom);
                    }

                   
                }
            }
        }
    }

    

	public class RoomContainer
    {
        // (x, y) indicates top left corner
        public int id;
        public int x;
        public int y;
        public int width;
        public int height;
        public int[] center;

        public int treeDepth;
        public bool isLastNode;
        public RoomContainer l_child;
        public RoomContainer r_child;

        public Room room;
        public int distanceFromPath;

        public bool isSplitHorizontally;
        public int split;

        public RoomContainer(int x, int y, int width, int height, int treeDepth, List<RoomContainer> roomContainers)
        {
            this.id = LevelGenerator.IDCounter;
            LevelGenerator.IDCounter++;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.center = new int[2] {(x + width/2), (y + height/2)};

            this.treeDepth = treeDepth;
            this.isLastNode = true;
            roomContainers.Add(this);
        }

        public List<RoomContainer> FindFinalNodesOnPath()
        {
            List<RoomContainer> finalNodes = new List<RoomContainer>();
            this.AddFinalNodesToList(finalNodes);
            return finalNodes;
        }

        public void AddFinalNodesToList(List<RoomContainer> roomContainers)
        {
            if (!this.isLastNode)
            {
                this.l_child.AddFinalNodesToList(roomContainers);
                this.r_child.AddFinalNodesToList(roomContainers);
            }
            else
            {
                roomContainers.Add(this);
            }
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
                this.split = split_x;

                int left_w = split_x - this.x;
                int right_w = this.width - left_w;

                l_child = new RoomContainer(this.x, this.y, left_w, this.height, newTreeDepth, roomContainers);
                r_child = new RoomContainer(split_x, this.y, right_w, this.height, newTreeDepth, roomContainers);
            }
            else
            {
                int split_y = this.y + FindRandomSplit(this.height);
                this.split = split_y;

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
        public int id;
        public int x;
        public int y;
        public int width;
        public int height;

        public List<Room> neighbours;

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

            neighbours = new List<Room>();
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

        // debugging bois
        public void PrintNeighbours()
        {
            this.Print();
            foreach(Room neighbour in this.neighbours)
            {
                Debug.Log("neighbour: x = " + neighbour.x + ", y = " + neighbour.y);
            }
        }
    }

    public class Path
    {
        // (x, y) indicates top left corner
        public int x;
        public int y;
        public int width;
        public int height;
        public bool isHorizontal;

        public Path(int x, int y, int size, bool horizontal)
        {
            this.x = x;
            this.y = y;
            this.isHorizontal = horizontal;
            if(horizontal)
            {
                this.width = size;
                this.height = PATH_WIDTH;
            }
            else
            {
                this.width = PATH_WIDTH;
                this.height = size;
            }
        }

        public void PlaceBetweenContainers(List<RoomContainer> roomContainers)
        {

            int left_x = roomContainers[0].center[0];
            int left_y = roomContainers[0].center[1];
            int right_x = roomContainers[1].center[0];
            int right_y = roomContainers[1].center[1];

            this.x = left_x;
            this.y = left_y;

            int distance = 0;

            if (this.isHorizontal)
            {
                distance = (right_x - left_x) + 2;
                this.width = distance;
            }
            else
            {
                distance = (right_y - left_y) + 2;
                this.height = distance;
            }
        }

        public void FillRoomNeighboursAfterPathCreation(List<RoomContainer> finalNodes, Dictionary<string, Path> _pathDict)
        {
            // TODO: Sometimes a path doesn't intersect multiple rooms and keeps going until it reaches another path.
            // In that case, neighbours don't get added correctly because the connected RoomContainer will be removed in this step:

            if (finalNodes.Count == 2)
            {
                RoomContainer rc1 = finalNodes[0];
                RoomContainer rc2 = finalNodes[1];

                rc1.room.neighbours.Add(rc2.room);
                rc2.room.neighbours.Add(rc1.room);

                if (!_pathDict.ContainsKey(rc1.id + "-" + rc2.id))
                    _pathDict.Add(rc1.id + "-" + rc2.id, this);

                if (!_pathDict.ContainsKey(rc2.id + "-" + rc1.id))
                    _pathDict.Add(rc2.id + "-" + rc1.id, this);
            }
        }

        // these methods are becoming abomiantions like a UFKCING spaghetti monster PIECE Of shit
        public List<RoomContainer> GetTwoClosestContainers(List<RoomContainer> roomContainers)
        {
            int pathMiddle;
            List<RoomContainer> temp = new List<RoomContainer>();
            int[] distances = new int[roomContainers.Count];
            int[] shortestDistanceIndexes = new int[2];

            if (this.isHorizontal)
            {
                pathMiddle = this.x + (this.width / 2);
                for (int i = 0; i < roomContainers.Count; i++)
                {
                    RoomContainer rc = roomContainers[i];
                    int rcLeft = rc.x;
                    int rcRight = rc.x + rc.width;
                    int rc_distance = Mathf.Min(Mathf.Abs(rcLeft - pathMiddle), Mathf.Abs(rcRight - pathMiddle));
                    rc.distanceFromPath = rc_distance;
                }
            }
            else
            {
                pathMiddle = this.y + (this.height / 2);
                for (int i = 0; i < roomContainers.Count; i++)
                {
                    RoomContainer rc = roomContainers[i];
                    int rcLeft = rc.y;
                    int rcRight = rc.y + rc.height;
                    int rc_distance = Mathf.Min(Mathf.Abs(rcLeft - pathMiddle), Mathf.Abs(rcRight - pathMiddle));
                    rc.distanceFromPath = rc_distance;
                }
            }

            List<RoomContainer> sortedList = roomContainers.OrderBy(rc => rc.distanceFromPath).ToList();
            sortedList.RemoveRange(2, sortedList.Count - 2);
            return sortedList;
        }

        public List<RoomContainer> RemoveContainersWithRoomsNotOnPath(List<RoomContainer> roomContainers)
        {
            List<RoomContainer> temp = new List<RoomContainer>();
            foreach (RoomContainer rc in roomContainers)
            {
                if (rc.room.x < (this.x + this.width) && (rc.room.x + rc.room.width) > this.x && (rc.room.y + rc.room.height) > this.y && rc.room.y < (this.y + this.height))
                {
                    temp.Add(rc);
                }
            }
            return temp;
        }

        // debugging bois
        public void Print()
        {
            Debug.Log("Path Values \n x = " + this.x + ", y = " + this.y + ", width = " + this.width + ", height = " + this.height + ", isHorizontal = " + this.isHorizontal);
        }
    }
}
