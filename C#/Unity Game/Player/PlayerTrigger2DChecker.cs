using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger2DChecker : MonoBehaviour
{
    public DialogueTrigger dialogueTriggerScript;
    public UM_PlayerScript playerScript;

    public void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Interactive")
        {
            dialogueTriggerScript = collision.GetComponent<DialogueTrigger>();
        }
        if(collision.tag == "Pit")
        {
            playerScript.Pit();
        }
        if(collision.tag == "PitSave")
        {
            playerScript.ledgePoint = collision.transform;
        }
        if(collision.tag == "Save")
        {
            playerScript.savePoint = collision.transform;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Interactive")
        {
            dialogueTriggerScript = null;
        }
    }

    public bool IsTriggerInteractive()
    {
        if (dialogueTriggerScript != null)
            return true;
        return false;
    }

    public void PlayDialogue()
    {
        dialogueTriggerScript.Play();
    }
}
