using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] doors; // 상단, 하단, 좌측, 우측 순서
    public Tilemap tilemap;
    public List<BoxCollider2D> boxColliders;
    public Vector2Int RoomIndex { get; set; }

    public Transform doorEntranceUp; // 문 앞 빈 오브젝트 참조
    public Transform doorEntranceDown;
    public Transform doorEntranceLeft;
    public Transform doorEntranceRight;

    private void Start()
    {
        Invoke("InitializeDoors", 3f);
        doorEntranceUp = doors[0].transform.GetChild(4).transform;
        doorEntranceDown = doors[1].transform.GetChild(4).transform;
        doorEntranceLeft = doors[2].transform.GetChild(4).transform;
        doorEntranceRight = doors[3].transform.GetChild(4).transform;
    }

    // 배열에있는 문들의 콜라이더를 담고 타일맵에서 문들의 콜라이더만큼의 영역을 None으로 설정하는 메서드
    public void InitializeDoors()
    {
        boxColliders = new List<BoxCollider2D>();

        foreach (GameObject door in doors)
        {
            boxColliders.Add(door.GetComponent<BoxCollider2D>());
        }
        DisableTilemapColliderWithinBox();
    } 
    private void DisableTilemapColliderWithinBox()
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                Vector3 cellWorldPosition = tilemap.CellToWorld(cellPosition);

                foreach (BoxCollider2D boxCollider in boxColliders)
                {
                    if (boxCollider.bounds.Contains(cellWorldPosition))
                    {
                        tilemap.SetColliderType(cellPosition, Tile.ColliderType.None);
                    }
                }
            }
        }
    }

    // 문여는 메서드
    public GameObject GetDoor(Vector2Int direction)
    {
        int index = GetDoorIndex(direction);
        if (index >= 0 && index < doors.Length)
        {
            return doors[index];
        }
        return null;
    }
    private int GetDoorIndex(Vector2Int direction)
    {
        if (direction == Vector2Int.up) return 0;
        if (direction == Vector2Int.down) return 1;
        if (direction == Vector2Int.left) return 2;
        if (direction == Vector2Int.right) return 3;
        return -1;
    }
    public void OpenDoor(Vector2Int direction)
    {
        int doorIndex = GetDoorIndex(direction);
        if (doorIndex >= 0 && doorIndex < doors.Length)
        {
            doors[doorIndex].SetActive(true);
        }
    }
    public bool IsDoorOpen(Vector2Int direction)
    {
        int index = GetDoorIndex(direction);
        if (index >= 0 && index < doors.Length)
        {
            return doors[index].activeSelf;
        }
        return false;
    }
}
