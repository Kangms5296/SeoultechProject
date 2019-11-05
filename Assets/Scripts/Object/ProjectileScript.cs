using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [Header("Projectile Info")]
    public float speed;                 // 투사체가 날아가는 속도
    
    public Collider projectileCollider;     // 충돌 판정 콜라이더
    
    private bool isPenetrating;         // 관통 여부
    private Vector3 moveVector;         // 이동 방향
    private float distance;             // 이동 거리
    private int damage;                 // 데미지

    public bool forPlayer;

    private Coroutine movingCoroutine;

    // 총알이 날아갈 방향과 데미지 등을 입력
    public void Init(Vector3 startPos, Vector3 moveVector, bool isPenetrating, int damage, float distance)
    {
        transform.position = startPos;
        this.moveVector = moveVector;
        this.isPenetrating = isPenetrating;
        this.damage = damage;
        this.distance = distance;
        
        gameObject.SetActive(true);

        movingCoroutine = StartCoroutine(Moving());
    }


    IEnumerator Moving()
    {
        // 충돌판정 on
        projectileCollider.enabled = true;

        float conMovedDistance = 0;
        while (conMovedDistance < distance)
        {
            // 투사체 이동
            transform.position = transform.position + moveVector.normalized * Time.deltaTime * speed;

            // 이동한 거리를 체크
            conMovedDistance += Time.deltaTime * speed;

            yield return null;
        }

        StartCoroutine(Removing());
    }

    IEnumerator Removing()
    { 
        // 충돌판정 off
        projectileCollider.enabled = false;

        // 모든 파티클을 삭제
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        if (particles != null)
            foreach (ParticleSystem particle in particles)
                particle.Stop();

        // 1초간 대기
        float conTime = 0;
        while (conTime < 0.5f)
        {
            conTime += Time.deltaTime;
            yield return null;
        }

        // 필드에서 삭제
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (forPlayer)
        {
            if (other.CompareTag("Monster"))
            {
                // 몬스터 데미지 처리
                MonsterScript monster = other.GetComponent<MonsterScript>();
                monster.TakeDamage(damage, moveVector, 0.1f);

                // 이동 정지
                if (movingCoroutine != null)
                    StopCoroutine(movingCoroutine);

                // 관통 여부에 따라 총알 삭제
                if (!isPenetrating)
                    StartCoroutine(Removing());
            }
            else if (other.CompareTag("Beatable Object"))
            {
                BeatableObjectScript temp = other.GetComponent<BeatableObjectScript>();
                temp.Hit();

                // 이동 정지
                if (movingCoroutine != null)
                    StopCoroutine(movingCoroutine);

                // 관통 여부에 따라 총알 삭제
                if (!isPenetrating)
                    StartCoroutine(Removing());
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                // 몬스터 데미지 처리
                PlayerScript player = other.GetComponent<PlayerScript>();
                player.TakeDamage(damage, moveVector, 1);

                // 이동 정지
                if (movingCoroutine != null)
                    StopCoroutine(movingCoroutine);

                // 관통 여부에 따라 총알 삭제
                if (!isPenetrating)
                    StartCoroutine(Removing());
            }
        }
    }
}
