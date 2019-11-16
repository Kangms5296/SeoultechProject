using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceCanvasUIs : MonoBehaviour
{
    private static Dictionary<string, WorldSpaceCanvasUIs> instances = new Dictionary<string, WorldSpaceCanvasUIs>();

    // Start is called before the first frame update
    void Awake()
    {
        if (instances == null)
        {
            instances.Add(gameObject.name, this);
        }

        gameObject.SetActive(false);
    }

    public static void SetPosition(string name, Vector3 newPos)
    {
        instances[name].transform.position = newPos;
    }

    public static void SetActive(string name, bool value)
    {
        instances[name].gameObject.SetActive(value);
    }
}
