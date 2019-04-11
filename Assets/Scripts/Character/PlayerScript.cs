using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, CharacterScript
{
    public float moveSpeed = 100f;
    public float rotateSpeed = 0.15f;

    Rigidbody rigid;
    
    float h, v;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
    }
    void FixedUpdate()
    {
        // 캐릭터 이동 및 회전
        Move();
    }


    // ===================================================== public function ============================================================

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Damaged()
    {
        throw new System.NotImplementedException();
    }

    public void Move()
    {
        // 캐릭터 이동
        Vector3 movement = new Vector3(h, 0f, v);
        movement = movement * moveSpeed * Time.deltaTime;
        rigid.velocity = movement;

        // 캐릭터 회전
        Quaternion rotate = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), rotateSpeed);
        rigid.rotation = rotate;
    }

    public void Rotate()
    {
    }

    // ===================================================== private function ============================================================

}
