using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnScript : MonoBehaviour
{
    [System.Serializable]
    public class SpawnObejctInfo
    {
        public Transform target;
        public int probability;
    }
    public SpawnObejctInfo[] targetList;

    // Start is called before the first frame update
    void Start()
    {
        int maxProbability = 0;
        foreach (SpawnObejctInfo target in targetList)
            maxProbability += target.probability;

        int goal = Random.Range(0, maxProbability) + 1;
        Debug.Log(goal);
        int conProbability = 0;
        foreach(SpawnObejctInfo target in targetList)
        {
            conProbability += target.probability;
            if(conProbability >= goal)
            {
                Transform temp = Instantiate(target.target);
                temp.position = transform.position;
                temp.rotation = transform.rotation;
                Debug.Log(temp.name);
                break;
            }
        }
    }
    
}
