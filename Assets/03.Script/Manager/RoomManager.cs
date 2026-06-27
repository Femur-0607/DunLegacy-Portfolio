using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public CinemachineConfiner2D confiner;

    [SerializeField] GameObject startroomPrefab;
    [SerializeField] GameObject[] roomPrefab;
    [SerializeField] GameObject endroomPrefab;
    [SerializeField] GameObject miniMapPrefab;

    [Header("룸 생성 개수 조절")]
    [SerializeField] private int maxRooms = 10;
    [SerializeField] private int minRooms = 10;

    [Header("룸 영역 조절")]
    [SerializeField] int roomWidth = 100;
    [SerializeField] int roomHeight = 100;

    [Header("그리드 영역 조절")]
    [SerializeField] int gridSizeX = 7;
    [SerializeField] int gridSizeY = 7;

    private List<GameObject> roomObjects = new List<GameObject>();
    private List<GameObject> minimapObjects = new List<GameObject>();

    // 방의 위치를 추적하는 2D 배열
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    private int[,] roomGrid;
    private int roomCount;

    private Dictionary<Vector2Int, GameObject> minimapRooms = new Dictionary<Vector2Int, GameObject>();
    public bool generationComplete = false;

    public Room currentRoom;
    public MiniMaps currentMinimap;

    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(0, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);

        // Room-2 생성
        Vector2Int secondRoomIndex = new Vector2Int(1, gridSizeY / 2);
        GenerateSecondRoom(secondRoomIndex);
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();

            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }

        else if (roomCount < minRooms)
        {
            RegenerateRooms();
        }

        else if (!generationComplete)
        {
            generationComplete = true;
            GenerateMinimap(); // 모든 방이 생성된 후 미니맵 생성
        }
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }

    // 초기 방 생성 시작
    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex); // 큐에 데이터 집어넣기
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;

        var initialRoom = Instantiate(startroomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);

        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;

        roomObjects.Add(initialRoom);

        currentRoom = initialRoom.GetComponent<Room>(); // 초기 방을 현재 방으로 설정

        CollriderChange();
    }
 
    // Room-2 생성
    private void GenerateSecondRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;
        int randomnumber = Random.Range(0, roomPrefab.Length);
        
        var secondRoom = Instantiate(roomPrefab[randomnumber], GetPositionFromGridIndex(roomIndex), Quaternion.identity);


        secondRoom.name = $"Room-{roomCount}";
        secondRoom.GetComponent<Room>().RoomIndex = roomIndex;

        roomObjects.Add(secondRoom);

        var roomScript = secondRoom.GetComponent<Room>();
        roomScript.OpenDoor(Vector2Int.left);

        // 인접한 방이 있을 때만 다른 문 열기
        if (x + 1 < gridSizeX && roomGrid[x + 1, y] == 1) roomScript.OpenDoor(Vector2Int.right);
        if (y + 1 < gridSizeY && roomGrid[x, y + 1] == 1) roomScript.OpenDoor(Vector2Int.up);
        if (y - 1 >= 0 && roomGrid[x, y - 1] == 1) roomScript.OpenDoor(Vector2Int.down);
    }

    // 방 생성 시도
    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        // 유효성 검사
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
        {
            return false;
        }

        // Room-1과 Room-2방의 같은 X축에 배치되지 않도록 함
        if (x == 0 || x == 1)
        {
            return false;
        }

        if (roomCount >= maxRooms)
        {
            return false;
        }
        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
        {
            return false;
        }

        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;
        int randomnumber = Random.Range(0, roomPrefab.Length);
        GameObject roomToInstantiate = roomCount == maxRooms ? endroomPrefab : roomPrefab[randomnumber];
       
        var newRoom = Instantiate(roomToInstantiate, GetPositionFromGridIndex(roomIndex), Quaternion.identity);

        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y);

        return true;
    }

    // 방 재생성
    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        minimapObjects.ForEach(Destroy);
        minimapObjects.Clear();
        minimapRooms.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(0, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);

        Vector2Int secondRoomIndex = new Vector2Int(1, gridSizeY / 2);
        GenerateSecondRoom(secondRoomIndex);
    }

    // 생성된 방의 문 열기
    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        // Neighbours
        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // Determine Which Doors
        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            // left
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            // right
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }
        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            // below
            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);
        }
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);
        }
    }

    // 주어진 방의 인덱스에 해당하는 방 스크립트 반환
    public Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index); 
        if (roomObject != null)
            return roomObject.GetComponent<Room>();
        return null;
    }
    public MiniMaps GetMIniRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
            return roomObject.GetComponent<MiniMaps>();
        return null;
    }

    // 인접한 방의 개수 반환
    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;
        if (x > 0 && x - 1 < gridSizeX && roomGrid[x - 1, y] != 0) count++; // Left neighbour
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++; // Right neighbour
        if (y > 0 && y - 1 < gridSizeY && roomGrid[x, y - 1] != 0) count++; // Bottom neighbour
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++; // Top neighbour

        return count;
    }

    // 그리드 인덱스를 실제 위치로 변환하는 메서드
    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * gridX, roomHeight * (gridY - gridSizeY / 2), 1);
    }

    // confiner 콜라이더 변경
    public void CollriderChange()
    {
        Collider2D coll = currentRoom.GetComponentsInChildren<Collider2D>()[0];
        confiner.m_BoundingShape2D = coll;
    }

    // 미니맵 생성
    private void GenerateMinimap()
    {
        foreach (var room in roomObjects)
        {
            Room roomScript = room.GetComponent<Room>();
            Vector2Int roomIndex = roomScript.RoomIndex;

            Vector3 minimapPosition = room.transform.position + new Vector3(0, 0, 12);
            GameObject minimap = Instantiate(miniMapPrefab, GetPositionFromGridIndex(roomIndex) + new Vector3(0, 0, 12), Quaternion.identity);
            minimap.name = $"MiniMap-{roomIndex.x}-{roomIndex.y}";
            MiniMaps minimapScript = minimap.GetComponent<MiniMaps>();
            minimapScript.RoomIndex = roomIndex;

            minimapRooms[roomIndex] = minimap;
            minimapObjects.Add(minimap);


            if (roomIndex == new Vector2Int(0, gridSizeY / 2))
            {
                minimapScript.OpenDoor(Vector2Int.right);
            }
            else
            {
                // 다른 방들의 문 상태에 따라 미니맵 문 열기
                if (roomScript.IsDoorOpen(Vector2Int.left)) minimapScript.OpenDoor(Vector2Int.left);
                if (roomScript.IsDoorOpen(Vector2Int.right)) minimapScript.OpenDoor(Vector2Int.right);
                if (roomScript.IsDoorOpen(Vector2Int.down)) minimapScript.OpenDoor(Vector2Int.down);
                if (roomScript.IsDoorOpen(Vector2Int.up)) minimapScript.OpenDoor(Vector2Int.up);
            }

            minimap.transform.SetParent(room.transform);
        }
    }
}
