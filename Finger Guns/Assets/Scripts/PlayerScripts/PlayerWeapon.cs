using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    #region Variables
    public Transform firePoint;
    public Transform sprayPoint1;
    public Transform sprayPoint2;

    public GameObject bullet;
    public GameObject fastBullet;
    public GameObject homingBullet;
    public GameObject sprayBullet;
    public GameObject blastBullet;

    public float regularBulletRate = 0.5f;
    public float fastBulletRate = 0.5f;
    public float homingBulletRate = 0.5f;
    public float sprayBulletRate = 0.5f;
    public float blastBulletRate = 0.5f;

    //Private Variables
    private Transform playerHand;
    private Vector3 mousePosition;
    [Range(1,5)]
    private int weaponSelect = 1;
    private bool shooting;
    private float currentFireRate;
    private float currentFireTime;
    #endregion

    #region Monobehaviour Callbacks 
    private void Awake()
    {
        playerHand = gameObject.transform;
    }

    private void Update()
    {
        GetInput();
        Aim();
        WeaponSwitch();
    }
    #endregion

    #region Private Methods
    private void GetInput()
    {
        //Get Mouse World Position
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Shooting
        if (currentFireTime <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
                currentFireTime = currentFireRate;
            }
        }
        else
        {
            currentFireTime -= Time.deltaTime;
        }
    }

    private void Aim()
    {
        //Aim Hand
        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        playerHand.eulerAngles = new Vector3(0, 0, angle);
    }

    private void Shoot()
    {        
        switch (weaponSelect)
        {
            case 1:
                Instantiate(bullet, firePoint.position, firePoint.rotation);
                currentFireRate = regularBulletRate;
                break;
            case 2:
                Instantiate(fastBullet, firePoint.position, firePoint.rotation);
                currentFireRate = fastBulletRate;
                break;
            case 3:
                Instantiate(homingBullet, firePoint.position, firePoint.rotation);
                currentFireRate = homingBulletRate;
                break;
            case 4:
                Instantiate(sprayBullet, firePoint.position, firePoint.rotation);
                Instantiate(sprayBullet, sprayPoint1.position, sprayPoint1.rotation);
                Instantiate(sprayBullet, sprayPoint2.position, sprayPoint2.rotation);
                currentFireRate = sprayBulletRate;
                break;
            case 5:
                Instantiate(blastBullet, firePoint.position, firePoint.rotation);
                currentFireRate = blastBulletRate;
                break;
        }
    }
    private void WeaponSwitch()
    {
        if(Input.GetButtonDown("Weapon1"))
        {
            weaponSelect = 1;
        }
        else if (Input.GetButtonDown("Weapon2"))
        {
            weaponSelect = 2;
        }
        else if (Input.GetButtonDown("Weapon3"))
        {
            weaponSelect = 3;
        }
        else if (Input.GetButtonDown("Weapon4"))
        {
            weaponSelect = 4;
        }
        else if (Input.GetButtonDown("Weapon5"))
        {
            weaponSelect = 5;
        }
    }
    #endregion
}
