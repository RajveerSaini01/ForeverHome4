using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float damage = 50;
    public float fireRate = 300; // RPM
    public float barrelVelocity = 500f; // m/s
    public int maxAmmo = 10;
    public int ammo = 10;
    public float reloadTime = 0.75f;
    
 
    public AudioSource fireClip;
    
    [SerializeField] private GameObject player;
    [SerializeField] private Transform holsterTransform;
    private Holster PlayerHolsterScript;

    public bool equipped;
    private bool reloading;
    private bool chambered = true;

    private GameObject canvas;
    public TextMeshProUGUI ammoText;
    
    void Awake()
    {
        LevelGeneration.OnReady += OnMapReady;
        canvas = transform.Find("Canvas").gameObject;
        ammoText.text = "Ammo: " + ammo;
        fireClip = GetComponent<AudioSource>();
        InteractHandler.OnInteract += OnInteract;
        
        // InteractHandler.OnInteract += OnInteract;
        //
        // player = GameObject.FindGameObjectWithTag("Player");
        // PlayerHolsterScript = player.GetComponent<Holster>();
        // //holsterTransform = player.transform.Find("Main Camera").transform.Find("WeaponTransform");
        // holsterTransform = Camera.main.transform.Find("WeaponTransform");
        //
        // canvas = transform.Find("Canvas").gameObject;
        // ammoText.text = "Ammo: " + ammo;
        // fireClip = GetComponent<AudioSource>();

        // This Try-Catch exists because we need it to defer searching for a Player when they get placed by the map
        // But they still need to check when they get instantiated by player (via dropping)
        try
        {
            player = GameObject.FindGameObjectWithTag("Player");
            PlayerHolsterScript = player.GetComponent<Holster>();
            //holsterTransform = player.transform.Find("Main Camera").transform.Find("WeaponTransform");
            holsterTransform = Camera.main.transform.Find("WeaponTransform");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    private void OnMapReady()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerHolsterScript = player.GetComponent<Holster>();
        //holsterTransform = player.transform.Find("Main Camera").transform.Find("WeaponTransform");
        holsterTransform = Camera.main.transform.Find("WeaponTransform");
    }

    private void OnInteract(string nm)
    {
        if (gameObject.name == nm)
        {
            //Debug.Log("Picking up " + nm);
            GameObject equippable = Instantiate(gameObject, holsterTransform.position, holsterTransform.rotation, holsterTransform);
            equippable.name = name;
            equippable.GetComponent<WeaponScript>().equipped = true;
            
            Rigidbody wepPhysics = equippable.GetComponent<Rigidbody>();
            wepPhysics.useGravity = false;
            wepPhysics.isKinematic = true;
            
            Weapon holstered = new Weapon(equippable, ammo);
            PlayerHolsterScript.AddWeaponToHolster(holstered);
            PlayerHolsterScript.SetActiveWeapon(holstered);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
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

        // This shouldn't be in Update; too expensive. Put inside OnInteract as modifer post-instantiation
        if (canvas != null)
        {
            canvas.SetActive(equipped);
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
