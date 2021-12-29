using System.Collections;
using UnityEngine;

// 총을 구현한다
public class Gun : MonoBehaviour {
    // 총의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State
    {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }

    public State state { get; private set; }    // 현재 총의 상태

    public Transform fireTransform;             // 총알이 발사될 위치

    public ParticleSystem muzzleFlashEffect;    // 총구 화염 효과
    public ParticleSystem shellEjectEffect;     // 탄피 배출 효과

    private LineRenderer bulletLineRenderer;    // 총알 궤적을 그리기 위한 렌더러

    private AudioSource gunAudioPlayer;         // 총 소리 재생기
    public AudioClip shotClip;                  // 발사 소리
    public AudioClip reloadClip;                // 재장전 소리

    public float damage = 25;                   // 공격력
    private float fireDistance = 50f;           // 사정거리

    public int ammoRemain = 100;                // 남은 전체 탄약
    public int magCapacity = 25;                // 탄창 용량
    public int magAmmo;                         // 현재 탄창에 남아있는 탄약


    public float timeBetFire = 0.12f;           // 총알 발사 간격
    public float reloadTime = 1.8f;             // 재장전 소요 시간
    private float lastFireTime;                 // 총을 마지막으로 발사한 시점

    private RaycastHit hit;                     // 총알 맞은 위치 데이터

    private void Awake()
    {
        // 사용할 컴포넌트들의 참조를 가져오기
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        // 궤적에 사용할 점은 2개다
        bulletLineRenderer.positionCount = 2;

        // 궤적 비활성화
        bulletLineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        // 총 상태 초기화
        state = State.Ready;
        magAmmo = magCapacity;
    }

    // 발사 시도
    public void Fire()
    {
        // 총알이 있을 경우 발사
        if (magAmmo > 0 && state == State.Ready && Time.time >= lastFireTime + timeBetFire)
        {
            // 마지막 총알 발사 시점 초기화
            lastFireTime = Time.time;

            // 총알 발사
            Shot();

            // 총알 갯수 감소
            magAmmo--;
            if (magAmmo <= 0)
            {
                state = State.Empty;
            }
        }
    }

    // 실제 발사 처리
    private void Shot()
    {
        Vector3 endPose = Vector3.zero;

        // 사정거리에 물체 유무 확인
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance))
        {
            // 맞은 물체가 데미지 받을 수 있으면, 데미지 주기
            MyLivingEntity target = hit.collider.gameObject.GetComponent<MyLivingEntity>();
            if (target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
                print("총알이 IDamageable 맞힘!");
            }

            // 맞은 곳 업데이트
            endPose = hit.point;
        }
        else
            endPose = fireTransform.position + fireTransform.forward * fireDistance;

        // 궤적 & 파티클 & 사운드 설정
        StartCoroutine("ShotEffect", endPose);
    }

    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다
    private IEnumerator ShotEffect(Vector3 _endPose)
    {
        // 파티클 시스템 플레이
        if (muzzleFlashEffect != null)
            muzzleFlashEffect.Play();
        if (shellEjectEffect != null)
            shellEjectEffect.Play();

        // 발사 사운드 플레이
        gunAudioPlayer.PlayOneShot(shotClip);

        // 궤적 시작 지점 설정
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        // 끝 지점 설정
        bulletLineRenderer.SetPosition(1, _endPose);
        // 궤적 활성화
        bulletLineRenderer.enabled = true;

        // _t초 후 궤적 비활성화
        yield return new WaitForSeconds(0.03f);
        bulletLineRenderer.enabled = false;
    }

    // 재장전 시도
    public bool Reload()
    {
        // 현재 장전중인 상황이 아니거나, 남은 탄창이 있으면
        if (state != State.Reloading && ammoRemain >= 0 && magAmmo < magCapacity)
        {
            StartCoroutine(ReloadRoutine());
            return true;
        }
        return false;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine()
    {
        // 현재 상태를 재장전 중 상태로 전환
        state = State.Reloading;

        // 재장전 소리
        gunAudioPlayer.PlayOneShot(reloadClip);

        // 탄창 상황에 따라 탄알 장전

        // 추가적으로 필요한 총알
        int needAmmo = magCapacity - magAmmo;

        if (ammoRemain >= needAmmo)
        {
            ammoRemain -= needAmmo;
            magAmmo += needAmmo;
        }
        else
        {
            magAmmo += ammoRemain;
            ammoRemain = 0;
        }

        // 재장전 소요 시간 만큼 처리를 쉬기
        yield return new WaitForSeconds(reloadTime);

        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = State.Ready;
    }
}
