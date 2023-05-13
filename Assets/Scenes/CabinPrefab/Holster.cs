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
    private HolsterData holsterData;
    public GameObject activeWeapon;
    private List<GameObject> holster = new();

    private void Awake()
    {
        // if (holsterData.holster.Count > 0)
        // {
        //     Transform holsterTransform = Camera.main.transform.Find("WeaponTransform");
        //     foreach (var weapon in holsterData.holster)
        //     {
        //         GameObject equippable = Instantiate(weapon.weaponObject, holsterTransform.position, holsterTransform.rotation, holsterTransform);
        //         equippable.name = weapon.weaponObject.name;
        //         equippable.GetComponent<WeaponScript>().equipped = true;
        //     
        //         Rigidbody wepPhysics = equippable.GetComponent<Rigidbody>();
        //         wepPhysics.useGravity = false;
        //         wepPhysics.isKinematic = true;
        //
        //         Weapon localWeapon = new Weapon(equippable, weapon.ammoCount);
        //         holster.Add(localWeapon);
        //         
        //         if (weapon.weaponObject.name == holsterData.activeWeapon.weaponObject.name)
        //         {
        //             activeWeapon = localWeapon;
        //         }
        //     }
        // }
    }

    private void Update()
    {
        if (holster.Count > 1)
        {
            int newIndex = holster.IndexOf(activeWeapon);
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                newIndex += 1;
                if (newIndex > holster.Count - 1)
                {
                    newIndex = 0;
                }

            }else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                newIndex -= 1;
                if (newIndex < 0)
                {
                    newIndex = holster.Count - 1;
                }
            }
            SetActiveWeapon(holster[newIndex]);
        }
    }
    
    // public void UploadToScriptable()
    // {
    //     foreach (var wp in holster)
    //     {
    //         GameObject prefab = Resources.Load(wp.weaponObject.name) as GameObject;
    //         Weapon prefabWep = new Weapon(prefab, wp.ammoCount);
    //         holsterData.holster.Add(prefabWep);
    //
    //         if (wp == activeWeapon)
    //         {
    //             holsterData.activeWeapon = prefabWep;
    //         }
    //     }
    // }

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
    public int ammoCount;

    public Weapon(GameObject weapon, int ammo)
    {
        weaponObject = weapon;
        ammoCount = ammo;
    }
}