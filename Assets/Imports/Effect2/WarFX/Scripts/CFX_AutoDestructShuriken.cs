using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	public bool OnlyDeactivate;
	
	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}
	
	IEnumerator CheckIfAlive ()
	{
        float conTime;
		while(true)
		{
            conTime = 0;
            while(conTime < 0.5f)
            {
                conTime += Time.deltaTime;
                yield return null;
            }

			if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
				if(OnlyDeactivate)
                    gameObject.SetActive(false);
				else
					GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}
}
