using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiniMaps : MonoBehaviour
{
    [SerializeField] private GameObject[] doors; // 상단, 하단, 좌측, 우측 순서
 
    public Vector2Int RoomIndex { get; set; }

    // 배열에있는 문들의 콜라이더를 담고 타일맵에서 문들의 콜라이더만큼의 영역을 None으로 설정하는 메서드
   
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
        if (doorIndex >= 0 && doorIndex < doors.Length && doors[doorIndex] != null)
        {
            doors[doorIndex].SetActive(true);
        }
    }
}
