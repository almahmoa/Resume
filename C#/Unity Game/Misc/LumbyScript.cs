using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumbyScript : MonoBehaviour
{
    public Animator anim;
    public string currentAnim;
    public UM_PlayerScript playerScript;
    public DialogueData finalWord;
    public SpriteRenderer theSR;

    public DialogueData introDD;

    public void AppearNoHat()
    {
        DialogueAnimation("neutralNoHat");
    }

    public void AppearHat()
    {
        DialogueAnimation("neutralHat");
    }

    public void DialogueAnimation(string nextAnim)
    {
        if (nextAnim != "")
        {
            if (nextAnim == "closeMouth")
            {
                Debug.Log(currentAnim);
                if(currentAnim == "reMOHat")
                {
                    anim.SetBool("reMOHat", false);
                    currentAnim = "reMCHat";
                    anim.SetBool(currentAnim, true);
                }
                else if (currentAnim == "sadMOHat")
                {
                    anim.SetBool("sadMOHat", false);
                    currentAnim = "sadMCHat";
                    anim.SetBool(currentAnim, true);
                }
                else if (currentAnim == "sadMONoHat")
                {
                    anim.SetBool("sadMONoHat", false);
                    currentAnim = "sadMCNoHat";
                    anim.SetBool(currentAnim, true);
                }
            }
            else if (nextAnim != currentAnim)
            {
                if (currentAnim != "")
                    anim.SetBool(currentAnim, false);
                anim.SetBool(nextAnim, true);
                currentAnim = nextAnim;
            }
        }
        else if (currentAnim != "")
        {
            anim.SetBool(currentAnim, false);
            currentAnim = "";
        }
    }

    public void LumbyPauseEnd()
    {
        anim.SetTrigger("lumbyShockedAnim");
        //playerScript.music1.volume = 0.35f;
        playerScript.music1.Play();
        playerScript.muteSoundCredit = false;
        playerScript.restoremusicforcredit = true;
    }

    public void FinalWord()
    {
        theSR.flipX = true;
        playerScript.DialogueEnabler(finalWord);
    }

    public void GiftAnimation()
    {
        DialogueAnimation("");
        DialogueAnimation("recieveHat");
    }

    public void NewHat()
    {
        DialogueAnimation("shockedHat");
        playerScript.DialogueEnabler(introDD);
    }

    public void Disappear()
    {
        playerScript.StartTheGame();
        gameObject.SetActive(false);
    }
}
