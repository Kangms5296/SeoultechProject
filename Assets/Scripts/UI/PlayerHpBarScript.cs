using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBarScript : MonoBehaviour
{
    public Text hpText;

    public RectTransform inHp;
    public RectTransform changingHp;

    private float inHpConRatio = 1;
    private float inHpGoalRatio;
    private bool isInHpCoroutine;

    private float changingHpRatio = 1;
    private float changingHpGoalRatio;
    private bool isChangingCoroutine;

    private const float posXInMinHp = -410f;
    private const float posXInMaxHp = 0f;

    public void RecoveryHp(int conHp, int maxHp)
    {
        inHpGoalRatio = (float)conHp / maxHp;
        if (!isInHpCoroutine)
            StartCoroutine(InHpRecoverCoroutine());

        string hp = conHp.ToString("000") + "   /   " + maxHp.ToString("000");
    }

    public void DecreaseHp(int conHp, int maxHp)
    {
        inHpGoalRatio = (float)conHp / maxHp;
        if (!isInHpCoroutine)
            StartCoroutine(InHpDecreaseCoroutine());

        changingHpGoalRatio = (float)conHp / maxHp; ;
        if (!isChangingCoroutine)
            StartCoroutine(ChangingHpDecreaseCoroutine());

        hpText.text = conHp.ToString("000") + "   /   " + maxHp.ToString("000");
    }

    private IEnumerator InHpRecoverCoroutine()
    {
        isInHpCoroutine = true;

        while (inHpConRatio > inHpGoalRatio)
        {
            inHp.anchoredPosition = new Vector2(posXInMinHp * (1 - inHpConRatio), 0);

            inHpConRatio -= Time.deltaTime ;
            yield return null;
        }
        inHp.anchoredPosition = new Vector2(posXInMinHp * (1 - inHpGoalRatio), 0);

        isInHpCoroutine = false;
    }

    private IEnumerator InHpDecreaseCoroutine()
    {
        isInHpCoroutine = true;

        while (inHpConRatio > inHpGoalRatio)
        {
            inHp.anchoredPosition = new Vector2(posXInMinHp * (1 - inHpConRatio), 0);

            inHpConRatio -= Time.deltaTime;
            yield return null;
        }
        inHp.anchoredPosition = new Vector2(posXInMinHp * (1 - inHpGoalRatio), 0);

        isInHpCoroutine = false;
    }

    private IEnumerator ChangingHpDecreaseCoroutine()
    {
        isChangingCoroutine = true;

        while (changingHpRatio > changingHpGoalRatio)
        {
            changingHp.anchoredPosition = new Vector2(posXInMinHp * (1 - changingHpRatio), 0);

            changingHpRatio -= Time.deltaTime * 0.2f;
            yield return null;
        }
        changingHp.anchoredPosition = new Vector2(posXInMinHp * (1 - changingHpGoalRatio), 0);

        isChangingCoroutine = false;
    }
}
