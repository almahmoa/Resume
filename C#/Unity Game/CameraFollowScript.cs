using UnityEngine;
using System.Collections;

public class CameraFollowScript : MonoBehaviour {

	public Controller2DScript controller;
    public Camera cam;
	public float verticalOffset;
    public float horizontalOffset = 0;
    public float setVerticalOffset = 4f;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
    public float smoothRecoveryFromBigFall = 0.0f;
    public float smoothDSDPan = 0.0f;
    public float recoveryFromBigFallTime = 0.3f;
	public float verticalSmoothTime;
    public Vector2 focusAreaSize;
    public Vector3 focusPosition;
    public Vector3 previousPosition;
    public float camSize = 8;
    public float camSizeZoom = 8;
    private Vector2 currentCamPos;

    FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;

	bool lookAheadStopped;
    public bool bigFall;
    public bool recoveringFromBigFall;
    public bool airDownAttack;

    //after hitting tigger, move camera to the right, letting you see DSD, then zoom out to see the player after a dialogue que
    public bool horizontalPan;
    public bool stopFollowingPlayer = false;
    public float xPan;
    public Transform DSDLoc;
    public bool DSDFocus;
    public Transform chestLoc;
    public bool chestFocus;
    public float smoothChestZoom = 0.0f;
    public float smoothChestZoom2 = 0.0f;
    public float smoothChestZoom3 = 0.0f;
    public float chestZoomSize = 4;
    public bool restoreCamera;
    public bool daddyFocus;
    public Transform daddyLoc;
    public float smoothDaddyZoom = 0.0f;
    public float smoothDaddyZoom2 = 0.0f;
    public bool daddyFocusOut;
    public bool chestLockOn;
    public bool ddefeated;
    public bool playerFocus;
    public bool playerFocusOut;
    public Transform playerLoc;
    public float playerZoom = 0.0f;
    public float playerZoom2 = 0.0f;
    public float playerZoom3 = 0.0f;
    public bool camPanCredit;
    public bool camPanIntro;
    public float smoothPanIntro = 0.0f;

    void Start() {
        verticalOffset = setVerticalOffset;
        focusAreaSize = new Vector2(2.5f, Camera.main.orthographicSize);
		focusArea = new FocusArea (controller.collider.bounds, focusAreaSize);
        focusPosition = focusArea.centre + Vector2.up * verticalOffset;
    }

