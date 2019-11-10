using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavyTextEffect : MonoBehaviour
{
    private RectTransform[] childs;

    [Header("Wave Info")]
    public float perDelay;
    public float restartDelay;
    public float waveHeight;
    public float waveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        childs = GetComponentsInChildren<RectTransform>();
        
    }

    public void OnWave()
    {
        StartCoroutine(StartingWave());
    }
    
    IEnumerator StartingWave()
    {
        int conIndex = 1;

        float conTime;
        while (true)
        {
            StartCoroutine(Jumping(conIndex));

            conTime = 0;
            while (conTime < perDelay)
            {
                conTime += Time.deltaTime;
                yield return null;
            }

            conIndex++;
            if (conIndex == childs.Length)
            {
                conIndex = 1;

                conTime = 0;
                while (conTime < restartDelay)
                {
                    conTime += Time.deltaTime;
                    yield return null;
                }
            }

            yield return null;
        }
    }

    IEnumerator Jumping(int index)
    {
        float conHeight = 0;
        float xPos = childs[index].anchoredPosition.x;

        // 올라갔다가
        while (conHeight < waveHeight)
        {
            conHeight += Time.deltaTime * 100 * waveSpeed;
            childs[index].anchoredPosition = new Vector2(xPos, conHeight);

            yield return null;
        }
        conHeight = waveHeight;
        childs[index].anchoredPosition = new Vector2(xPos, conHeight);

        while(conHeight > 0)
        {
            conHeight -= Time.deltaTime * 100 * waveSpeed;
            childs[index].anchoredPosition = new Vector2(xPos, conHeight);

            yield return null;
        }
        conHeight = 0;
        childs[index].anchoredPosition = new Vector2(xPos, conHeight);
    }
}
