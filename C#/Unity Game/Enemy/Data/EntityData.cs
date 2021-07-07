using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]
public class EntityData : ScriptableObject
{
    [Header("Move Variable")]
    public float accelerationTimeGrounded = 0.1f;
    public float moveSpeed = 7;
    public float chargeSpeed = 12;
    public float chargeTime = 2f;

    [Header("Jump Variable")]
    public float JumpHeight = 4;
    public float timeToJumpApex = .4f;
    public int amountOfJumps;
    public int setAmountOfJumps = 1;
    public bool isJumping;

    [Header("Wall Jump Variable")]
    public Vector2 wallLeap;
    public bool wallJump;
    public bool wallAir;

    [Header("Wall Slide Variable")]
    public float wallSlideSpeedMax = 3;
    public bool wallSlide;
    public int wallDirX;

    [Header("In Air Variable")]
    public float accelerationTimeAirborne = .2f;
    public bool isFlying = false;

    [Header("Attack Variable")]
    public Transform attackPosition;
    public LayerMask whatIsDamagable;
    public List<Vector2> hitboxDistance;
    public List<Vector2> hitboxSize;
    public int attackCounterLimit = 0;

    [Header("Hit Variable")]
    public float hitTime = 0.25f;
    public float damageHopSpeed = 3f;
    public float stunResistance = 3f;
    public float stunRecoveryTime = 2f;

    [Header("Check Variable")]
    public float wallCheckDistance = 0.2f;
    public float ledgeCheckDistance = 3;
    public float ceilingCheckDistance = 0.2f;
    public float minAgroDistance = 3f;
    public float minViewAngle = 60;
    public float maxAgroDistance = 4f;
    public float maxViewAngle = 60;
    public Vector2 ledgeCheckBoxSize;
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    [Header("Miscellaneous Variable")]
    public int maxHealth = 30;
    public float minIdleTime;
    public float maxIdleTime;
    public float minMoveTime;
    public float maxMoveTime;
    public Vector3 velocity;
    public float cooldownTimer = 1f;
}
