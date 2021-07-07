using UnityEngine;

[CreateAssetMenu(fileName = "newSummonData", menuName = "Data/Summon Data/Base Data")]
public class SummonData : ScriptableObject
{
    [Header("Move Variable")]
    public float moveSpeed = 8;
    public float baseSpeed = 8;
    public float maxSpeed = 15;

    [Header("Jump Variable")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    public float accelerationTimeAirborne = 0.2f;

    [Header("Box Variable")]
    public float sizeX = 2f;
    public float sizeY = 3.5f;
    public float offsetX = 0;
    public float offsetY = 0;

    [Header("Other Variables")]
    public int maxHealth = 3;
    public string summonName = "none";
    public float setDelayTimeWalk = 0.35f;
    public float setDelayTimeJump = 0.35f;
    public float teleportDistance = 15;
    public bool canUnsummon = false;
    public float unsummonTime = 0;

    [Header("Attack Variable")]
    public bool canAttack = false;

    [Header("Interactive Variable")]
    public bool canSwing = true;
    public bool canFollow = true;
}
