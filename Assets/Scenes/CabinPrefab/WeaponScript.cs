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
    
    [Header("Weapon States")]
    [SerializeField] private bool equipped;
    [SerializeField] private bool reloading;
    [SerializeField] private bool chambered = true;
    
    void Awake()
    {
        wepPhysics = GetComponent<Rigidbody>();
        canvas = transform.Find("Canvas").gameObject;
        canvas.SetActive(false);

        ammoText.text = "Ammo: " + ammo;
        fireClip = GetComponent<AudioSource>();
        
        LevelGeneration.OnReady += OnMapReady;
        InteractHandler.OnInteract += OnInteract;
        
        // This Try-Catch exists because we need it to defer searching for a Player when they get placed by the map
        // But they still need to check when they get instantiated by player (via dropping)
        try
        {
            player = GameObject.FindGameObjectWithTag("Player");
            PlayerHolsterScript = player.GetComponent<Holster>();
            //holsterTransform = player.transform.Find("Main Camera").transform.Find("WeaponTransform");
            holsterTransform = Camera.main.transform.Find("WeaponTransform");
            //equipped = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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
        LevelGeneration.OnReady += OnMapReady;
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerHolsterScript = player.GetComponent<Holster>();
        holsterTransform = Camera.main.transform.Find("WeaponTransform");
    }

    private void OnInteract(string nm)
    {
        if (gameObject.name == nm && !equipped)
        {
            Debug.Log("Picking up " + nm);
            equipped = true;
            canvas.SetActive(true);
            transform.SetParent(holsterTransform, true);
            transform.position = transform.parent.position;
            transform.rotation = transform.parent.rotation;
            wepPhysics.useGravity = false;
            wepPhysics.isKinematic = true;
            
            PlayerHolsterScript.AddWeaponToHolster(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && equipped)
        {
            equipped = false;
            canvas.SetActive(false);
            transform.SetParent(null);
            wepPhysics.useGravity = true;
            wepPhysics.isKinematic = false;
            wepPhysics.AddForce(2f * Camera.main.transform.forward.normalized, ForceMode.Impulse);
            
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

    private void Shoot()
    {
        fireClip.Play();
        
        Vector3 barrelPos = transform.Find("Barrel").position;
        Vector3 direction = Camera.main.transform.forward;
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
        InteractHandler.OnInteract -= OnInteract;
    }
}
