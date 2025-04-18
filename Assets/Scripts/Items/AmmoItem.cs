using UnityEngine;

public class AmmoItem : MonoBehaviour
{
    public int ammoAmount = 30; // Amount of ammo this item provides

    private void OnTriggerEnter(Collider other)
    {
        // Assuming the player has a script called WeaponManager handling weapon selection
        if (other.CompareTag("Player"))
        {
            Weapon weaponManager = FindObjectOfType<Weapon>();
            
            weaponManager.AddAmmo(ammoAmount);
            Destroy(gameObject); 
        }
        else
        {
            Debug.Log("Weapon Scr is null");
        }
    }
}

