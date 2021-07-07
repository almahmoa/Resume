using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerScript player;
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

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            player.OnAttackInputDown();
        }
    }

    public void OnRollInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            player.OnRollInputDown();
        }
    }

    public struct AttackInfo
    {

    }
}
