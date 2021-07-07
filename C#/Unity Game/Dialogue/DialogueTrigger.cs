using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData DD;
    public UM_PlayerScript playerScript;
    public Animator anim;
    public CameraFollowScript cam;
    public bool isPlaying = false;
    public AudioSource revealSFX;
    public bool chestActive;
    public Transform playerLoc;
    public SpriteRenderer playerSR;

    public void Update()
    {
        if(!revealSFX.isPlaying && chestActive)
        {
            cam.chestFocus = false;
            anim.SetBool("open", true);
            StartCoroutine(ChestReveal());
            chestActive = false;
        }
    }

    IEnumerator ChestReveal()
    {
        yield return new WaitForSeconds(1f);
        playerScript.DialogueEnabler(DD);
        playerScript.openingChest = false;
    }

    public void Play()
    {
        //temp
        playerScript.cinematicStop = true;
        playerScript.isDialogue = true;
        if (!isPlaying)
        {
            playerScript.openingChest = true;
            FlipPlayer();
            isPlaying = true;
            revealSFX.Play();
            playerScript.lowerWorldVolume = true;
            cam.CurrentCamPos();
            cam.chestFocus = true;
            chestActive = true;
        }
        //anim.SetBool(animationString[0], true); //should not be sent to 0, should increment or change with every player input
    }

    public void FlipPlayer()
    {
        //Debug.Log(playerLoc.position.x + " " + transform.position.x);
        if (playerLoc.position.x > transform.position.x)
            playerScript.facingDir = -1;
        else
            playerScript.facingDir = 1;
    }

    public void AnimationFinished(string currentAnimation)
    {
        anim.SetBool(currentAnimation, false);
    }
}
