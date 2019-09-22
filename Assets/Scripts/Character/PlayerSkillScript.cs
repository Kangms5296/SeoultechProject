using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillScript : MonoBehaviour
{
    [Header("Rolling")]
    public float rollingDelay;
    private bool canRolling = true;
    public Image rollingDelayImage;



    public bool Rolling()
    {
        if (canRolling)
        {
            StartCoroutine(RollingCoroutine());
            return true;
        }
        return false;
    }

    IEnumerator RollingCoroutine()
    {
        canRolling = false;

        float conTime = 0;
        do
        {
            rollingDelayImage.fillAmount = 1 - (conTime / rollingDelay);

            conTime += Time.deltaTime;
            yield return null;
        } while (conTime < rollingDelay);
        rollingDelayImage.fillAmount = 0;

        canRolling = true;
    }
}
