using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponCrateInstantiateObject
{
    public WeaponScript weapon;
    public int probability;
}

public class WeaponCrateScript : BeatableObjectScript
{
    public Transform cover;
    public Transform body;

    public int minItemCount;                                // 최소 생성 아이템 수
    public int maxItemCount;                                // 최대 생성 아이템 수
    private Transform dropedWeaponParent;                   // 생성한 아이템이 들어갈 Parent Transform

    public WeaponCrateInstantiateObject[] weaponList;       // Crate에서 나올 아이템 종류
    private int weaponListProbabilitySum;                   // Crate에서 나올 아이템 들의 각 확률의 합

    private bool isCoverUpCoroutinePlaying;
    private Coroutine coverUpCoroutine;
    private float beforeHeight;

    private bool isCoverRotateCoroutinePlaying;
    private Coroutine coverRotateCoroutine;

    private void Start()
    {
        weaponListProbabilitySum = 0;
        foreach (WeaponCrateInstantiateObject weapon in weaponList)
            weaponListProbabilitySum += weapon.probability;

        beforeHeight = cover.localPosition.y;

        dropedWeaponParent = GameObject.Find("Droped Weapon Parent").GetComponent<Transform>();
    }

    public override void Break()
    {
        canHit = false;

        if (isCoverUpCoroutinePlaying)
            StopCoroutine(coverUpCoroutine);
        coverUpCoroutine = StartCoroutine(CoverUpCoroutine(2f, 1f));

        if (isCoverRotateCoroutinePlaying)
            StopCoroutine(coverRotateCoroutine);
        coverRotateCoroutine = StartCoroutine(CoverRatotateCoroutine(-15f, 1f));

        int createWeaponVelue = Random.Range(minItemCount, maxItemCount + 1);
        for (int i = 0; i < createWeaponVelue; i++)
        {
            int temp = Random.Range(0, 360);
            int angle = temp - temp % 60;
            StartCoroutine(WeaponDropCoroutine(angle, 1.2f, 1.5f));
        }

    }

    public override void Hit()
    {
        conHitCount++;

        if (conHitCount >= maxHitCount)
            Break();
        else
        {
            if (isCoverUpCoroutinePlaying)
                StopCoroutine(coverUpCoroutine);
            coverUpCoroutine = StartCoroutine(CoverUpCoroutine(0.1f, 2.5f));
        }
    }

    IEnumerator CoverUpCoroutine(float height, float speed)
    {
        isCoverUpCoroutinePlaying = true;

        float time = 0;
        while(time < 1)
        {
            cover.localPosition = new Vector3(0, beforeHeight + Mathf.Sin(Mathf.Deg2Rad * 180 * time) * height, 0);
            time += Time.deltaTime * speed;
            yield return null;
        }
        cover.localPosition = new Vector3(0, beforeHeight, 0);

        coverUpCoroutine = null;
        isCoverUpCoroutinePlaying = false;
    }

    IEnumerator CoverRatotateCoroutine(float angle, float speed)
    {
        isCoverRotateCoroutinePlaying = true;

        float time = 0;
        while (time < 1)
        {
            cover.localEulerAngles = new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * 90 * time) * angle, 0);
            time += Time.deltaTime * speed;
            yield return null;
        }
        cover.localEulerAngles = new Vector3(0, angle, 0);

        coverUpCoroutine = null;
        isCoverRotateCoroutinePlaying = false;
    }

    IEnumerator WeaponDropCoroutine(float angle, float height, float speed)
    {
        WeaponScript newWeapon = null;
        Transform newWeaponTrans = null;

        int conProbabilitySum = 0;
        int probability = Random.Range(0, weaponListProbabilitySum) + 1;
        foreach (WeaponCrateInstantiateObject weapon in weaponList)
        {
            conProbabilitySum += weapon.probability;
            if(conProbabilitySum >= probability)
            {
                newWeapon = Instantiate(weapon.weapon, dropedWeaponParent);
                newWeaponTrans = newWeapon.transform;

                break;
            }
        }

        float time = 0;
        while (time < 1)
        {
            newWeaponTrans.localPosition = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * time, Mathf.Sin(Mathf.Deg2Rad * 180 * time) * height, Mathf.Sin(angle * Mathf.Deg2Rad) * time);
            time += Time.deltaTime * speed;
            yield return null;
        }
        newWeaponTrans.localPosition = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(Mathf.Deg2Rad * 180) * height, Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}
