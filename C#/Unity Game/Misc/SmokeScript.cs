using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeScript : MonoBehaviour
{
    //on animation que, send signal for dsd to disappear or reappear (same code)
    public Animator anim;
    public Transform smokePos;
    public DarkShippieDuesScript DSDScript;
    public SummonScript summonScript;
    public DaddyScript daddyScript;
    public bool isSummon;
    public bool isDaddy;

    public void OnEnable()
    {
        transform.position = smokePos.transform.position;
    }

    public void SmokeTrigger()
    {
        if (isSummon)
            summonScript.SmokeTrigger();
        else if (isDaddy)
            daddyScript.SmokeTrigger();
        else
            DSDScript.SmokeTrigger();

    }


    public void AnimationFinished()
    {
        gameObject.SetActive(false);
    }
}
