using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTalk : MonoBehaviour
{

    PlayerController playerController;
    

    public void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // F 키를 눌렀을 때 대화를 시작
        
    }

    
}