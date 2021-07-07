using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DSDCinemaTriggerScript : MonoBehaviour
{
    public DarkShippieDuesScript DSDScript;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            DSDScript.IntroCinematic();
            gameObject.SetActive(false);
        }
    }
}
