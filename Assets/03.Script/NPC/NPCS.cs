using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCS : MonoBehaviour, INPC
{
    public string communications;

    public UIManager uiManager;

    public string playerLayerName = "Player";

    public GameObject targetObject;
    public GameObject talkUI;
    public Text text;

    protected virtual void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            targetObject = gameObject.transform.GetChild(0).gameObject;
            talkUI = uiManager.transform.GetChild(0).gameObject;
            text = talkUI.transform.GetChild(0).GetComponent<Text>();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {        
                targetObject.SetActive(true);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            targetObject.SetActive(false);
            talkUI.SetActive(false);
        }
    }

    public virtual void Talk()
    {
        talkUI.SetActive(true);
        text.text = communications;
    }
}
