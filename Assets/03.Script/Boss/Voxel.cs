using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    public float destoryTIme = 5f;
    float currentTime;

    private void OnEnable()
    {
        currentTime = 0;

       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > destoryTIme )
        {
            gameObject.SetActive( false );
            VoxelMaker.voxelPool.Add(gameObject);
        }
    }
}
