using UnityEngine;

public class ToggleOnTap : MonoBehaviour
{
    public GameObject miniMapPanel;
    public GameObject miniMapIcon;
    public bool isActive = false;
    public bool isToggleMinimapObjectsCalled = false;

    public GameObject miniMapObject; // Minimap 오브젝트를 저장할 변수

    void Start()
    {
        miniMapPanel.SetActive(false);
        miniMapIcon.SetActive(true);

        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isActive = !isActive;
            miniMapPanel.SetActive(isActive);
            miniMapIcon.SetActive(!isActive);

            FindMinimapObject();
            ToggleMinimapObjects(isActive);
        }
    }

    // Minimap 오브젝트 찾기
    void FindMinimapObject()
    {
        if (RoomManager.Instance != null && RoomManager.Instance.currentRoom != null)
        {
            foreach (Transform child in RoomManager.Instance.currentRoom.transform)
            {
                if (child.gameObject.layer == LayerMask.NameToLayer("Minimap"))
                {
                    foreach (Transform grandchild in child)
                    {
                        if (grandchild.gameObject.layer == LayerMask.NameToLayer("Minimap"))
                        {
                            miniMapObject = grandchild.gameObject;
                            return;
                        }
                    }
                }
            }
        }
    }

    // Minimap 오브젝트 토글
    public void ToggleMinimapObjects(bool state)
    {
        isToggleMinimapObjectsCalled = true;

        if (miniMapObject != null)
        {
            miniMapObject.SetActive(state);
        }
    }
}
