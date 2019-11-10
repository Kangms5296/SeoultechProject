using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingTextEffect : MonoBehaviour
{
    public bool PlayOnAwake;

    public float scalingSpeed;
    public float minSize;
    public float maxSize;
    public float restartDelay;

    private void Start()
    {
        if (PlayOnAwake)
            OnScaling();
    }

    public void OnScaling()
    {
        StartCoroutine(StartingScaling());
    }

    private IEnumerator StartingScaling()
    {
        RectTransform rect = GetComponent<RectTransform>();

        float conTime = 0;
        float conSize = minSize;
        while(true)
        {
            // 사이즈 업
            while(conSize < maxSize)
            {
                rect.localScale = new Vector3(conSize, conSize, 1);

                conSize += Time.deltaTime * scalingSpeed;
                yield return null;
            }
            rect.localScale = new Vector3(maxSize, maxSize, 1);

            // 사이즈 업
            while (conSize > minSize)
            {
                rect.localScale = new Vector3(conSize, conSize, 1);

                conSize -= Time.deltaTime * scalingSpeed;
                yield return null;
            }
            rect.localScale = new Vector3(minSize, minSize, 1);

            conTime = 0;
            while (conTime < restartDelay)
            {
                conTime += Time.deltaTime;
                yield return null;
            }

            yield return null;
        }
    }
}
