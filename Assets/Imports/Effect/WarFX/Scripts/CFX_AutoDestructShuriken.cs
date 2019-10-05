using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
    public GameObject deactivatedTarget;

	
	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}
	
	IEnumerator CheckIfAlive ()
	{
        float tempTime;
		while(true)
		{
            tempTime = 0;
            while(tempTime < 1)
            {
                tempTime += Time.deltaTime;
                yield return null;
            }

			if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
                deactivatedTarget.SetActive(false);
				break;
			}
		}
	}
}
