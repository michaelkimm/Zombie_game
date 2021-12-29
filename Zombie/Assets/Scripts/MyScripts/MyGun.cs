using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGun : MonoBehaviour
{
    public enum GunState { Ready, Reload, empty };
    public GunState state { get; private set; }

    public float damage = 2.0f;                 // �� ������
    public float maxDistance = 5.0f;            // �����Ÿ�
    public int bulletCnt = 0;                   // ���� źâ ���� �Ѿ�
    public int bulletInBox = 100;               // źâ �ڽ� �� ���� �Ѿ�
    public int bulletMax = 30;                  // źâ �� �ִ� �Ѿ� ����

    public float reloadTime = 2;                // ���� ���� �� �ð�
    public float reloadCurTime = 0;             // ���� �� ���� �ð�

    public ParticleSystem MuzzleFlashEffect_;   // ��ƼŬ �ý���
    public ParticleSystem ShellEjectEffect_;    // ��ƼŬ �ý���

    private AudioSource gunAudioSource;
    public AudioClip gunShoot;                  // �Ѿ� �߻� �����
    public AudioClip gunReload;                 // �� ���� �����

    public Transform gunMuzzle;                 // �ѱ� ��ġ
    private LineRenderer lineRenderer;          // �Ѿ� ����
    public float bulletExistTime = 0.3f;        // �Ѿ� ���� ���� �ð�
    private Vector3 endPose;                    // �Ѿ� ���� �� ����

    private RaycastHit hit;                     // �Ѿ� ���� ��ġ ������

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
        // �Ѿ��� ���� ��� �߻�
        if (bulletCnt >= 0)
        {
            // ���� ���ӽð� ����
            StartCoroutine("BulletExistTime", bulletExistTime);
        }
    }

    IEnumerator BulletExistTime(float _t)
    {
        // ��ƼŬ �ý��� �÷���
        if (MuzzleFlashEffect_ != null)
            MuzzleFlashEffect_.Play();
        if (ShellEjectEffect_ != null)
            ShellEjectEffect_.Play();

        // �߻� ���� �÷���
        gunAudioSource.clip = gunShoot;
        gunAudioSource.Play();

        // ���� Ȱ��ȭ
        lineRenderer.enabled = true;

        // ���� ���� ���� ����
        lineRenderer.SetPosition(0, gunMuzzle.position);
        endPose = gunMuzzle.position + gunMuzzle.forward * maxDistance;

        // �����Ÿ��� ��ü ������
        if (Physics.Raycast(gunMuzzle.position, gunMuzzle.forward, out hit, maxDistance))
        {
            lineRenderer.SetPosition(0, gunMuzzle.position);
            endPose = hit.point;
        }

        // �� ���� ����
        lineRenderer.SetPosition(1, endPose);

        // _t�� �� ���� ��Ȱ��ȭ
        yield return new WaitForSeconds(_t);
        lineRenderer.enabled = false;
    }

    public bool Reload()
    {
        return true;
    }
}
