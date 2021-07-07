using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    [SerializeField]
    private GameObject GO;
    public int facingDir = 1;

    public static GameObjectPool Instance { get; private set; }
    public Queue<GameObject> objects = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public GameObject Get()
    {
        if (objects.Count == 0)
            AddObjects(1);
        return objects.Dequeue();
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        objects.Enqueue(objectToReturn);
    }

    private void AddObjects(int count)
    {
        var newObject = Instantiate(GO);
        newObject.SetActive(false);
        objects.Enqueue(newObject);

        //newObject.GetComponent<IGameObjectPooled>().Pool = this;
    }
}
