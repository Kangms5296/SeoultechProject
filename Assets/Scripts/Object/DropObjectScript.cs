using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObjectScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Vector3 position = transform.position;
            WorldSpaceCanvasUIs.SetPosition("Click F", new Vector3(position.x, position.y + 0.8f, position.z));

            WorldSpaceCanvasUIs.SetActive("Click F", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            WorldSpaceCanvasUIs.SetActive("Click F", false);
        }
    }
}
