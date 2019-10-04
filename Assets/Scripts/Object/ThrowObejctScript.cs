using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObejctScript : MonoBehaviour
{
    public bool instantaneousExplosion;                             // 즉시 폭발 여부
    public float explosionTime;                                     // 폭발까지의 시간

    private Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        // 즉시 폭발하는 투척물에 대해서는
        if (instantaneousExplosion)
        {
            // 적캐릭터, 땅, 건물에 부딪히면
            if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9 || collision.gameObject.layer == 12)
            {
                // 폭발
                gameObject.SetActive(false);
            }
        }
    }
}
