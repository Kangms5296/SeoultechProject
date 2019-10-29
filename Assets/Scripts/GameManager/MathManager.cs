using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathManager : MonoBehaviour
{
    private static MathManager _instance = null;

    public static MathManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(MathManager)) as MathManager;

                if (_instance == null)
                    Debug.LogError("There's no active MathManager object");
            }

            return _instance;
        }
    }
    int count = 0;
    public int[] Combination(int first, int last)
    {
        int length = last - first;
        if (length <= 0)
            return null;

        int[] result = new int[length];
        for (int i = 0; i < length; i++)
            result[i] = first + i;

        int random;
        int temp;
        while (0 < length)
        {
            random = Random.Range(0, length);

            temp = result[random];
            result[random] = result[length - 1];
            result[length - 1] = temp;

            length--;
        }

        length = last - first;

        return result;
    }
}
