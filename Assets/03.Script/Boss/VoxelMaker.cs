using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMaker : MonoBehaviour
{

    public GameObject voxelFactory;
    public Transform poolTr;
    public int voxelPoolsize = 50;
    public static List<GameObject> voxelPool = new List<GameObject>();
    int a = -1;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < voxelPoolsize; i++)
        {
            GameObject voxel = Instantiate(voxelFactory);
            voxel.SetActive(false);
            voxel.transform.parent = poolTr;
            voxelPool.Add(voxel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)&&voxelPool.Count>0)
        {


            a++;
            if(a==4)
            {
                a = -1;
            }

            GameObject voxel = voxelPool[0];
            voxel.SetActive(true);
            Vector2[] directions = new Vector2[4];
            directions[0] = Vector2.up;         // 위쪽 방향
            directions[1] = Vector2.right;      // 오른쪽 방향
            directions[2] = -Vector2.up;        // 아래쪽 방향
            directions[3] = -Vector2.right;     // 왼쪽 방향
            
                AddForceToBullet(voxel, directions[a]);
                voxelPool.RemoveAt(0);

        }
    }
 


    private void AddForceToBullet(GameObject bullet, Vector2 direction)
    {
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0f;
        rigid.AddForce(direction * 5, ForceMode2D.Impulse);
    }
}
