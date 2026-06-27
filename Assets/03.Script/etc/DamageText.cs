using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private float textSpeed = 1f;
    private float alphaSpeed = 2f;
    private float destroyTime = 1f;
    private TextMeshPro textMeshPro;
    private Color originalColor;
    public int damage;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        if (textMeshPro != null)
        {
            originalColor = textMeshPro.color;
        }
    }

    private void OnEnable()
    {
        Initialize();
        if (textMeshPro != null)
        {
            Color color = originalColor;
            color.a = 1f;
            textMeshPro.color = color;
        }
        StartCoroutine(FadeAndDisable());
    }
    public void Initialize()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshPro>(); // 다시 컴포넌트를 가져오는 시도
            if (textMeshPro == null)
            {
                return; // 여기서 초기화를 할 수 없음
            }
        }

        textMeshPro.text = damage.ToString();
    }
    private IEnumerator FadeAndDisable()
    {
        while (textMeshPro.color.a > 0)
        {
            Color currentColor = textMeshPro.color;
            currentColor.a -= Time.deltaTime * alphaSpeed;
            textMeshPro.color = currentColor;
            transform.Translate(new Vector3(0, textSpeed * Time.deltaTime, 0));
            yield return null;
        }
        yield return new WaitForSeconds(destroyTime);
        ObjectPool.Instance.ReturnObject(gameObject);
    }
}