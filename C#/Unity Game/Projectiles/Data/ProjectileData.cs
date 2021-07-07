using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newProjectileData", menuName = "Data/Projectile Data/Base Data")]
public class ProjectileData : ScriptableObject
{
    public float endSpeed;
    public float checkRadius = 0.5f;
    public float minRangeRadius = 0.5f;
    public float maxRangeRadius = 0.5f;
    public float acceleration;
    public LayerMask whatIsGround;
    public LayerMask whatIsDamagable;

    public string target; //use to set transform by name or tag
    public bool isHoming;
    public Sprite sprite;
    public float size;
    public Color color;

    public float setLifeTime;
    //animator
    //add drift multiplier
    //timer to death
    //explosion on death/ collision? applies to all size of impact are the variable
    //high start speed to slow down
    //projectile that move on a path
    //bool gravity and gravity float
}
