using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaddyCinematicTriggerScript : MonoBehaviour
{
    public DaddyScript DScript;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            DScript.IntroCinematic();
            gameObject.SetActive(false);
        }
    }
}
