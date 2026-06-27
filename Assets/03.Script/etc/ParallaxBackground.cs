using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private Transform camerTransform;

    private Vector3 cameraStartPosition;
    private float distance;

    private Material[] materials;
    private float[] layerMoveSpeed;

    [SerializeField][Range(0.01f, 1.0f)]
    private float parallaxSpeed;

    private void Awake()
    {
        cameraStartPosition = transform.position;

        int backgroundCount = transform.childCount;
        GameObject[] backgrounds = new GameObject[backgroundCount];

        materials = new Material[backgroundCount];
        layerMoveSpeed = new float[backgroundCount];

        for(int i = 0; i < backgroundCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            materials[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        CalculateMoveSpeedByLayer(backgrounds, backgroundCount);
    }

    private void CalculateMoveSpeedByLayer(GameObject[] backgrounds, int count)
    {
        float farthesBackDistance = 0;
        for (int i = 0; i < count; ++i)
        {
            if ((backgrounds[i].transform.position.z - camerTransform.position.z) > farthesBackDistance)
            {
                farthesBackDistance = backgrounds[i].transform.position.z - camerTransform.position.z;
            }
        }

        for (int i = 0;i < count; ++i)
        {
            layerMoveSpeed[i] = 1 - (backgrounds[i].transform.position.z - camerTransform.position.z) / farthesBackDistance;
        }
    }

    private void LateUpdate()
    {
        distance = camerTransform.position.x - cameraStartPosition.x;
        transform.position = new Vector3(camerTransform.position.x, transform.position.y, 0);

        for (int i = 0; i < materials.Length; ++i)
        {
            float speed = layerMoveSpeed[i] * parallaxSpeed;
            materials[i].SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);
        }
    }
}
