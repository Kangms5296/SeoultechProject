﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectPullManager : MonoBehaviour
{
    [System.Serializable]
    public struct PullInfo
    {
        public GameObject instance;
        public int size;
    }
    public List<PullInfo> pullInfos;

    private static Dictionary<string, List<GameObject>> pullList;

    // Start is called before the first frame update
    void Start()
    {
        pullList = new Dictionary<string, List<GameObject>>();
        
        for (int i = 0; i < pullInfos.Count; i++)
        {
            List<GameObject> temp = new List<GameObject>();
            for (int j = 0; j < pullInfos[i].size; j++)
                temp.Add(Instantiate(pullInfos[i].instance, this.transform));

            pullList.Add(pullInfos[i].instance.name, temp);
            Debug.Log(pullInfos[i].instance.name);
        }
    }

    public static GameObject GetInstanceByName(string typeName)
    {
        for (int i = 0; i < pullList[typeName].Count; i++)
            if (pullList[typeName][i].activeSelf == false)
                return pullList[typeName][i];

        return null;
    }
}