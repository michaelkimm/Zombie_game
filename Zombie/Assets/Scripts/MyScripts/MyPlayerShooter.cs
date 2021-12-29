using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <���>
// 1. �Է¿� ���� �� Ȱ��ȭ�Ͽ� �߻� ���
// 2. ���� �ִϸ��̼� ���

public class MyPlayerShooter : MonoBehaviour
{
    public Transform gunPivot;
    public Gun gun;                 // �� ��ü
    public Transform rightHandle;   // ���� ���������� ��ġ
    public Transform leftHandle;    // ���� �޼����� ��ġ

    private Animator playerAnimation;

    void Start()
    {
        playerAnimation = GetComponent<Animator>();
    }

    void Update()
    {
        // ���� �Ҵ�ǰ� fireŰ �������� ��� 
        if (gun != null && InputManager.Instance.fire)
        {
            gun.Fire();
        }
        else if (gun != null && InputManager.Instance.reload)
        {
            if (gun.Reload())
            {
                playerAnimation.SetTrigger("Reload");
            }
        }

    }

    private void OnAnimatorIK(int layerIndex)
    {
        gunPivot.position = playerAnimation.GetIKHintPosition(AvatarIKHint.RightElbow);

        // ���� �� IK
        playerAnimation.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimation.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimation.SetIKPosition(AvatarIKGoal.LeftHand, leftHandle.position);
        playerAnimation.SetIKRotation(AvatarIKGoal.LeftHand, leftHandle.rotation);

        // ������ �� IK
        playerAnimation.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimation.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimation.SetIKPosition(AvatarIKGoal.RightHand, rightHandle.position);
        playerAnimation.SetIKRotation(AvatarIKGoal.RightHand, rightHandle.rotation);
    }

    private void OnEnable()
    {
        gun.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        gun.gameObject.SetActive(false);
    }

}
