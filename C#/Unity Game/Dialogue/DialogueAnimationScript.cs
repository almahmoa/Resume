using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAnimationScript : MonoBehaviour
{
    public Animator anim;
    //public Animator tailAnim;
    //public Animator fontAnim;
    public float finTimer;
    public float setFinTimer = 2f;
    public bool isFin;

    public void Update()
    {
        if (isFin && finTimer > 0)
            finTimer -= Time.deltaTime;
        if(isFin && finTimer <= 0)
            gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        isFin = false;
        anim.SetBool("fSpeak", false);
        //tailAnim.SetBool("fSpeak", false);
    }

    public void NewSpeak()
    {
        anim.SetTrigger("nSpeak");
        //tailAnim.SetTrigger("nSpeak");
    }

    public void FinishSpeak()
    {
        anim.SetBool("fSpeak", true);
        //tailAnim.SetBool("fSpeak", true);
        //fontAnim.SetTrigger("fSpeak");
        finTimer = setFinTimer;
        isFin = true;
    }
}
