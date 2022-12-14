using FC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 충돌체를 생성해 무기를 주울수 있도록 한다.
/// 루팅했으면 충돌체는 제거.
/// 무기를 다시 버릴수도 있어야 하며, 다시 충돌체를 붙여줍니다.(땅 밑으로 내려가지 않게)
/// 관련해서 UI도 컨트롤 할 수 있어야 하고
/// ShootBehaviour에 획득한 무기를 넣어주게 된다.
/// </summary>
public class InteractiveWeapon : MonoBehaviour
{
    public string label_weaponName; //무기이름
    public SoundList shotSound, reloadSound, pickSound, dropSound, noBulletSoud;
    public Sprite weaponSprite;
    public Vector3 rightHandPosition; //플레이어 오른손에 보정위치
    public Vector3 relativeRotation; // 플레이어에 맞춘 보정을 위한 회전값
    public float bulletDamage = 10f;
    public float recoilAngle; //반동
    public enum WeaponType
    {
        NONE,
        SHORT,
        LONG,
    }
    public enum WeaponMode 
    {
        SEMI,
        BURST,
        AUTO,
    }
    public WeaponType weaponType = WeaponType.NONE;
    public WeaponMode weaponMode = WeaponMode.SEMI;
    public int burstSize = 1;

    public int currentMagCapacity, totalBullets; //현재 탄창용량, 소지하고있는 전체 총알 양
    public int fullMag, maxBullets; //재장전시 꽉 채우는 탄의 양과 한번에 채울수 있는 최대 총알 양
    private GameObject player, gameController;
    private ShootBehaviour playerInventory;
    private BoxCollider weaponCollider;
    private SphereCollider interactiveRadius;
    private Rigidbody weaponRigidbody;
    private bool pickable;

    //UI
    public GameObject screeHUD;
    public WeaponUIManager weaponHUD;
    private Transform pickHUD;
    public Text pickupHUD_Label; 
    public Transform muzzleTransform;

    private void Awake()
    {
        gameObject.name = this.label_weaponName;
        gameObject.layer = LayerMask.NameToLayer(TagAndLayer.LayerName.IgnoreRayCast);
        foreach(Transform tr in transform)
        {
            tr.gameObject.layer = LayerMask.NameToLayer(TagAndLayer.LayerName.IgnoreRayCast);
        }

        player = GameObject.FindGameObjectWithTag(TagAndLayer.TagName.Player);
        playerInventory = player.GetComponent<ShootBehaviour>();
        gameController = GameObject.FindGameObjectWithTag(TagAndLayer.TagName.GameController);

        if(weaponHUD == null)
        {
            if(screeHUD == null)
            {
                screeHUD = GameObject.Find("ScreenHUD");
            }
            weaponHUD = screeHUD.GetComponent<WeaponUIManager>();
        }
        if(pickHUD == null)
        {
            pickHUD = gameController.transform.Find("PickupHUD");
        }
        // 인터랙션을 위한 충돌체 설정.
        weaponCollider = transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        CreateInteractiveRadius(weaponCollider.center);
        weaponRigidbody = gameObject.AddComponent<Rigidbody>();

        if (this.weaponType == WeaponType.NONE)
        {
            weaponType = WeaponType.SHORT;
        }
        fullMag = currentMagCapacity;
        maxBullets = totalBullets;
        pickHUD.gameObject.SetActive(false);

        if(muzzleTransform == null)
        {
            muzzleTransform = transform.Find("muzzle");
        }
    }

    private void CreateInteractiveRadius(Vector3 center)
    {
        interactiveRadius = gameObject.AddComponent<SphereCollider>();
        interactiveRadius.center = center;
        interactiveRadius.radius = 1;
        interactiveRadius.isTrigger = true;
    }

    private void TogglePickHUD(bool toggle)
    {
        pickHUD.gameObject.SetActive(toggle);
        if (toggle)
        {
            pickHUD.position = this.transform.position + Vector3.up * 0.5f;
            Vector3 direction = player.GetComponent<BehaviourController>().playerCamera.forward;
            direction.y = 0;
            pickHUD.rotation = Quaternion.LookRotation(direction);
            pickupHUD_Label.text = "Pick" + gameObject.name;
        }
    }

    private void UpdateHUD()
    {
        weaponHUD.UpdateWeaponHUD(weaponSprite, currentMagCapacity, fullMag,totalBullets);
    }
    public void Toggle(bool active)
    {
        if (active)
        {
            SoundManager.Instance.PlayOneShotEffect((int)pickSound,transform.position, 0.5f);
        }
        weaponHUD.Toggle(active);
        UpdateHUD();
    }

    private void Update()
    {
        if(this.pickable && Input.GetButtonDown(ButtonName.Pick))
        {
            //무기를 주웠을 경우 물리적 기능 끄기
            weaponRigidbody.isKinematic = true;
            weaponCollider.enabled = false;
            playerInventory.AddWeapon(this);
            Destroy(interactiveRadius);
            Toggle(true);
            pickable = false;
            TogglePickHUD(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //총이 땅에 떨어질때
        if(collision.collider.gameObject != player && Vector3.Distance(transform.position,player.transform.position) <= 5f)
        {
            SoundManager.Instance.PlayOneShotEffect((int)dropSound, transform.position, 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            pickable = false;
            TogglePickHUD(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player && playerInventory && playerInventory.isActiveAndEnabled)
        {
            pickable = true;
            TogglePickHUD(true);
        }
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        transform.position += Vector3.up;
        weaponRigidbody.isKinematic = false;
        transform.parent = null;
        CreateInteractiveRadius(weaponCollider.center);
        weaponCollider.enabled = true;
        weaponHUD.Toggle(false);

    }

    public bool StartReload()
    {
        //탄창이 꽉찼거나 소지하고 있는 총알이 0이면
        if(currentMagCapacity == fullMag || totalBullets == 0)
        {
            return false;
        }
        else if (totalBullets < fullMag - currentMagCapacity)
        {
            currentMagCapacity += totalBullets;
            totalBullets = 0;
        }
        else
        {
            totalBullets -= fullMag - currentMagCapacity;
            currentMagCapacity = fullMag;
        }
        return true;
    }

    public void EndReload()
    {
        UpdateHUD();
    }

    public bool Shoot(bool firstShot = true)
    {
        if(currentMagCapacity > 0)
        {
            currentMagCapacity--;
            UpdateHUD();
            return true;
        }

        if(firstShot && noBulletSoud != SoundList.None)
        {
            SoundManager.Instance.PlayOneShotEffect((int)noBulletSoud, muzzleTransform.position,5f);
        }

        return false;
    }

    public void ResetBullet()
    {
        currentMagCapacity = fullMag;
        totalBullets = maxBullets;
    }
}
