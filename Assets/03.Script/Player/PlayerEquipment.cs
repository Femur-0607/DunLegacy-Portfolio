using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public Transform ThandPos;
    public Transform RhandPos;
    public Transform LhandPos;
    private Weapons currentWeapon;
    private CinemachineImpulseSource impulseSource;

    private bool isAttacking = false;
    private float attackCooldown = 0f;


    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        PlayerManager playerManager = PlayerManager.Instance;
    }
    private void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Z) && currentWeapon != null && !isAttacking && attackCooldown <= 0f)
        {
            StartCoroutine(AttackWithDelay(currentWeapon.weaponData.attackSpeed));
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        IItem item = other.GetComponent<IItem>();

        if (item != null && other.gameObject.tag == "Item")
        {
            item.Use(LhandPos, null);
            currentWeapon = other.GetComponent<Weapons>();
            PlayerManager playerManager = PlayerManager.Instance;
            if (playerManager != null)
            {
                playerManager.hasLongsword = true;
                playerManager.SaveData();
            }
        }
    }

    // °ø°Ý µô·¹ÀÌ
    private IEnumerator AttackWithDelay(float delay)
    {
        isAttacking = true;
        currentWeapon.Attack();
        attackCooldown = delay;
        impulseSource.GenerateImpulse();
        yield return new WaitForSeconds(delay);
        isAttacking = false;
    }
}
