using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Holster : MonoBehaviour
{
    [SerializeField] private Weapon activeWeapon;
    public List<Weapon> holster;
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
            
            holster.Remove(activeWeapon);
            Destroy(activeWeapon.weaponObject);
            activeWeapon = (holster.Count > 0) ? holster[0] : null;
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

    public void setActiveWeapon(Weapon wp)
    {
        activeWeapon = wp;
    }
}


[Serializable]
public class Weapon
{
    public GameObject weaponObject;
    [SerializeField] private int ammoCount;

    public Weapon(GameObject weapon, int ammo)
    {
        weaponObject = weapon;
        ammoCount = ammo;
    }
}