	void LateUpdate()
    {
        if(!stopFollowingPlayer)
        {
            focusArea.Update(controller.collider.bounds);
        }

        focusPosition = focusArea.centre + Vector2.up * verticalOffset;

        if (focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if (Mathf.Sign(controller.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && controller.playerInput.x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }
            }
        }
        /*
        if (airDownAttack)
        {
            verticalOffset = Mathf.SmoothDamp(verticalOffset, 0f, ref smoothRecoveryFromBigFall, recoveryFromBigFallTime * .75f);
        }
        else if (bigFall)
        {
            verticalOffset = Mathf.SmoothDamp(verticalOffset, -8f, ref smoothRecoveryFromBigFall, recoveryFromBigFallTime * 1.5f);
        }
        else if (!bigFall)
        {
            //verticalOffset = setVerticalOffset;
            verticalOffset = Mathf.SmoothDamp(verticalOffset, setVerticalOffset, ref smoothRecoveryFromBigFall, recoveryFromBigFallTime);
            //verticalOffset += Mathf.SmoothDamp(verticalOffset, setVerticalOffset, ref smoothVelocityY, verticalSmoothTime);
        }*/
        if(horizontalPan && !chestLockOn)
            horizontalOffset = Mathf.SmoothDamp(horizontalOffset, xPan, ref smoothRecoveryFromBigFall, recoveryFromBigFallTime * .75f);
        else if (DSDFocus)
        {
            horizontalOffset = Mathf.SmoothDamp(horizontalOffset, (currentCamPos.x - DSDLoc.position.x) * -1, ref smoothDSDPan, recoveryFromBigFallTime * .75f);
        }
        else if (!chestFocus && !daddyFocus && !chestLockOn)
        {
            horizontalOffset = Mathf.SmoothDamp(horizontalOffset, 0, ref smoothDSDPan, recoveryFromBigFallTime * .75f);
        }

        if(chestFocus)
        {
            chestLockOn = true;
            horizontalOffset = Mathf.SmoothDamp(horizontalOffset, (currentCamPos.x - chestLoc.position.x) * -1, ref smoothChestZoom, 5f);
            verticalOffset = Mathf.SmoothDamp(verticalOffset, ((currentCamPos.y - chestLoc.position.y) * -1) + 3, ref smoothChestZoom2, 15f);
            camSize = Mathf.SmoothDamp(camSize, chestZoomSize, ref smoothChestZoom3, 15f);
        }
        else if(restoreCamera)
        {
            chestLockOn = false;
            horizontalOffset = 0;
            verticalOffset = 4;
            camSize = 8;
            restoreCamera = false;
        }

        if(daddyFocus)
        {
            if(ddefeated)
            {
                horizontalOffset = Mathf.SmoothDamp(horizontalOffset, (currentCamPos.x - daddyLoc.position.x)* -1, ref smoothDaddyZoom, 0.15f);
            }
            else
            {
                verticalOffset = Mathf.SmoothDamp(verticalOffset, ((currentCamPos.y - daddyLoc.position.y) * -1) + 5, ref smoothDaddyZoom, .25f);
                camSize = Mathf.SmoothDamp(camSize, 3, ref smoothDaddyZoom2, 0.15f);
            }
        }
        else if(daddyFocusOut)
        {
            verticalOffset = Mathf.SmoothDamp(verticalOffset, 4, ref smoothDaddyZoom, 1f);
            camSize = Mathf.SmoothDamp(camSize, 8, ref smoothDaddyZoom2, 0.25f);
        }

        if(playerFocus)
        {
            horizontalOffset = Mathf.SmoothDamp(horizontalOffset, (currentCamPos.x - playerLoc.position.x) * -1, ref playerZoom3, 1f);
            //verticalOffset = Mathf.SmoothDamp(verticalOffset, ((currentCamPos.y - playerLoc.position.y) * -1) + 5, ref playerZoom, 1f);
            camSize = Mathf.SmoothDamp(camSize, 6, ref playerZoom2, 0.5f);
        }
        else if(playerFocusOut)
        {
            horizontalOffset = Mathf.SmoothDamp(horizontalOffset, 0, ref playerZoom3, 1f);
            verticalOffset = Mathf.SmoothDamp(verticalOffset, 4, ref playerZoom, 1f);
            camSize = Mathf.SmoothDamp(camSize, 8, ref playerZoom2, 0.25f);
        }

        if(camPanCredit)
        {
            verticalOffset = Mathf.SmoothDamp(verticalOffset, 15, ref playerZoom, 5f);
        }

        if(camPanIntro)
        {
            verticalOffset = Mathf.SmoothDamp(verticalOffset, 4, ref smoothPanIntro, 2f);
        }

        cam.orthographicSize = camSize;

        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y - 3f, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector3.right * (currentLookAheadX - 0.1f);
 

        if (!float.IsNaN(focusPosition.x) && !float.IsNaN(focusPosition.y) && !float.IsNaN(focusPosition.z))
        {
            transform.position = focusPosition + Vector3.right * horizontalOffset + Vector3.forward * -10;
        }

    }

    public void CurrentCamPos()
    {
        currentCamPos = transform.position;
    }

    void OnDrawGizmos() {
		Gizmos.color = new Color (1, 0, 0, .5f);
		Gizmos.DrawCube (focusArea.centre, focusAreaSize);
	}

	struct FocusArea {
		public Vector2 centre;
		public Vector2 velocity;
		float left,right;
		float top,bottom;


		public FocusArea(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left + right)/2, (top + bottom)/2);
		}

		public void Update(Bounds targetBounds) {
			float shiftX = 0;
			if (targetBounds.min.x < left)
            {
				shiftX = targetBounds.min.x - left;
			}
            else if (targetBounds.max.x > right)
            {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;
            
			float shiftY = 0;
			if (targetBounds.min.y < bottom)
            {
				shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			centre = new Vector2((left + right)/2, (top + bottom)/2);
			velocity = new Vector2 (shiftX, shiftY);
		}
	}
}
