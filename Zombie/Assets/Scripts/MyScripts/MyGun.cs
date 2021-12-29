using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGun : MonoBehaviour
{
    public enum GunState { Ready, Reload, empty };
    public GunState state { get; private set; }

    public float damage = 2.0f;                 // 총 데미지
    public float maxDistance = 5.0f;            // 사정거리
    public int bulletCnt = 0;                   // 현재 탄창 남은 총알
    public int bulletInBox = 100;               // 탄창 박스 내 남은 총알
    public int bulletMax = 30;                  // 탄창 내 최대 총알 갯수

    public float reloadTime = 2;                // 총의 장전 총 시간
    public float reloadCurTime = 0;             // 장전 중 지난 시간

    public ParticleSystem MuzzleFlashEffect_;   // 파티클 시스템
    public ParticleSystem ShellEjectEffect_;    // 파티클 시스템

    private AudioSource gunAudioSource;
    public AudioClip gunShoot;                  // 총알 발사 오디오
    public AudioClip gunReload;                 // 총 장전 오디오

    public Transform gunMuzzle;                 // 총구 위치
    private LineRenderer lineRenderer;          // 총알 궤적
    public float bulletExistTime = 0.3f;        // 총알 궤적 지속 시간
    private Vector3 endPose;                    // 총알 궤적 끝 지점

    private RaycastHit hit;                     // 총알 맞은 위치 데이터

    void Start()
    {
        gunAudioSource = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        state = GunState.Ready;
    }

    void Update()
    {
        
    }

    public void Fire()
    {
        // 총알이 있을 경우 발사
        if (bulletCnt >= 0)
        {
            // 궤적 지속시간 설정
            StartCoroutine("BulletExistTime", bulletExistTime);
        }
    }

    IEnumerator BulletExistTime(float _t)
    {
        // 파티클 시스템 플레이
        if (MuzzleFlashEffect_ != null)
            MuzzleFlashEffect_.Play();
        if (ShellEjectEffect_ != null)
            ShellEjectEffect_.Play();

        // 발사 사운드 플레이
        gunAudioSource.clip = gunShoot;
        gunAudioSource.Play();

        // 궤적 활성화
        lineRenderer.enabled = true;

        // 궤적 시작 지점 설정
        lineRenderer.SetPosition(0, gunMuzzle.position);
        endPose = gunMuzzle.position + gunMuzzle.forward * maxDistance;

        // 사정거리에 물체 맞으면
        if (Physics.Raycast(gunMuzzle.position, gunMuzzle.forward, out hit, maxDistance))
        {
            lineRenderer.SetPosition(0, gunMuzzle.position);
            endPose = hit.point;
        }

        // 끝 지점 설정
        lineRenderer.SetPosition(1, endPose);

        // _t초 후 궤적 비활성화
        yield return new WaitForSeconds(_t);
        lineRenderer.enabled = false;
    }

    public bool Reload()
    {
        return true;
    }
}
