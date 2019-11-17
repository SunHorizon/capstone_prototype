using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{

    public GameObject[] inventory;

    [SerializeField] private string InvconName, InvWeaponName, InvkeyName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            foreach (GameObject inv in inventory)
            {
                if (inv.name == InvconName)
                {
                    inv.GetComponent<Inventory>().AddItem(other.GetComponent<ItemScript>());
                    
                }
            }      
        }

        if (other.tag == "Weapon")
        {
            foreach (GameObject inv in inventory)
            {
                if (inv.name == InvWeaponName)
                {
                    inv.GetComponent<Inventory>().AddItem(other.GetComponent<ItemScript>());
                }
            }
        }
    }
}
