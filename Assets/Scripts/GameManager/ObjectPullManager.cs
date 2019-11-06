using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectPullManager : MonoBehaviour
{

    private static ObjectPullManager _instance = null;

    public static ObjectPullManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(ObjectPullManager)) as ObjectPullManager;

                if (_instance == null)
                    Debug.LogError("There's no active ObjectPullManager object");
            }

            return _instance;
        }
    }


    [System.Serializable]
    public struct PullInfo
    {
        public GameObject instance;
        public int size;

        public Transform parentObject;
    }
    public List<PullInfo> pullInfos;

    private Dictionary<string, List<GameObject>> pullList;

    // Start is called before the first frame update
    void Awake()
    {
        pullList = new Dictionary<string, List<GameObject>>();
        
        for (int i = 0; i < pullInfos.Count; i++)
        {
            List<GameObject> temp = new List<GameObject>();
            for (int j = 0; j < pullInfos[i].size; j++)
                temp.Add(Instantiate(pullInfos[i].instance, pullInfos[i].parentObject));

            pullList.Add(pullInfos[i].instance.name, temp);
        }
    }

    public GameObject GetInstanceByName(string typeName)
    {
        for (int i = 0; i < pullList[typeName].Count; i++)
            if (pullList[typeName][i].activeSelf == false)
                return pullList[typeName][i];

        return null;
    }
}
