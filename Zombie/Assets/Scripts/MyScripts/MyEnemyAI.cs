using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MyEnemyAI : MyLivingEntity
{
    private Animator enemyAnimatior;        // �� �ִϸ�����
    private AudioSource enemyAudioSource;   // �� ����� �ҽ�
    public AudioClip dieClip;               // ���� �����
    public AudioClip damagedClip;           // �� damaged �����
    private NavMeshAgent enemyNavMeshAgent; // �� �Ž� ������Ʈ
    public ParticleSystem bloodSprayEffect;// �� Ƣ��� ����Ʈ

    private CapsuleCollider capsuleCollider;    // �� ĸ�� �ݶ��̴�
    private BoxCollider boxCollider;            // �� �ڽ� �ݶ��̴�

    private Renderer enemyRenderer;

    public MyLivingEntity targetLivingEntity;   // Ÿ��
    public LayerMask targetMask;                // Ÿ�� ���̾� ����ũ

    public float attackCoolTime;                // ���� �� Ÿ��
    private float lastAttackTime = 0f;          // ���������� ������ ���� 

    public float damage;                        // ���ݷ�


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
        // ������Ʈ ��������
        enemyAnimatior = GetComponent<Animator>();
        enemyAudioSource = GetComponent<AudioSource>();
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
        enemyRenderer = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        // �ױ� ������ ��� Ÿ�� ���� & Ž��
        StartCoroutine("UpdatePath");
    }

    private void Update()
    {
        // Move �ִϸ��̼� ����
        enemyAnimatior.SetBool("HasTarget", target);
    }

    IEnumerator UpdatePath()
    {
        while (!death)
        {
            if (enemyNavMeshAgent == null)
                print("no enemyNavMeshAgent!");

            // Ÿ���� �ִ� ��� ����, ���� ��� ��� ����
            if (target)
            {

                // �Ž� ������Ʈ On
                // Ÿ���� ������, Ÿ���� ��ġ�� �׺���̼� ����
                enemyNavMeshAgent.isStopped = false;
                enemyNavMeshAgent.SetDestination(targetLivingEntity.transform.position);
            }
            else
            {
                // Ÿ���� ������, �׺�AI ����!
                enemyNavMeshAgent.isStopped = true;

                Collider[] collidersInCircle = Physics.OverlapSphere(transform.position, 20f, targetMask);

                // �ݶ��̴� �ε��� �ֵ� �߿� LivingEntity�� ��ü ã��
                for (int i = 0; i < collidersInCircle.Length; i++)
                {
                    MyLivingEntity livingEntity = collidersInCircle[i].GetComponent<MyLivingEntity>();

                    // ã���� LivingEntity�� ���
                    if (livingEntity != null && livingEntity.death != death)
                    {
                        // Ÿ�� ����
                        targetLivingEntity = livingEntity;
                        break;
                    }
                }
            }

            // 0.25�� �ֱ�� �н� ������Ʈ
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void Setup(float _newHealth, float _newDamage, float _newSpeed, Color _skinColor)
    {
        // ü�� �ʱ�ȭ
        initialHealth = _newHealth;
        currentHealth = initialHealth;

        // ������ �ʱ�ȭ
        damage = _newDamage;

        // �ӵ� �ʱ�ȭ
        enemyNavMeshAgent.speed = _newSpeed;

        // NavMeshAgent Ȱ��ȭ
        enemyNavMeshAgent.enabled = true;

        // �ݶ��̴� On
        capsuleCollider.enabled = true;
        boxCollider.enabled = true;

        // ���� ����
        enemyRenderer.material.color = _skinColor;
    }

    public override void OnDamage(float _damage, Vector3 _hitPoint, Vector3 _hitNormal)
    {
        if (!death)
        {
            // Damaged �Ҹ� on
            enemyAudioSource.PlayOneShot(damagedClip);

            // ��Ƣ��� ��� ����
            bloodSprayEffect.transform.position = _hitPoint;
            bloodSprayEffect.transform.rotation = Quaternion.LookRotation(_hitNormal);
            bloodSprayEffect.Play();

            print("�� Ƣ��� ��� ����!");

            // ������ �ֱ�
            base.OnDamage(_damage, _hitPoint, _hitNormal);
        }
    }

    public override void Die()
    {
        // Die �Ҹ� on
        enemyAudioSource.PlayOneShot(dieClip);

        // ���� �ִϸ��̼� ����
        enemyAnimatior.SetTrigger("Die");

        // �Ž� ������Ʈ Off
        enemyNavMeshAgent.isStopped = true;
        enemyNavMeshAgent.enabled = false;

        // �ݶ��̴� Off
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
        // ���� ��������� & �浹ü�� ����ִ� ����ü & ���� �ʾҰ� & Ÿ���� ���

        MyLivingEntity targetLivingEntity_ = other.gameObject.GetComponent<MyLivingEntity>();
        if (!death && targetLivingEntity_ != null && targetLivingEntity_ == targetLivingEntity)
        {
            // 1�� �������� ����
            if (Time.time >= lastAttackTime + attackCoolTime)
            {
                // Ÿ�ٿ��� ������ �ֱ�
                targetLivingEntity_.OnDamage(damage,
                                            other.ClosestPoint(transform.position),
                                            Vector3.Normalize(transform.position - other.transform.position));

                // ������ ���� �ð� ������Ʈ
                lastAttackTime = Time.time;
            }
        }
    }


}
