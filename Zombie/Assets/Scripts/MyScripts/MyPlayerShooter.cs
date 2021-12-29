using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <기능>
// 1. 입력에 따라 총 활성화하여 발사 명령
// 2. 관련 애니메이션 재생

public class MyPlayerShooter : MonoBehaviour
{
    public Transform gunPivot;
    public Gun gun;                 // 총 객체
    public Transform rightHandle;   // 총의 오른손잡이 위치
    public Transform leftHandle;    // 총의 왼손잡이 위치

    private Animator playerAnimation;

    void Start()
    {
        playerAnimation = GetComponent<Animator>();
    }

    void Update()
    {
        // 총이 할당되고 fire키 눌러졌을 경우 
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

        // 왼쪽 팔 IK
        playerAnimation.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimation.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimation.SetIKPosition(AvatarIKGoal.LeftHand, leftHandle.position);
        playerAnimation.SetIKRotation(AvatarIKGoal.LeftHand, leftHandle.rotation);

        // 오른쪽 팔 IK
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
