using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "newDialogueData", menuName = "Data/Dialogue Data/Base Data")]
public class DialogueData : ScriptableObject
{
    [TextArea(10, 100)]
    public string textField;
    public DialogueData nextDD;
    //need to indicate if which text bubble used (example top or bottom, round or square)
    //work around since cant referece gameobject, use int to indicate which textbubble to reference
    public int textBubble;
    //need to indicate which text label to use (corresponding with text bubble)
    public bool hasTail;
    public float tailX;
    public float tailY;
    public int tdir; //1 = points left; -1 = points right
    public float fontSize;
    //Transistion between talkers
    public bool nSpeaker;
    //who's talking
    public bool isDSD;
    public bool isUM;
    public bool isLumby;
    public bool isDaddy;
    //
    public bool DSDWon;
    public bool DSDLost;
    public bool unpausePlayer;
    public string smallText;
    public bool restoreCamera;
    public bool zoomIn;
    public bool zoomOut;
    public bool daddyStart;
    public bool endingStart;
    public bool lumbypauseend;
    public bool toCredit;
    public bool giftTrigger;
    public bool startGame;
    public bool stopsound;
    //Camera script
    public bool isCamPanX;
    public bool isCamPanY;
    public float camPanAmountX;
    public float camPanAmountY;
    //animation trigger
    public string animationName;
}
