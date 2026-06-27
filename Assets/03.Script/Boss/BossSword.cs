using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Sword : MonoBehaviour
{
    public float Damage;

    [SerializeField]
    private GameObject sword_Effect = null; // Ä® ÃæÀü ÀÌÆåÆ®
    [SerializeField]
    private GameObject sword_Land_Effect = null; // Ä® ÂøÁö ÀÌÆåÆ®
    [SerializeField]
    private Transform effect_Destination = null;
    [SerializeField]
    public GameObject target = null; // Å¸°Ù ÇÃ·¹ÀÌ¾î

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    public void Sword_Init()
    {
        isLanding = false;
        shootStart = false;
        sword_Effect.SetActive(true);
        sword_Land_Effect.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7) // Wall Layer = 7 ¹ø ·¹ÀÌ¾î
        {
            isLanding = true;
            sword_Land_Effect.SetActive(true);
            sword_Effect.SetActive(false);
            StartCoroutine(land_Effect_Off());
        }
        else if (collision.tag == "Player")
        {
            collision.GetComponent<PlayerHealth>().OnDamage(Damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7) // Wall Layer = 7 ¹ø ·¹ÀÌ¾î
        {
            isLanding = true;
            sword_Land_Effect.SetActive(true);
            sword_Effect.SetActive(false);
            StartCoroutine(land_Effect_Off());
        }
    }

    IEnumerator land_Effect_Off()
    {
        yield return new WaitForSeconds(0.7f);

        sword_Land_Effect.SetActive(false);
    }

    [SerializeField]
    bool isLanding = false; // ¶¥¿¡ µµÂøÇß´ÂÁö Ã¼Å©
    void Sword_Effect()
    {
        if (isLanding)
            return;

        Vector3 targetPos = new Vector3(sword_Effect.transform.localPosition.x, effect_Destination.localPosition.y, 0);

        sword_Effect.transform.localPosition = Vector3.MoveTowards(sword_Effect.transform.localPosition, targetPos, 2.5f * Time.deltaTime);


        if (sword_Effect.transform.localPosition == targetPos)
            sword_Effect.transform.localPosition = new Vector3(0, -0.6f, 0);
    }

    public bool shootStart = false;
    void Sword_Dir()
    {
        if (shootStart || isLanding)
            return;

        Vector3 dir = transform.position - target.transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 7 * Time.deltaTime);
        transform.rotation = rotation;
        sword_Effect.transform.rotation = rotation;
    }

    public Vector3 targetVec;
    public void Shoot_Sword()
    {
        if (!shootStart || isLanding)
        {
            //targetVec = transform.position - target.transform.position;
            return;
        }

        transform.position -= targetVec.normalized * 20f * Time.deltaTime;
    }

    void Update()
    {
        Sword_Effect();
        Sword_Dir();
        Shoot_Sword();
    }
}
