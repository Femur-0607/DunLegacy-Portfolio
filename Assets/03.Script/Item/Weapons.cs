using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour, IItem
{
    private static Weapons _instance;
    public static Weapons Instance { get { return _instance; } }

    public WeaponsData weaponData;
    private Animator animator;
    private GameObject attackEffect;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        attackEffect = transform.Find("SwingFX")?.gameObject;
    }

    public void Use(Transform target, GameObject fx)
    {
        transform.parent = target.transform;
        transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        SoundManager.Instance.effectSource.PlayOneShot(SoundManager.Instance.Player_Weapon[0]);
        StartCoroutine(AttackEffect());
    }

    private IEnumerator AttackEffect()
    {
        attackEffect.SetActive(true);
        attackEffect.GetComponent<BoxCollider2D>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        attackEffect.SetActive(false);
        attackEffect.GetComponent<BoxCollider2D>().enabled = false;
    }
}
