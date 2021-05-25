using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    #region Variables
    //Components
    FingerGunMan player;
    Animator anim;

    //Public
    [Header("Firepoints")]
    [SerializeField] Transform firePoint;
    [Space()]
    [Header("Bullet Prefabs")]
    [SerializeField] GameObject bullet;
    [Space()]
    [Header("Firerate")]
    [SerializeField] float fireRate = 0.5f;
    [Space()]
    [Header("Camera")]
    [SerializeField] Transform cameraTarget;
    [SerializeField] float lookAheadAmount, lookAheadSpeed;
    [Space()]
    [Header("SFX")]
    private FMOD.Studio.EventInstance instance;
    [FMODUnity.EventRef]
    public string shootRegularSound;

    //Private Variables
    private PlayerControls playerControls;
    private Transform playerHand;
    private Vector3 mousePosition;
    private float currentFireRate;
    private float currentFireTime;
    #endregion

    #region Monobehaviour Callbacks 
    private void Awake()
    {
        playerControls = new PlayerControls();
        playerHand = gameObject.transform;
        player = FindObjectOfType<FingerGunMan>();
        anim = GetComponentInParent<Animator>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        GetInput();
        Aim();
    }
    #endregion

    #region Private Methods
    private void GetInput()
    {
        //Get Mouse World Position
        mousePosition = Camera.main.ScreenToWorldPoint(playerControls.Gameplay.Aim.ReadValue<Vector2>());

        //Shooting
        if (currentFireTime <= 0)
        {
            if (playerControls.Gameplay.Shoot.triggered && player.ShootingEnabled)
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
        float angle;
        if (player.FacingRight)
            angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        else
            angle = Mathf.Atan2(-aimDirection.y, -aimDirection.x) * Mathf.Rad2Deg;

        playerHand.eulerAngles = new Vector3(0, 0, angle);

        //Move Camera Target
        cameraTarget.localPosition = new Vector3(Mathf.Lerp(cameraTarget.localPosition.x,
            aimDirection.x * lookAheadAmount, lookAheadSpeed * Time.deltaTime),
            cameraTarget.localPosition.y, cameraTarget.localPosition.z);

        //Player Flip Conditions
        if (player.FacingRight)
        {
            if (angle < -90 || angle > 90)
                player.FlipPlayer = true;
            else
                player.FlipPlayer = false;
        }
        else
        {
            if (angle < -90 || angle > 90)
                player.FlipPlayer = true;
            else
                player.FlipPlayer = false;
        }
    }

    private void Shoot()
    {
        Instantiate(bullet, firePoint.position, firePoint.rotation);
        currentFireRate = fireRate;
        anim.SetTrigger("Shoot");

        //SFX
        instance = FMODUnity.RuntimeManager.CreateInstance(shootRegularSound);
        instance.start();
        instance.release();
    }
    #endregion
}