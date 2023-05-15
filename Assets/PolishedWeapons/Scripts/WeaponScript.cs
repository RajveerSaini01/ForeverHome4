using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponScript : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 50;
    public float fireRate = 300; // RPM
    public float barrelVelocity = 500f; // m/s
    public int maxAmmo = 10;
    public int ammo = 10;
    public float reloadTime = 0.75f;

    [Header("Weapon Properties")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform holsterTransform;
    [SerializeField] private Holster PlayerHolsterScript;
    [SerializeField] private AudioSource fireClip;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI ammoText;
    
    private Rigidbody wepPhysics;
    private Camera cam;
    
    [Header("Weapon States")]
    [SerializeField] private bool equipped;
    [SerializeField] private bool reloading;
    [SerializeField] private bool chambered = true;
    [SerializeField] private bool animating = false;

    //Header("Animations")]

    void Awake()
    {
        wepPhysics = GetComponent<Rigidbody>();
        canvas = transform.Find("Canvas").gameObject;
        canvas.SetActive(false);

        ammoText.text = "Ammo: " + ammo;
        fireClip = GetComponent<AudioSource>();
        
        
        
        LevelGeneration.OnReady += OnMapReady;

        // This Try-Catch exists because we need it to defer searching for a Player when they get placed by the map
        // But they still need to check when they get instantiated by player (via dropping)
        
        cam = Camera.main;
        if (cam != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            PlayerHolsterScript = player.GetComponent<Holster>();
            holsterTransform = cam.transform.Find("WeaponTransform");
        }
    }

    public void SetValues(float dmg, float fr, float bv, int mAmmo, int cAmmo, float rt, bool eq)
    {
        canvas.SetActive(true);
        damage = dmg;
        fireRate = fr;
        barrelVelocity = bv;
        maxAmmo = mAmmo;
        ammo = cAmmo;
        reloadTime = rt;
        equipped = eq;
        ammoText.text = "Ammo: " + ammo;
    }

    private void OnMapReady()
    {
        LevelGeneration.OnReady -= OnMapReady;

        cam = Camera.main;
        if (cam != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            PlayerHolsterScript = player.GetComponent<Holster>();
            holsterTransform = cam.transform.Find("WeaponTransform");
        }
    }

    private void OnInteract(string nm)
    {
        if (gameObject.name == nm && !equipped)
        {
            
            Debug.Log("Picking up " + nm);
            equipped = true;
            canvas.SetActive(true);
            transform.SetParent(holsterTransform, true);
            // transform.position = transform.parent.position;
            // transform.rotation = transform.parent.rotation;
            wepPhysics.useGravity = false;
            wepPhysics.isKinematic = false; // set to true to get rid of the floaty gun physics
            PlayerHolsterScript.AddWeaponToHolster(gameObject);
        }
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }
        
        Debug.DrawRay(transform.position, -transform.forward, Color.red);
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        
        // World-Screen Positional Lock
        Ray camRay = cam.ViewportPointToRay(new Vector3(0.8f,0.4f, 0f), cam.stereoActiveEye);
        holsterTransform.position = camRay.direction + camRay.origin;
        Debug.DrawRay(camRay.origin, camRay.direction);
        
        // Kinematic Tether
        KinematicTetherUpdate();
        
        
        if (Input.GetKeyDown(KeyCode.G) && equipped)
        {
            equipped = false;
            canvas.SetActive(false);
            transform.SetParent(null);
            wepPhysics.useGravity = true;
            wepPhysics.isKinematic = false;
            wepPhysics.AddForce(2f * cam.transform.forward.normalized, ForceMode.Impulse);
            
            PlayerHolsterScript.RemoveWeaponFromHolster(gameObject);
        }
        
        if (Input.GetButtonDown("Fire1"))
        {
            if (equipped && chambered && ammo > 0 && !reloading)
            {
                Shoot();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (equipped && !reloading)
            {
                StartCoroutine(nameof(Reload));
            }
        }
    }

    private void KinematicTetherUpdate()
    {
        // If the gun is too far away, slowly move it back to holster (also do rotation)
        if (equipped && !animating && transform.parent != null)
        {
            // Tether Position (Approximate)
            if (Vector3.Distance(transform.position, transform.parent.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, Time.deltaTime * 5f);
                wepPhysics.drag += 0.05f;
            }
            else
            {
                transform.position = transform.parent.position;
                wepPhysics.drag = 0f;
            }
            
            // Tether Rotations (Approximate)
            if (Quaternion.Angle(transform.rotation, transform.parent.rotation) > 5f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.parent.rotation, Time.deltaTime * 200f);
                wepPhysics.angularDrag += 0.05f;
            }
            else
            {
                transform.rotation = transform.parent.rotation;
                wepPhysics.angularDrag = 0f;
            }
        }
    }

    private void Shoot()
    {
        fireClip.Play();
        
        Vector3 barrelPos = transform.Find("Barrel").position;
        //Vector3 direction = cam.transform.forward; //cam maybe null?? idk
        Vector3 direction = gameObject.transform.forward;
        GameObject bullet = Instantiate(bulletPrefab, barrelPos, Quaternion.Euler(direction));
        bullet.GetComponent<Rigidbody>().AddForce(barrelVelocity * direction, ForceMode.Force);
        bullet.GetComponent<bullet>().damage = damage;

        ammo--;
        if (ammo == 0)
        {
            ammoText.color = Color.red;
        }
        ammoText.text = "Ammo: " + ammo;
        StartCoroutine(nameof(RechamberShot)); // As to slow spam fire
    }

    private void OnEnable()
    {
       
        if (!chambered)
        {
            StartCoroutine(nameof(RechamberShot));
        }
        if (reloading)
        {
            StartCoroutine(nameof(Reload));
        }
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(RechamberShot));
        StopCoroutine(nameof(Reload));
    }

    IEnumerator Reload()
    {
        reloading = true;
        ammoText.color = Color.white;
        ammoText.text = "Reloading";
        yield return new WaitForSeconds(reloadTime / 4);
        for (int i = 0; i < 3; i++)
        {
            ammoText.text += ".";
            yield return new WaitForSeconds(reloadTime / 4);
        }

        ammo = maxAmmo;
        ammoText.text = "Ammo: " + ammo;
        reloading = false;
    }

    IEnumerator RechamberShot()
    {
        chambered = false;
        yield return new WaitForSeconds(60f/fireRate); // Rounds per minute -> seconds per shot
        chambered = true; // Enable shooting after the cooldown
    }

    private void OnDestroy()
    {
        LevelGeneration.OnReady -= OnMapReady;
    }
}
