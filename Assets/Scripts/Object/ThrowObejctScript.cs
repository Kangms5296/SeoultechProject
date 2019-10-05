using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObejctScript : MonoBehaviour
{
    public enum ExplotionType { None, Instant, Time};                     // 폭발 안함, 즉시 폭발, 시간 폭발
    public ExplotionType type;

    public float explosionTime;                                     // 폭발까지의 시간
    public string particleName;                                     // 폭발할때 생성되는 파티클

    private Collider coll;
    private Rigidbody rigid;

    private Vector3 moveVector;
    private float magnitude;


    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
    }


    public void Init(Vector3 moveVector, float magnitude)
    {
        this.moveVector = moveVector;
        this.magnitude = magnitude;

        coll.enabled = true;
        rigid.useGravity = true;
        rigid.AddForce(moveVector * magnitude + Vector3.up * 250);

        // 일정시간 이후 폭발
        if(type == ExplotionType.Time)
            StartCoroutine(TimeCheckCoroutine());
    }

    public void Boom()
    {
        GameObject temp = ObjectPullManager.GetInstanceByName(particleName);

        temp.transform.position = transform.position;
        temp.SetActive(true);

        // 오브젝트 삭제
        transform.parent.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {

        // 땅, 건물에 부딪히면
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
        {
            // 즉시 폭발하는 오브젝트는
            if (type == ExplotionType.Instant)
            {
                // 폭발
                Boom();
            }
        }
    }

    private IEnumerator TimeCheckCoroutine()
    {
        float conTime = 0;

        while(true)
        {
            if (conTime > explosionTime)
                break;

            conTime += Time.deltaTime;
            yield return null;
        }

        // 폭발
        Boom();
    }



}
