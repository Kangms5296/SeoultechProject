using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoundManager : MonoBehaviour
{
    private static RoundManager _instance = null;

    public static RoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(RoundManager)) as RoundManager;

                if (_instance == null)
                    Debug.LogError("There's no active RoundManager object");
            }

            return _instance;
        }
    }

    [System.Serializable]
    public struct PhaseInfo
    {
        public Transform respawnSpotParent;

        [System.Serializable]
        public struct RespawnInfo
        {
            public string monsterName;
            public int respawnCount;
        }
        public List<RespawnInfo> respawnInfo;
    }
    
    [System.Serializable]
    public struct RoundInfo
    {
        // 이번 라운드의 페이즈 정보
        public List<PhaseInfo> phaseInfos;

        // 이번 라운드 구분 벽
        public GameObject roundDivisionWall;

        // 다음 라운드 시작 트리거
        public GameObject nextRoundTrigger;
    }
    public List<RoundInfo> roundInfos;

    private int conMonsterCount;

    private bool[] roundClear;

    private void Start()
    {
        roundClear = new bool[roundInfos.Count];
    }

    public void RoundStart(int roundIndex)
    {
        // 이미 클리어한 Round는
        if (roundClear[roundIndex])
            // 무시
            return;
        roundClear[roundIndex] = true;

        StartCoroutine(RoundCoroutine(roundIndex));
    }


    public void MonsterDie()
    {
        conMonsterCount--;
    }
    

    private IEnumerator RoundCoroutine(int roundIndex)
    {
        // 라운드 간 다른 지역으로의 이동 제한
        roundInfos[roundIndex].roundDivisionWall.SetActive(true);

        // 이번 라운드의 각 페이즈를 시작
        int phaseMaxCount = roundInfos[roundIndex].phaseInfos.Count;
        for(int phaseConCount = 0; phaseConCount < phaseMaxCount; phaseConCount++)
        {
            // 시작 전 잠시 대기
            float conTime = 0;
            float maxTime = 3;
            while (conTime < maxTime)
            {
                conTime += Time.deltaTime;
                yield return null;
            }

            // 무작위 조합 생성
            int[] random = MathManager.Instance.Combination(0, roundInfos[roundIndex].phaseInfos[phaseConCount].respawnSpotParent.childCount);

            // 몬스터를 무작위 위치로 생성
            conMonsterCount = 0;
            foreach (PhaseInfo.RespawnInfo temp in roundInfos[roundIndex].phaseInfos[phaseConCount].respawnInfo)
            {
                for(int i = 0; i < temp.respawnCount; i++)
                {
                    GameObject monster = ObjectPullManager.Instance.GetInstanceByName(temp.monsterName);
                    monster.transform.position = roundInfos[roundIndex].phaseInfos[phaseConCount].respawnSpotParent.GetChild(random[conMonsterCount]).position;
                    monster.SetActive(true);
                    conMonsterCount++;
                }
            }
            // 몬스터가 모두 죽을때까지 대기
            while (conMonsterCount > 0)
                yield return null;
        }

        // 다른 지역으로의 이동 제한 해제
        roundInfos[roundIndex].roundDivisionWall.SetActive(false);

        // 모든 라운드를 Clear 했으므로 Clear Panel 생성
        if (roundInfos[roundIndex].nextRoundTrigger == null)
            SystemManager.Instance.ClearPanelOn();
        // 새로운 라운드 진입 가능하도록 Trigger 생성
        else
            roundInfos[roundIndex].nextRoundTrigger.SetActive(true);
    }
}
