using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLivingEntity : MonoBehaviour
{
    public float initialHealth = 100;                   // 초기 체력
    public float currentHealth { get; protected set; }  // 현재 체력
    public bool death { get; protected set; }

    protected virtual void OnEnable()
    {
        // 활성화 됬을 시 상태 초기화
        death = false;
        currentHealth = initialHealth;
    }

    public virtual void OnDamage(float _damage, Vector3 _hitPoint, Vector3 _hitNormal)
    {
        currentHealth -= _damage;

        // 체력이 0보다 낮거나 죽지 않은 경우 사망 처리
        if (currentHealth <= 0 && !death)
        {
            currentHealth = 0;
            Die();
        }
    }

    public virtual void Die()
    {
        death = true;
    }

    public virtual void RestoreHealth(float _plusHealth)
    {
        // 죽은 경우 실행 x
        if (death)
            return;

        // 체력 회복
        currentHealth += _plusHealth;
        if (currentHealth >= initialHealth)
            currentHealth = initialHealth;
    }

}
