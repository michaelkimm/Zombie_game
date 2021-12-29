using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 플레이어 hp와 관련된 작업 수행
// 사망, 데미지
public class MyPlayerHealth : MyLivingEntity
{
    private AudioSource myAudioSource;   // 오디오 소스
    public AudioClip dieAudioClip;      // 죽음 오디오
    public AudioClip heartAudioClip;    // 데미지 입었을 때 오디오
    public AudioClip itemGetAudioClip;  // 아이템 습득 사운드

    private Animator myAnimator;        // 애니메이터
    private PlayerMovement myPlayerMovement; // 플레이어 방향 움직임 컨트롤러
    private MyPlayerShooter myPlayerShooter;   // 플레이어 슈팅 모션 컨트롤러
    public GameObject sliderObject;
    private Slider slider;

    private void Awake()
    {
        // 사용할 컴포넌트 가져오기
        myAudioSource = GetComponent<AudioSource>();
        myAnimator = GetComponent<Animator>();
        myPlayerMovement = GetComponent<PlayerMovement>();
        myPlayerShooter = GetComponent<MyPlayerShooter>();
    }

    void Start()
    {
        
    }

    

    protected override void OnEnable()
    {
        base.OnEnable();

        // 움직임 & 슈팅 컨트롤러 turn on
        myPlayerMovement.enabled = true;
        myPlayerShooter.enabled = true;

        // 슬라이더 활성화 & 초기화
        sliderObject.SetActive(true);
        sliderObject.GetComponent<Slider>().maxValue = initialHealth;
        sliderObject.GetComponent<Slider>().value = currentHealth;
    }

    public override void OnDamage(float _damage, Vector3 _hitPoint, Vector3 _hitNormal)
    {
        if (!death)
        {
            // 데미지 사운드 실행
            myAudioSource.PlayOneShot(heartAudioClip);

            base.OnDamage(_damage, _hitPoint, _hitNormal);

            // hp UI 업데이트
            sliderObject.GetComponent<Slider>().value = currentHealth;
        }
    }

    public override void Die()
    {
        base.Die();

        // 죽음 사운드 실행
        myAudioSource.PlayOneShot(dieAudioClip);

        // 죽음 애니메이션 재생
        myAnimator.SetTrigger("Die");

        // 움직임 & 슈팅 컨트롤러 turn off
        myPlayerMovement.enabled = false;
        myPlayerShooter.enabled = false;

        // 슬라이더 비활성화
        sliderObject.SetActive(false);
    }

    public override void RestoreHealth(float _plusHealth)
    {
        base.RestoreHealth(_plusHealth);

        // hp UI 업데이트
        sliderObject.GetComponent<Slider>().value = currentHealth;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!death)
        {
            MyIItem item = other.GetComponent<MyIItem>();

            if (item != null)
            {
                // 아이템 습득 사운드 재생
                myAudioSource.PlayOneShot(itemGetAudioClip);

                // 나에게 아이템을 사용한다.
                item.Use(this.gameObject);
            }
        }
    }

}
