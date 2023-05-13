using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEditor;
using Object = UnityEngine.Object;

public class Holster : MonoBehaviour
{
    [SerializeField] private HolsterData holsterData;
    public GameObject activeWeapon;
    private List<GameObject> holster = new();

    private void Awake()
    {
        if (holsterData.holster.Count > 0)
        {
            Transform holsterTransform = Camera.main.transform.Find("WeaponTransform");
            foreach (var weapon in holsterData.holster)
            {
                GameObject equippable = Instantiate(weapon.weaponObject, holsterTransform.position, holsterTransform.rotation, holsterTransform);
                equippable.name = weapon.weaponObject.name;
                WeaponScript weaponScript = equippable.GetComponent<WeaponScript>();
                weaponScript.SetValues(weapon.damage, weapon.fireRate, weapon.barrelVelocity, weapon.maxAmmo,
                    weapon.ammo, weapon.reloadTime, true);
                
                
                Rigidbody wepPhysics = equippable.GetComponent<Rigidbody>();
                wepPhysics.useGravity = false;
                wepPhysics.isKinematic = true;
        
                AddWeaponToHolster(equippable);
            }
        }
    }

    private void Update()
    {
        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (holster.Count > 1 && axis != 0f)
        {
            int newIndex = holster.IndexOf(activeWeapon);
            if (axis > 0f)
            {
                newIndex += 1;
                if (newIndex > holster.Count - 1)
                {
                    newIndex = 0;
                }

            }else if (axis < 0f)
            {
                newIndex -= 1;
                if (newIndex < 0)
                {
                    newIndex = holster.Count - 1;
                }
            }
            SetActiveWeapon(holster[newIndex]);
        }

        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     UploadToScriptable();
        // }
    }
    
    public void UploadToScriptable()
    {
        holsterData.holster.Clear();
        holsterData.activeWeapon = null;
        foreach (var weapon in holster)
        {
            GameObject prefab = Resources.Load(weapon.name) as GameObject;
            WeaponScript weaponScript = weapon.GetComponent<WeaponScript>();
            Weapon prefabWep = new Weapon(prefab, weaponScript.damage, weaponScript.fireRate, weaponScript.barrelVelocity, weaponScript.maxAmmo, weaponScript.ammo, weaponScript.reloadTime);
            
            
            holsterData.holster.Add(prefabWep);
    
            if (weapon == activeWeapon)
            {
                holsterData.activeWeapon = prefabWep;
            }
        }
    }

    public void AddWeaponToHolster(GameObject o)
    {
        holster.Add(o);
        SetActiveWeapon(o);
    }

    public void RemoveWeaponFromHolster(GameObject o)
    {
        holster.Remove(o);
        if (holster.Count > 0)
        {
            SetActiveWeapon(holster[0]);
        }
        else
        {
            activeWeapon = null;
        }
        
    }

    public void SetActiveWeapon(GameObject o)
    {
        foreach (var weapon in holster)
        {
            weapon.SetActive(weapon == o);
        }
        activeWeapon = o;
    }
}


[Serializable]
public class Weapon
{
    public GameObject weaponObject;
    public float damage;
    public float fireRate; // RPM
    public float barrelVelocity; // m/s
    public int maxAmmo;
    public int ammo;
    public float reloadTime;

    public Weapon(GameObject gameObject, float dmg, float fr, float bv, int mAmmo, int cAmmo, float rt)
    {
        weaponObject = gameObject;
        damage = dmg;
        fireRate = fr;
        barrelVelocity = bv;
        maxAmmo = mAmmo;
        ammo = cAmmo;
        reloadTime = rt;
    }
}