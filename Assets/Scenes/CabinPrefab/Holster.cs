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
    [SerializeField] private Weapon activeWeapon;
    [SerializeField] private List<Weapon> holster;

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
        if (Input.GetKeyDown(KeyCode.G) && activeWeapon != null)
        {
            GameObject dropped = Instantiate(activeWeapon.weaponObject, activeWeapon.weaponObject.transform.position, activeWeapon.weaponObject.transform.rotation);
            dropped.name = activeWeapon.weaponObject.name;
            dropped.GetComponent<WeaponScript>().equipped = false;
                
            Rigidbody wepPhysics = dropped.GetComponent<Rigidbody>();
            wepPhysics.useGravity = true;
            wepPhysics.isKinematic = false;
            wepPhysics.AddForce(2f * Camera.main.transform.forward.normalized, ForceMode.Impulse);
            
            
            // Remove the local active weapon's holsterData counterpart (both list and active)
            // Delete the local active weapon's gameobject
            // Remove the local active weapon from local holster list and local active weapon

            foreach (var weapon in holsterData.holster)
            {
                if (weapon.weaponObject.name == activeWeapon.weaponObject.name)
                {
                    holsterData.holster.Remove(weapon);
                    holsterData.activeWeapon = holsterData.holster.Count > 0 ? holsterData.holster[0] : null;
                    break;
                }
            }
            
            holster.Remove(activeWeapon);
            Destroy(activeWeapon.weaponObject);

            activeWeapon = holster.Count > 0 ? holster[0] : null;
        }
        
        if (holster.Count > 0)
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
            
            // Update holsterData active weapon
            // Cycle through holsterData's holster. Find the Weapon that matches our local active
            foreach (var weapon in holsterData.holster)
            {
                if (activeWeapon.weaponObject.name.Contains(weapon.weaponObject.name))
                {
                    holsterData.activeWeapon = weapon;
                }
            }
        }

        // Find a better way to do this, preferably outside Update()
        if (activeWeapon != null)
        {
            foreach (var weapon in holster)
            {
                if (weapon != activeWeapon)
                {
                    weapon.weaponObject.SetActive(false);
                }
                else
                {
                    weapon.weaponObject.SetActive(true);
                }
            }
        }
    }

    public void SetActiveWeapon(Weapon wp)
    {
        activeWeapon = wp;
        //holsterData.activeWeapon = activeWeapon;
    }
    public void AddWeaponToHolster(Weapon wp)
    {
        holster.Add(wp);
        Debug.Log($"Adding {wp.weaponObject.name} to holster");
        
        string[] assets = AssetDatabase.FindAssets("t:Prefab", new string[]{ "Assets/WeaponPrefabs"});
        for (int i = 0; i < assets.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
            Object prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
            if (wp.weaponObject.name.Contains(prefab.name))
            {
                Weapon prefabWep = new Weapon(prefab as GameObject, wp.ammoCount);
                holsterData.holster.Add(prefabWep);
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


// Read/Write data to file
//
// [Serializable]
// public class PlayerInfo
// {
//     public List<Weapon> holster = new ();
// }
