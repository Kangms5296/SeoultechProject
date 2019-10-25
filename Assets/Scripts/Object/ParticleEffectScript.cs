using UnityEngine;
using System.Collections;

public class ParticleEffectScript : MonoBehaviour
{
    public bool OnlyDeactivate;

    public ParticleSystemRenderer test;

    private void Start()
    {
    }

    void OnEnable()
    {

        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()
    {
        float conTime;
        while (true)
        {
            conTime = 0;
            while (conTime < 0.5f)
            {
                conTime += Time.deltaTime;
                yield return null;
            }

            if (!GetComponent<ParticleSystem>().IsAlive(true))
            {
                if (OnlyDeactivate)
                    gameObject.SetActive(false);
                else
                    GameObject.Destroy(this.gameObject);
                break;
            }
        }
    }
}
