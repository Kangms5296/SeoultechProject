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

    [Header("Focus")]
    public float focusDealy;
    private bool canFocus = true;
    public Image focusDelayImage;

    public bool Rolling()
    {
        if (canRolling)
        {
            StartCoroutine(RollingCoroutine());
            return true;
        }
        return false;
    }

    public bool Focus()
    {
        if (canFocus)
        {
            StartCoroutine(FocusCoroutine());
            return true;
        }
        return false;
    }

    public bool CanFicus()
    {
        return canFocus;
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

    IEnumerator FocusCoroutine()
    {
        canFocus = false;

        float conTime = 0;
        do
        {
            focusDelayImage.fillAmount = 1 - (conTime / focusDealy);

            conTime += Time.deltaTime;
            yield return null;
        } while (conTime < focusDealy);
        focusDelayImage.fillAmount = 0;

        canFocus = true;
    }
}
