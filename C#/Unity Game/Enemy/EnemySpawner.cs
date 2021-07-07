using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //maybe use a dictionary in the future to store different levels in one script rather than multiple gameobjects with their own list.
    public List<GameObject> enemies = new List<GameObject>();
    //public List<Vector2> enemiesPos = new List<Vector2>();


    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
        //enemiesPos.Add(enemyPos);
    }

    public void Respawn()
    {
        //foreach (Collider2D collider in detectedObjects)
        //call Remove for each respawned enemy
        foreach(GameObject enemy in enemies)
        {
            enemy.SetActive(true);
        }
        RemoveEnemy();
    }

    public void RemoveEnemy()
    {
        enemies.Clear();
    }
}
