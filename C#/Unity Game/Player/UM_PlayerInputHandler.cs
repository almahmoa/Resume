using UnityEngine;
using UnityEngine.InputSystem;

public class UM_PlayerInputHandler : MonoBehaviour
{
    public UM_PlayerScript player;
    public Vector2 RawMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }

    private Vector2 inputVector;

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();

        if (Mathf.Abs(RawMovementInput.x) > 0.5f) //buffer for stick
        {
            NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        }
        else
        {
            NormInputX = 0;
        }
        if (Mathf.Abs(RawMovementInput.y) > 0.5f) //buffer for stick
        {
            NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
        }
        else
        {
            NormInputY = 0;
        }
        inputVector = new Vector2(NormInputX, NormInputY);
        player.SetDirectionalInput(inputVector);
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.OnJumpInputDown();
        }

        if (context.canceled)
        {
            player.OnJumpInputUp();
        }
    }

    public void OnRightInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            player.OnRightInputDown();
        }

        if (context.canceled)
        {
            player.OnRightInputUp();
        }
    }

    public void OnBottomInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.OnBottomInputDown();
        }
    }

    public void OnLeftInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.OnLeftInputDown();
        }
    }

    public struct AttackInfo //I have no idea what this was suppose to be for
    {

    }

    public void OnStartInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.OnStartInputDown();
        }
    }

    public void OnEscapeInput()
    {
        Application.Quit();
    }

}
