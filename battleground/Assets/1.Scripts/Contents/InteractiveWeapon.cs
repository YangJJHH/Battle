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
    private Rigidbody weaponRigidbody;
    private bool pickable;

    //UI
    public GameObject screeHUD;
    public WeaponUIManager weaponHUD;
    private Transform pickHUD;
    public Text pickupHUD_Label;

    public Transform muzzleTransform;

}
