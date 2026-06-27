using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public string moveAxisName = "Horizontal";

    public static float move {  get; private set; }

    private void Update()
    {
        move = Input.GetAxis(moveAxisName);
    }
}