using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Bird : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private void Start()
    {
        StartCoroutine(removeMe());
    }

    IEnumerator removeMe()
    {
        yield return new WaitForSecondsRealtime(30f);

        this.transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
    }
}
