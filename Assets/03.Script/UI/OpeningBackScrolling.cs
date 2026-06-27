using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningBackScrolling : MonoBehaviour
{
    private float distance; // 배경의 이동 거리
    private float parallaxSpeed;
    private bool shouldMove = false;

    private Material[] materials;
    public Transform CameraTransform;
    
    private void Awake()
    {
        transform.position = CameraTransform.position;

        int backgroundCount = transform.childCount;
        GameObject[] backgrounds = new GameObject[backgroundCount];

        materials = new Material[backgroundCount];

        for (int i = 0; i < backgroundCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            materials[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        parallaxSpeed = 0.1f;
    }
    private void Update()
    {
        transform.position = new Vector3(CameraTransform.position.x, CameraTransform.position.y);
        if (shouldMove)
        {
            MoveBackground();
        }
    }

    // 외부에서 호출하여 배경을 업데이트하는 메서드
    public void MoveBackground()
    {
        // 시간에 따라 배경을 이동시키는 방법
        distance = -Time.time * parallaxSpeed;

        for (int i = 0; i < materials.Length; ++i)
        {
            Vector2 offset = new Vector2(distance, 0);
            materials[i].SetTextureOffset("_MainTex", offset);
        }
    }
    public void StartMovingBackground()
    {
        shouldMove = true;
    }
}
