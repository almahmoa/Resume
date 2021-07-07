using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingArmSummonScript : MonoBehaviour
{
    [Header("Interactive Variable")]
    public GameObject lockOn;
    public bool isSwinging = false;
    public float swingXOffset = 0;
    public float swingYOffset = 0;
    public float anchorXOffset = 0;
    public float anchorYOffset = 0;
    private float smoothSwing = 0.0f;
    public float swingAnchorTime = 0.3f;

    [Header("Components")]
    public SpriteRenderer theSR;
    public UM_PlayerScript playerScript;
    public Animator anim;
    public Animator animOA;
    public HingeJoint2D theHJ;
    public Rigidbody2D theRB;

    [Header("Summon")]
    public GameObject summonGO;
    public float OffsetX = 0;
    public float OffsetY = 0;
    public float OffsetYPos = 0;

    private void OnEnable()
    {
        /*
        if (playerScript.facingDir == -1)
        {
            theSR.flipX = true;
        }
        else if (playerScript.facingDir == 1)
        {
            theSR.flipX = false;
        }
        */
        theHJ.autoConfigureConnectedAnchor = false;
        transform.position = new Vector2(summonGO.transform.position.x + OffsetX, summonGO.transform.position.y + OffsetY);
        //anim.SetBool("swingStart", true);
        //animOA.SetBool("swingStart", true);
        //swingXOffset = 2;
        swingYOffset = lockOn.transform.position.y - summonGO.transform.position.y + OffsetYPos;
        theHJ.connectedAnchor = new Vector2(lockOn.transform.position.x + -.25f * playerScript.facingDir, lockOn.transform.position.y); //try incremementing to this position on update starting at currentposition when hooked to summon pos
    }

    void FixedUpdate()
    {
        //swingXOffset = Mathf.SmoothDamp(swingXOffset, 0, ref smoothSwing, swingAnchorTime);
        swingYOffset = Mathf.Lerp(swingYOffset, 5.32f, swingAnchorTime * Time.deltaTime);
        //Debug.Log(swingYOffset);
        theHJ.anchor = new Vector2(0, swingYOffset);
    }

    public void AnimationFinished(string currentAnimation)
    {
        //anim.SetBool(currentAnimation, false);
        //animOA.SetBool(currentAnimation, false);
        //isSwinging = true;
    }
}
