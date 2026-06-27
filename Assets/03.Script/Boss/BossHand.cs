using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Hand : MonoBehaviour
{
    private Animator bossHandAnimator;
    [SerializeField]
    private GameObject bossLaser;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private bool attackStart = false;

    void Start()
    {
        bossHandAnimator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        MoveToTarget();
    }

    
    void MoveToTarget()
    {
        if (!attackStart)
            return;

        Vector3 targetPos = new Vector3(transform.position.x, target.transform.position.y, 0);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, 1f);

        if (transform.position == targetPos)
            attackStart = false;
    }

    public IEnumerator bossanimator_Coroutine()
    {
        attackStart = true;

        yield return new WaitForSeconds(1f);

        Attack_Start();
    }

    public void Attack_Start()
    {
        attackStart = false;
        bossLaser.SetActive(true);
    }

    public void Attack_Finish()
    {
        bossLaser.SetActive(false);
        bossHandAnimator.SetBool("isAttack", false);
    }
}
