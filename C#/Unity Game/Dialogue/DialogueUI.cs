using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> textLabel;
    [SerializeField] private float typewriterSpeed = 25f;
    public DialogueData currentDD;

    public GameObject[] textBubbles;
    public RectTransform tailRT;

    public bool isDialogueFinished;
    public bool canSkipDialogue;

    public Transform playerPos;
    public UM_PlayerScript playerScript;
    public DarkShippieDuesScript DSDScript;
    public DaddyScript daddyScript;
    public LumbyScript lumbyScript;
    public CameraFollowScript cam;

    public void Run(string textToType)
    {
        if (currentDD.fontSize != 0)
            textLabel[currentDD.textBubble].fontSize = currentDD.fontSize;
        else
            textLabel[currentDD.textBubble].fontSize = 57.93f;

        if (currentDD.lumbypauseend)
            playerScript.music1.Stop();
        if (currentDD.stopsound)
            playerScript.music2.Stop();

        if (currentDD.nSpeaker)
        {
            textBubbles[currentDD.textBubble].GetComponent<DialogueAnimationScript>().NewSpeak();
        }
        if (currentDD.hasTail)
        {
            tailRT.localPosition = new Vector3(currentDD.tailX, currentDD.tailY, 0);
            //tailRT.localScale = new Vector3(currentDD.tdir, 1, 1);
            if (currentDD.isUM)
                tailRT.localScale = new Vector3(-playerScript.facingDir, 1, 1);
            else if (currentDD.isDSD)
                tailRT.localScale = new Vector3(-DSDScript.facingDir, 1, 1);
            else if (currentDD.isDaddy)
                tailRT.localScale = new Vector3(-daddyScript.facingDir, 1, 1);
            else if(currentDD.isLumby)
                tailRT.localScale = new Vector3(playerScript.facingDir, 1, 1);
        }
        if (currentDD.isCamPanX)
        {
            cam.xPan = currentDD.camPanAmountX;
            cam.horizontalPan = true;
        }
        else
        {
            cam.horizontalPan = false;
        }

        //Animation - Sprite change for speaker
        if (currentDD.animationName != null)
        {
            if (currentDD.isUM)
                playerScript.DialogueAnimation(currentDD.animationName);
            else if (currentDD.isDSD)
                DSDScript.DialogueAnimation(currentDD.animationName);
            else if (currentDD.isDaddy)
                daddyScript.DialogueAnimation(currentDD.animationName);
            else if (currentDD.isLumby)
                lumbyScript.DialogueAnimation(currentDD.animationName);
        }
        isDialogueFinished = false;
        canSkipDialogue = true;
        StartCoroutine(TypeText(textToType));
    }

    private IEnumerator TypeText(string textToType)
    {
        textLabel[currentDD.textBubble].text = string.Empty;
        //Typewriter effect -----------------------------------------
        float t = 0;
        int charIndex = 0;

        while(charIndex < textToType.Length && canSkipDialogue)
        {
            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            textLabel[currentDD.textBubble].text = textToType.Substring(0, charIndex);

            yield return null;
        }
        //-------------------------------------------------------------
        //when text is done being written
        //set new line indicator active
        textLabel[currentDD.textBubble].text = textToType;
        isDialogueFinished = true;
        canSkipDialogue = false;
        if (currentDD.smallText != "")
            playerScript.SmallTextAnimation(currentDD.smallText);
        //enable GO for end line indicator.
    }

    public void PlayerInput()
    {
        //see if player has made an input during dialogue
        //if dialogue is not finished, it will boot out of the while loop to finish the text instantly
        if (canSkipDialogue)
            canSkipDialogue = false;
        else if(isDialogueFinished && !canSkipDialogue)
        {
            if (currentDD.giftTrigger)
                lumbyScript.GiftAnimation();
            if (currentDD.startGame)
                playerScript.DialogueIntroFinished();
            if (currentDD.endingStart)
                playerScript.EndingScenes();
            //close the motuh of the speaker after they are done talking
            if (currentDD.animationName == "openMouth")
            {
                if (currentDD.isUM)
                    playerScript.DialogueAnimation("closeMouth");
            }
            if(currentDD.isLumby)
            {
                if (currentDD.animationName == "reMOHat" || currentDD.animationName == "sadMOHat" || currentDD.animationName == "sadMONoHat")
                        lumbyScript.DialogueAnimation("closeMouth");
            }

            if (currentDD.restoreCamera)
            {
                cam.restoreCamera = true;
                playerScript.lowerWorldVolume = false; //for the chest
                playerScript.music1.volume = 0.35f;
            }
            //play end animation for end line indicator, disables GO after animation for end line indicator.
            if (currentDD.nextDD != null)
            {
                currentDD = currentDD.nextDD;
                Run(currentDD.textField);
            }
            else
            {
                if (currentDD.lumbypauseend)
                    lumbyScript.LumbyPauseEnd();
                if (currentDD.toCredit)
                    playerScript.ToCredit();
                //play the animation to close the bubble
                //in this case as a placement , it will just disable
                textBubbles[currentDD.textBubble].GetComponent<DialogueAnimationScript>().FinishSpeak(); // need to add finish animation to all text bubble
                if (currentDD.isDSD)
                {
                    if (currentDD.DSDWon)
                        playerScript.PlayerIsDead();
                    else if (currentDD.DSDLost)
                        DSDScript.DSDEndingDialogueFinished();
                    else
                        DSDScript.HealthBarAppear();
                }
                else if (currentDD.isDaddy)
                {
                    if (currentDD.zoomIn)
                        daddyScript.ZoomIn();
                    else if (currentDD.zoomOut)
                        daddyScript.ZoomOut();
                }
                if (currentDD.daddyStart)
                    daddyScript.HealthBarAppear();
                //textBubbles[currentDD.textBubble].SetActive(false); // set in animation script
                //send player that it can move
                playerScript.isDialogue = false;
                if (currentDD.unpausePlayer)
                    playerScript.cinematicStop = false;
            }
        }
        //if dialogue is finished, the script will check if there is a nextDD
    }

    public void OnPlayerEnable(DialogueData currentDD)
    {
        this.currentDD = currentDD;
        //tail.rect = new Rect(currentDD.tRight, currentDD.tTop, currentDD.tLeft, currentDD.tBottom);

        // might wait for animation que, which is sent to player to enable
        //this que can be sent from other GO
        textBubbles[currentDD.textBubble].SetActive(true);

        Run(currentDD.textField);
    }
}
