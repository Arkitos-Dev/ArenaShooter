using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponSwitching : MonoBehaviour
{
    public List<GameObject> weapons;
    public List<Weapon> weaponScr;
    private int currentWeaponIndex = 0;
    public float dropForce = 3f;
    
    public Transform fpsCam;
    public GameObject Pistol;
    public GameObject Shotgun;
    public GameObject Rifle;
    public TwoBoneIKConstraint targetR;
    public TwoBoneIKConstraint targetL;
    public Transform pistolR;
    public Transform pistolL;
    public Transform shotgunR;
    public Transform shotgunL;
    public Transform rifleR;
    public Transform rifleL;
    public RigBuilder rigBuilder;
    public Animator arms;
    public Animator pistol;
    private Weapon weapon;
    
    

    private void Start()
    { 
        SwitchWeapon(currentWeaponIndex);
    }

    private void Update()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SwitchWeapon(i);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            Drop();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    private void SwitchWeapon(int newIndex)
    {
        // Check if the index is within the valid range
        if (newIndex >= 0 && newIndex < weapons.Count)
        {
            // Deactivate the current weapon
            weapons[currentWeaponIndex].SetActive(false);

            // Update the current index
            currentWeaponIndex = newIndex;

            // Activate the new weapon
            weapons[currentWeaponIndex].SetActive(true);
            
            weapon = weapons[currentWeaponIndex].GetComponent<Weapon>(); 
        }

        // Adjust arms and targets based on the new weapon
        switch (currentWeaponIndex)
        {
            case 0:
                targetR.data.target = pistolR;
                targetL.data.target = pistolL;
                arms.SetBool("pistol", true);
                arms.SetBool("shotgun", false);
                arms.SetBool("rifle", false);
                rigBuilder.Build();
                break;
            case 1:
                targetR.data.target = shotgunR;
                targetL.data.target = shotgunL;
                arms.SetBool("pistol", false);
                arms.SetBool("shotgun", true);
                arms.SetBool("rifle", false);
                rigBuilder.Build();
                break;
            case 2:
                targetR.data.target = rifleR;
                targetL.data.target = rifleL;
                arms.SetBool("pistol", false);
                arms.SetBool("shotgun", false);
                arms.SetBool("rifle", true);
                rigBuilder.Build();
                break;
        }

        // Update ammo text after all configurations are set
        weapon.UpdateAmmoText();
    }
    
    public void AddToCurrentWep(int ammAmount)
    {
        if (currentWeaponIndex >= 0 && currentWeaponIndex < weaponScr.Count)
        {
            weaponScr[currentWeaponIndex].AddAmmo(ammAmount);
        }
    }

    private void Drop()
    {
        GameObject currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.SetActive(false); // Deactivate the current weapon
    
        Vector3 dropPosition = fpsCam.transform.position + fpsCam.transform.forward * 2f;
        GameObject droppedWeapon = Instantiate(currentWeapon, dropPosition, Quaternion.LookRotation(fpsCam.transform.forward));

        // Configure the dropped weapon
        droppedWeapon.SetActive(true);
        Rigidbody droppedRb = droppedWeapon.AddComponent<Rigidbody>();
        BoxCollider boxCollider = droppedWeapon.AddComponent<BoxCollider>();
        droppedRb.AddForce(fpsCam.transform.forward * dropForce, ForceMode.Impulse);

        // Remove scripts from the dropped weapon
        foreach (MonoBehaviour script in droppedWeapon.GetComponents<MonoBehaviour>())
        {
            Destroy(script);
        }

        // Remove the original weapon from the list
        weapons.RemoveAt(currentWeaponIndex);
        if (currentWeaponIndex >= weapons.Count && weapons.Count > 0)
        {
            currentWeaponIndex = 0; // reset to the first weapon
        }
    }
    
    private void PickUp()
    {
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, 3f))
        {
            GameObject pickedUpWeapon = hit.collider.gameObject;

            switch (pickedUpWeapon.tag)
            {
                case "Pistol":
                    weapons.Add(Pistol);
                    Destroy(pickedUpWeapon);
                    SwitchWeapon(weapons.IndexOf(Pistol));
                    break;
                case "Rifle":
                    weapons.Add(Rifle);
                    Destroy(pickedUpWeapon);
                    SwitchWeapon(weapons.IndexOf(Rifle));
                    break;
                case "Shotgun":
                    weapons.Add(Shotgun);
                    Destroy(pickedUpWeapon);
                    SwitchWeapon(weapons.IndexOf(Shotgun));
                    break;
            }
        }
    }
}
