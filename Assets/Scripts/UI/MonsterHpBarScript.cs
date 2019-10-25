using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHpBarScript : MonoBehaviour
{
    public RectTransform changingHp;
    public RectTransform inHp;

    [HideInInspector] public Transform target;

    private float conChangingHpRatio;
    private float conInHpRatio;
    private float newHpRatio;

    private const float minHpPos = -305;
    private const float maxHpPos = 0;

    private bool isChangingHpDecrease = false;
    private bool isInHpDecrease = false;

    private void LateUpdate()
    {
        transform.position = target.position;
    }

    public void Init(Transform target)
    {
        conChangingHpRatio = 1;
        conInHpRatio = 1;

        inHp.anchoredPosition = new Vector2(0, 0);
        changingHp.anchoredPosition = new Vector2(0, 0);

        this.target = target;

        gameObject.SetActive(true);
    }


    public void DecreaseHp(float newHpRatio)
    {
        this.newHpRatio = newHpRatio;

        if (!isChangingHpDecrease)
            StartCoroutine(ChangingHpDecreaseCoroutine());

        if (!isInHpDecrease)
            StartCoroutine(InHpDecreaseCoroutine());
    }

    public void Die()
    {
        transform.position = new Vector3(1000, 0, 0);

        gameObject.SetActive(false);
    }
    
    private IEnumerator ChangingHpDecreaseCoroutine()
    {
        isChangingHpDecrease = true;

        while (conChangingHpRatio - newHpRatio > 0.01f )
        {
            conChangingHpRatio = Mathf.Lerp(conChangingHpRatio, newHpRatio, Time.deltaTime);
            changingHp.anchoredPosition = new Vector2(minHpPos * (1 - conChangingHpRatio), 0);
            yield return null;
        }
        conChangingHpRatio = newHpRatio;
        changingHp.anchoredPosition = new Vector2(minHpPos * (1 - conChangingHpRatio), 0);

        isChangingHpDecrease = false;
    }

    private IEnumerator InHpDecreaseCoroutine()
    {
        isInHpDecrease = true;

        while (conInHpRatio - newHpRatio > 0.01f)
        {
            conInHpRatio = Mathf.Lerp(conInHpRatio, newHpRatio, Time.deltaTime * 5);
            inHp.anchoredPosition = new Vector2(minHpPos * (1 - conInHpRatio), 0);
            yield return null;
        }
        conInHpRatio = newHpRatio;
        inHp.anchoredPosition = new Vector2(minHpPos * (1 - conInHpRatio), 0);

        isInHpDecrease = false;
    }
}
