using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomManager : MonoBehaviour
{
    public GameObject roomPrefab; // 방 프리팹
    public GameObject wallPrefab; // 벽 프리팹
    public GameObject doorPrefab; // 문 프리팹
    public Transform mapParent; // 방을 포함할 부모 객체
    public int mapWidth = 20; // 맵 너비
    public int mapHeight = 20; // 맵 높이
    public int roomSize = 5; // 방의 크기

    private int[,] mapArray;

    void Start()
    {
        mapArray = new int[mapWidth, mapHeight];
        GenerateMapStructure();
        GenerateMap();
    }

    void GenerateMapStructure()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (Random.Range(0, 100) < 20) // 20% 확률로 방 생성
                {
                    mapArray[i, j] = 1; // 방이 있는 경우 1
                }
                else
                {
                    mapArray[i, j] = 0; // 방이 없는 경우 0
                }
            }
        }
    }

    void GenerateMap()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (mapArray[i, j] == 1)
                {
                    Vector3 roomPosition = new Vector3(i * roomSize, j * roomSize, 0);
                    GameObject room = Instantiate(roomPrefab, roomPosition, Quaternion.identity, mapParent);

                    // 상하좌우를 확인하여 벽과 문 생성
                    CreateWallsAndDoors(i, j, room.transform);
                }
            }
        }
    }

    void CreateWallsAndDoors(int x, int y, Transform roomTransform)
    {
        bool topRoom = y < mapHeight - 1 && mapArray[x, y + 1] == 1;
        bool bottomRoom = y > 0 && mapArray[x, y - 1] == 1;
        bool leftRoom = x > 0 && mapArray[x - 1, y] == 1;
        bool rightRoom = x < mapWidth - 1 && mapArray[x + 1, y] == 1;

        if (!topRoom) CreateWall(roomTransform, Vector3.up * roomSize / 2);
        if (!bottomRoom) CreateWall(roomTransform, Vector3.down * roomSize / 2);
        if (!leftRoom) CreateWall(roomTransform, Vector3.left * roomSize / 2);
        if (!rightRoom) CreateWall(roomTransform, Vector3.right * roomSize / 2);

        if (topRoom) CreateDoor(roomTransform, Vector3.up * roomSize / 2);
        if (bottomRoom) CreateDoor(roomTransform, Vector3.down * roomSize / 2);
        if (leftRoom) CreateDoor(roomTransform, Vector3.left * roomSize / 2);
        if (rightRoom) CreateDoor(roomTransform, Vector3.right * roomSize / 2);
    }

    void CreateWall(Transform parent, Vector3 position)
    {
        Instantiate(wallPrefab, parent.position + position, Quaternion.identity, parent);
    }

    void CreateDoor(Transform parent, Vector3 position)
    {
        Instantiate(doorPrefab, parent.position + position, Quaternion.identity, parent);
    }
}