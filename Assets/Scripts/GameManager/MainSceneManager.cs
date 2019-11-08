using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    public Image blackPanel;

    [System.Serializable]
    public struct CameraRoamingInfo
    {
        public Transform from;
        public Transform to;
    }
    public CameraRoamingInfo[] cameraRoamingInfos;


    private void Start()
    {
        StartCoroutine(CameraRoamingCoroutine());
    }

    public void OnClickStart()
    {
        StartCoroutine(Starting());
    }

    private IEnumerator CameraRoamingCoroutine()
    {
        int infoCount = cameraRoamingInfos.Length;
        int index = 0;

        Transform cameraTrans = Camera.main.transform;

        float conTime;
        float maxTime;

        WaitForSeconds waitForSeconds = new WaitForSeconds(5f);

        while (true)
        {
            conTime = 0;
            maxTime = Vector3.Distance(cameraRoamingInfos[index].from.position, cameraRoamingInfos[index].to.position) / 2.5f;
            while (conTime < maxTime)
            {
                cameraTrans.position = Vector3.Lerp(cameraRoamingInfos[index].from.position, cameraRoamingInfos[index].to.position, conTime / maxTime);

                conTime += Time.deltaTime;
                yield return null;
            }
            cameraTrans.position = cameraRoamingInfos[index].to.position;

            yield return waitForSeconds;

            index++;
            if (index == infoCount)
                index = 0;
        }
    }


    private IEnumerator Starting()
    {
        blackPanel.enabled = true;

        float conTime = 0;
        float maxTime = 1;
        while(conTime < maxTime)
        {
            blackPanel.color = new Color(0, 0, 0, conTime);

            conTime += Time.deltaTime;
            yield return null;
        }
        blackPanel.color = new Color(0, 0, 0, conTime);

        SceneManager.LoadScene("Stage1");
    }
}
