using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SizeUpCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SizeUpCoroutine()
    {
        float conTime = 0;
        float maxTime = 1;

        while(conTime < maxTime)
        {
            transform.localScale = new Vector3(1 - conTime, 1 - conTime, 1 - conTime);

            conTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(0, 0, 0);
    }
}
