using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// �÷��̾� hp�� ���õ� �۾� ����
// ���, ������
public class MyPlayerHealth : MyLivingEntity
{
    private AudioSource myAudioSource;   // ����� �ҽ�
    public AudioClip dieAudioClip;      // ���� �����
    public AudioClip heartAudioClip;    // ������ �Ծ��� �� �����
    public AudioClip itemGetAudioClip;  // ������ ���� ����

    private Animator myAnimator;        // �ִϸ�����
    private PlayerMovement myPlayerMovement; // �÷��̾� ���� ������ ��Ʈ�ѷ�
    private MyPlayerShooter myPlayerShooter;   // �÷��̾� ���� ��� ��Ʈ�ѷ�
    public GameObject sliderObject;
    private Slider slider;

    private void Awake()
    {
        // ����� ������Ʈ ��������
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

        // ������ & ���� ��Ʈ�ѷ� turn on
        myPlayerMovement.enabled = true;
        myPlayerShooter.enabled = true;

        // �����̴� Ȱ��ȭ & �ʱ�ȭ
        sliderObject.SetActive(true);
        sliderObject.GetComponent<Slider>().maxValue = initialHealth;
        sliderObject.GetComponent<Slider>().value = currentHealth;
    }

    public override void OnDamage(float _damage, Vector3 _hitPoint, Vector3 _hitNormal)
    {
        if (!death)
        {
            // ������ ���� ����
            myAudioSource.PlayOneShot(heartAudioClip);

            base.OnDamage(_damage, _hitPoint, _hitNormal);

            // hp UI ������Ʈ
            sliderObject.GetComponent<Slider>().value = currentHealth;
        }
    }

    public override void Die()
    {
        base.Die();

        // ���� ���� ����
        myAudioSource.PlayOneShot(dieAudioClip);

        // ���� �ִϸ��̼� ���
        myAnimator.SetTrigger("Die");

        // ������ & ���� ��Ʈ�ѷ� turn off
        myPlayerMovement.enabled = false;
        myPlayerShooter.enabled = false;

        // �����̴� ��Ȱ��ȭ
        sliderObject.SetActive(false);
    }

    public override void RestoreHealth(float _plusHealth)
    {
        base.RestoreHealth(_plusHealth);

        // hp UI ������Ʈ
        sliderObject.GetComponent<Slider>().value = currentHealth;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!death)
        {
            MyIItem item = other.GetComponent<MyIItem>();

            if (item != null)
            {
                // ������ ���� ���� ���
                myAudioSource.PlayOneShot(itemGetAudioClip);

                // ������ �������� ����Ѵ�.
                item.Use(this.gameObject);
            }
        }
    }

}
