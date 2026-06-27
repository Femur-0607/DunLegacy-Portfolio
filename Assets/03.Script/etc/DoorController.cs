using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorController : MonoBehaviour
{
    public GameObject[] doors;
    public GameObject player;
    public Collider2D currentTilemapCollider;
    public bool[] isDoorOpen;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isDoorOpen = new bool[doors.Length];
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            // 모든 문이 닫혀있다면 열고, 열려있다면 닫는다.
            bool allDoorsClosed = true;
            foreach (bool doorOpen in isDoorOpen)
            {
                if (doorOpen)
                {
                    allDoorsClosed = false;
                    break;
                }
            }

            if (allDoorsClosed)
            {
                OpenAllDoors();
            }
            else
            {
                CloseAllDoors();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
      
        //if (other.CompareTag("Player"))
        //{
        //    TilemapCollider2D tilemapCollider = other.GetComponent<TilemapCollider2D>();
            
        //    if (tilemapCollider != null)
        //    {
        //        currentTilemapCollider = tilemapCollider;
        //    }
        //}
    }

    public void OpenDoor(int doorIndex)
    {
        if (doorIndex >= 0 && doorIndex < doors.Length && !isDoorOpen[doorIndex])
        {
            isDoorOpen[doorIndex] = true;
            StartCoroutine(OpenDoorCoroutine(doorIndex));
        }
    }

    public void CloseDoor(int doorIndex)
    {
        if (doorIndex >= 0 && doorIndex < doors.Length && isDoorOpen[doorIndex])
        {
            isDoorOpen[doorIndex] = false;
            StartCoroutine(CloseDoorCoroutine(doorIndex));
        }
    }

    public void OpenAllDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            OpenDoor(i);
        }
    }

    public void CloseAllDoors()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            CloseDoor(i);
        }
    }
    // 문 열리는 코루틴
    private IEnumerator OpenDoorCoroutine(int doorIndex)
    {
        GameObject door = doors[doorIndex];
        Animator doorAnimator = door.GetComponent<Animator>();

        // 문 열리는 애니메이션 실행
        doorAnimator.SetTrigger("SealOpen");

        // 문이 열리는 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);

        // 문 콜라이더를 비활성화하여 플레이어가 통과할 수 있게 함
        Collider2D doorCollider = door.GetComponent<Collider2D>();
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
        Physics2D.IgnoreCollision(currentTilemapCollider, doorCollider, true);
    }
    // 문 닫히는 코루틴
    private IEnumerator CloseDoorCoroutine(int doorIndex)
    {
        GameObject door = doors[doorIndex];
        Animator doorAnimator = door.GetComponent<Animator>();

        // 문 닫히는 애니메이션 실행
        doorAnimator.SetTrigger("SealClose");

        // 문이 닫히는 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);

        // 문 콜라이더를 다시 활성화하여 플레이어가 통과하지 못하게 함
        Collider2D doorCollider = door.GetComponent<Collider2D>();
        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
        Physics2D.IgnoreCollision(currentTilemapCollider, doorCollider, false);
    }
}
