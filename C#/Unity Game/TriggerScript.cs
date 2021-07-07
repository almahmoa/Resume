using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public EnemySpawner ES;
    public LayerMask whatIsPlayer;

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.right, .05f, whatIsPlayer);
        if (hit)
        {
            Debug.Log("hit");
            ES.Respawn();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(this.transform.position, Vector2.right);
    }
}
