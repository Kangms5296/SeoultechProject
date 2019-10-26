using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrail : MonoBehaviour {
    
    [Header("타겟 스킨메쉬")]
    public GameObject TargetSkinMesh;

    [Header("이펙트 출력할 속도간격. 낮을 수록 부하가 심해집니다.")]
    [Range(0, 1)]
    public float ExportSpeedDelay = 0.1f;

    [Header("이펙트 출력시간. 체크할 경우 EffectLifeTime(초)동안 이펙트를 출력합니다. 체크 해제시 영구적으로 출력합니다.")]
    public bool UseLifeTime = false; 
    public float EffectLifeTime = 3;

    [Header("------------------------------------------------------------------------------------------------------------------------------------------------------")]
    [Header("쉐이더 변수 이름. 0~1까지 올라갑니다.")]
    public string ValueName;

    [Header("0 -> 1 속도 딜레이. 낮을 수록 빨라집니다. 0값이 되지 않도록 해주세요.")]
    [Range(0, 1)]
    public float ValueTimeDelay = 0.1f;

    [Header("변수 더할 값. 0.1이라면 1이 될때까지 10번 반복됨. 값이 높을 수록 가볍습니다.")]
    [Range(0, 1)]
    public float ValueDetail = 0.1f;
    

    private bool NeedObject;
    private void OnEnable()
    {
        if (TargetSkinMesh == null)
        {
            Debug.Log("<color=red>" + "타겟 스킨메쉬가 없습니다." + "</color>", this);
        }
        if (ValueName == "")
        {
            Debug.Log("<color=red>" + "변경할 쉐이더 변수이름이 존재하지 않습니다." + "</color>", this);
        }
        
        /*
        if(TargetSkinMesh != null && ValueName != "")
        {
            StopAllCoroutines();
            StartCoroutine("GhostStart");

            if(UseLifeTime == true)
            {
                StartCoroutine("TimerStart");
            }
        }
        */
    }

    IEnumerator GhostStart()
    {
        for (int e = 0; e < e + 1; e++)
        {
            NeedObject = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf == false)
                {
                    transform.GetChild(i).gameObject.transform.position = TargetSkinMesh.transform.position;
                    transform.GetChild(i).gameObject.transform.rotation = TargetSkinMesh.transform.rotation;
                    transform.GetChild(i).gameObject.GetComponent<MotionTrailRenderer>().SkinMesh = TargetSkinMesh.GetComponent<SkinnedMeshRenderer>();

                    transform.GetChild(i).gameObject.GetComponent<MotionTrailRenderer>().ValueName = ValueName;
                    transform.GetChild(i).gameObject.GetComponent<MotionTrailRenderer>().ValueTimeDelay = ValueTimeDelay;
                    transform.GetChild(i).gameObject.GetComponent<MotionTrailRenderer>().ValueDetail = ValueDetail;
                    transform.GetChild(i).gameObject.SetActive(true);
                    NeedObject = true;
                    break;
                }
            }
            if(NeedObject == false)
            {
                Debug.Log("<color=red>" + "Ghost 오브젝트가 부족합니다." + "</color>" + "\n 해결방법1 : Export Speed Delay를 올려주세요. \n 해결방법2 : Value Time Delay를 내려주세요. \n 해결방법3 : Value Detail을 올려주세요. \n 해결방법4 : Ghost를 더 복제 해주세요.");
            }
            yield return new WaitForSeconds(ExportSpeedDelay);
        }
        yield return null;
    }
    IEnumerator TimerStart()
    {
        yield return new WaitForSeconds(EffectLifeTime);
        StopAllCoroutines();
        yield return null;
    }

    private bool isMotionTrailOn = false;
    private Coroutine motionTrailCoroutine;

    public void OnMotionTrail()
    {
        if (isMotionTrailOn)
            return;

        motionTrailCoroutine = StartCoroutine(GhostStart());
        isMotionTrailOn = true;
    }

    public void OffMotionTrail()
    {
        if (!isMotionTrailOn)
            return;

        StopCoroutine(motionTrailCoroutine);
        isMotionTrailOn = false;
    }

}
