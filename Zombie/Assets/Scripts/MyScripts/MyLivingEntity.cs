using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyLivingEntity : MonoBehaviour
{
    public float initialHealth = 100;                   // �ʱ� ü��
    public float currentHealth { get; protected set; }  // ���� ü��
    public bool death { get; protected set; }

    protected virtual void OnEnable()
    {
        // Ȱ��ȭ ���� �� ���� �ʱ�ȭ
        death = false;
        currentHealth = initialHealth;
    }

    public virtual void OnDamage(float _damage, Vector3 _hitPoint, Vector3 _hitNormal)
    {
        currentHealth -= _damage;

        // ü���� 0���� ���ų� ���� ���� ��� ��� ó��
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
        // ���� ��� ���� x
        if (death)
            return;

        // ü�� ȸ��
        currentHealth += _plusHealth;
        if (currentHealth >= initialHealth)
            currentHealth = initialHealth;
    }

}
