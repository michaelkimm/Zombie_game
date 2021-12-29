using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MyEnemyAI : MyLivingEntity
{
    private Animator enemyAnimatior;        // 적 애니메이터
    private AudioSource enemyAudioSource;   // 적 오디오 소스
    public AudioClip dieClip;               // 죽음 오디오
    public AudioClip damagedClip;           // 적 damaged 오디오
    private NavMeshAgent enemyNavMeshAgent; // 적 매시 에이전트
    public ParticleSystem bloodSprayEffect;// 피 튀기는 이팩트

    private CapsuleCollider capsuleCollider;    // 적 캡술 콜라이더
    private BoxCollider boxCollider;            // 적 박스 콜라이더

    private Renderer enemyRenderer;

    public MyLivingEntity targetLivingEntity;   // 타겟
    public LayerMask targetMask;                // 타겟 레이어 마스크

    public float attackCoolTime;                // 공격 쿨 타임
    private float lastAttackTime = 0f;          // 마지막으로 공격한 시점 

    public float damage;                        // 공격력


    public bool target
    {
        get
        {
            if (targetLivingEntity != null && targetLivingEntity.death != true)
                return true;
            else
                return false;
        }
    }

    private void Awake()
    {
        // 컴포넌트 가져오기
        enemyAnimatior = GetComponent<Animator>();
        enemyAudioSource = GetComponent<AudioSource>();
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
        enemyRenderer = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        // 죽기 전까지 계속 타겟 추적 & 탐색
        StartCoroutine("UpdatePath");
    }

    private void Update()
    {
        // Move 애니메이션 실행
        enemyAnimatior.SetBool("HasTarget", target);
    }

    IEnumerator UpdatePath()
    {
        while (!death)
        {
            if (enemyNavMeshAgent == null)
                print("no enemyNavMeshAgent!");

            // 타겟이 있는 경우 추적, 없는 경우 경게 상태
            if (target)
            {

                // 매시 에이전트 On
                // 타겟이 있으면, 타겟의 위치로 네비게이션 설정
                enemyNavMeshAgent.isStopped = false;
                enemyNavMeshAgent.SetDestination(targetLivingEntity.transform.position);
            }
            else
            {
                // 타겟이 없으면, 네비AI 멈춰!
                enemyNavMeshAgent.isStopped = true;

                Collider[] collidersInCircle = Physics.OverlapSphere(transform.position, 20f, targetMask);

                // 콜라이더 부딛힌 애들 중에 LivingEntity인 객체 찾기
                for (int i = 0; i < collidersInCircle.Length; i++)
                {
                    MyLivingEntity livingEntity = collidersInCircle[i].GetComponent<MyLivingEntity>();

                    // 찾은게 LivingEntity인 경우
                    if (livingEntity != null && livingEntity.death != death)
                    {
                        // 타겟 설정
                        targetLivingEntity = livingEntity;
                        break;
                    }
                }
            }

            // 0.25초 주기로 패스 업데이트
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void Setup(float _newHealth, float _newDamage, float _newSpeed, Color _skinColor)
    {
        // 체력 초기화
        initialHealth = _newHealth;
        currentHealth = initialHealth;

        // 데미지 초기화
        damage = _newDamage;

        // 속도 초기화
        enemyNavMeshAgent.speed = _newSpeed;

        // NavMeshAgent 활성화
        enemyNavMeshAgent.enabled = true;

        // 콜라이더 On
        capsuleCollider.enabled = true;
        boxCollider.enabled = true;

        // 색깔 설정
        enemyRenderer.material.color = _skinColor;
    }

    public override void OnDamage(float _damage, Vector3 _hitPoint, Vector3 _hitNormal)
    {
        if (!death)
        {
            // Damaged 소리 on
            enemyAudioSource.PlayOneShot(damagedClip);

            // 피튀기는 모션 생성
            bloodSprayEffect.transform.position = _hitPoint;
            bloodSprayEffect.transform.rotation = Quaternion.LookRotation(_hitNormal);
            bloodSprayEffect.Play();

            print("피 튀기는 모션 생성!");

            // 데미지 주기
            base.OnDamage(_damage, _hitPoint, _hitNormal);
        }
    }

    public override void Die()
    {
        // Die 소리 on
        enemyAudioSource.PlayOneShot(dieClip);

        // 죽음 애니메이션 실행
        enemyAnimatior.SetTrigger("Die");

        // 매시 에이전트 Off
        enemyNavMeshAgent.isStopped = true;
        enemyNavMeshAgent.enabled = false;

        // 콜라이더 Off
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        base.Die();
    }

    public override void RestoreHealth(float _plusHealth)
    {
        base.RestoreHealth(_plusHealth);
    }

    private void OnTriggerStay(Collider other)
    {
        // 내가 살아있으며 & 충돌체가 살아있는 생명체 & 죽지 않았고 & 타겟일 경우

        MyLivingEntity targetLivingEntity_ = other.gameObject.GetComponent<MyLivingEntity>();
        if (!death && targetLivingEntity_ != null && targetLivingEntity_ == targetLivingEntity)
        {
            // 1초 간격으로 공격
            if (Time.time >= lastAttackTime + attackCoolTime)
            {
                // 타겟에게 데미지 주기
                targetLivingEntity_.OnDamage(damage,
                                            other.ClosestPoint(transform.position),
                                            Vector3.Normalize(transform.position - other.transform.position));

                // 마지막 공격 시간 업데이트
                lastAttackTime = Time.time;
            }
        }
    }


}
