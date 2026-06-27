using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapScrolling : MonoBehaviour
{
    public Tilemap tilemap;
    public float scrollSpeed = 2.0f;
    public bool shouldMove = false;

    private void Update()
    {
        if (shouldMove)
        {
            // 타일맵을 오른쪽으로 이동
            float movement = Time.deltaTime * scrollSpeed;
            tilemap.transform.position += new Vector3(movement, 0, 0);
        }
    }
    public void StartMoving()
    {
        shouldMove = true;
    }
}
