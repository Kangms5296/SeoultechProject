using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackDetectAreaScript : MonoBehaviour
{
    MonsterScript monsterScript;

    private void Start()
    {
        monsterScript = transform.parent.GetComponent<MonsterScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monsterScript.isPlayerInAttackDetectArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monsterScript.isPlayerInAttackDetectArea = false;
        }
    }
}
