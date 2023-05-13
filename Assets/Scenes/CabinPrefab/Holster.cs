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
    public Weapon activeWeapon;
    private List<Weapon> holster = new();

    private void Awake()
    {
        if (holsterData.holster.Count > 0)
        {
            Transform holsterTransform = Camera.main.transform.Find("WeaponTransform");
            foreach (var weapon in holsterData.holster)
            {
                GameObject equippable = Instantiate(weapon.weaponObject, holsterTransform.position, holsterTransform.rotation, holsterTransform);
                equippable.name = weapon.weaponObject.name;
                equippable.GetComponent<WeaponScript>().equipped = true;
            
                Rigidbody wepPhysics = equippable.GetComponent<Rigidbody>();
                wepPhysics.useGravity = false;
                wepPhysics.isKinematic = true;

                Weapon localWeapon = new Weapon(equippable, weapon.ammoCount);
                holster.Add(localWeapon);
                
                if (weapon.weaponObject.name == holsterData.activeWeapon.weaponObject.name)
                {
                    activeWeapon = localWeapon;
                }
            }
        }
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.G) && activeWeapon != null)
        // {
        //     GameObject dropped = Instantiate(activeWeapon.weaponObject, activeWeapon.weaponObject.transform.position, activeWeapon.weaponObject.transform.rotation);
        //     dropped.name = activeWeapon.weaponObject.name;
        //     dropped.GetComponent<WeaponScript>().equipped = false;
        //         
        //     Rigidbody wepPhysics = dropped.GetComponent<Rigidbody>();
        //     wepPhysics.useGravity = true;
        //     wepPhysics.isKinematic = false;
        //     wepPhysics.AddForce(2f * Camera.main.transform.forward.normalized, ForceMode.Impulse);
        //     
        //     Destroy(activeWeapon.weaponObject);
        // }
        
        if (holster.Count > 1)
        {
            int newIndex = holster.IndexOf(activeWeapon);
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                newIndex = Math.Clamp(newIndex+1, 0, holster.Count-1);
            
            }else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                newIndex = Math.Clamp(newIndex-1, 0, holster.Count-1);
            }
            activeWeapon = holster[newIndex];
        }

        // Find a better way to do this, preferably outside Update()
        if (activeWeapon != null)
        {
            foreach (var weapon in holster)
            {
                weapon.weaponObject.SetActive(weapon == activeWeapon);
            }
        }
    }

    public void SetActiveWeapon(Weapon wp)
    {
        activeWeapon = wp; 
    }
    public void AddWeaponToHolster(Weapon wp)
    {
        holster.Add(wp);
    }

    public void RemoveWeaponFromHolster(Weapon wp)
    {
        holster.Remove(wp);
    }

    public void ClearHolster()
    {
        holster.Clear();
    }

    public List<Weapon> GetHolster()
    {
        return holster;
    }
    public int GetHolsterSize()
    {
        return holster.Count;
    }
    public void UploadToScriptable()
    {
        foreach (var wp in holster)
        {
            GameObject prefab = Resources.Load(wp.weaponObject.name) as GameObject;
            Weapon prefabWep = new Weapon(prefab, wp.ammoCount);
            holsterData.holster.Add(prefabWep);

            if (wp == activeWeapon)
            {
                holsterData.activeWeapon = prefabWep;
            }
        }
    }
}


[Serializable]
public class Weapon
{
    public GameObject weaponObject;
    public int ammoCount;

    public Weapon(GameObject weapon, int ammo)
    {
        weaponObject = weapon;
        ammoCount = ammo;
    }
}