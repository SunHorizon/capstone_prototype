using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchInventory : MonoBehaviour
{

    // buttons for the consumables, weapon, and key items 
    private Button con, weapon, key;
    [SerializeField] private string conName, WeaponName, keyName;

    // get the 3 different inventory
    [SerializeField] private GameObject[] inventory;

    // get the names of inventory
    [SerializeField] private string InvconName, InvWeaponName, InvkeyName;

    // Start is called before the first frame update
    void Start()
    {
        // find the buttons and get their components
        con = GameObject.Find(conName).GetComponent<Button>();
        weapon = GameObject.Find(WeaponName).GetComponent<Button>();
        key = GameObject.Find(keyName).GetComponent<Button>();

        // add Listener to switch inventories
        con.onClick.AddListener(SwitchCon);
        weapon.onClick.AddListener(SwitchWeapon);
        key.onClick.AddListener(SwitchKeyItems);

        {      
            // loop through all inventories
            foreach (GameObject inv in inventory)
            {
                // set active to false if it is not desired inventory
                inv.SetActive(false);
                // set active to true if it is desired inventory
                if (inv.name == InvconName)
                {
                    inv.SetActive(true);
                }
            }
        }
    }

    // switch inventory to con
    void SwitchCon()
    {
        // loop through all inventories
        foreach (GameObject inv in inventory)
        {
            // set active to false if it is not desired inventory
            inv.SetActive(false);
            // set active to true if it is desired inventory
            if (inv.name == InvconName)
            {
                inv.SetActive(true);
            }
        }
    }

    // switch inventory to Weapon
    void SwitchWeapon()
    {
        // loop through all inventories
        foreach (GameObject inv in inventory)
        {
            // set active to false if it is not desired inventory
            inv.SetActive(false);
            // set active to true if it is desired inventory
            if (inv.name == InvWeaponName)
            {
                inv.SetActive(true);
            }
        }
    }

    // switch inventory to KeyItems
    void SwitchKeyItems()
    {
        // loop through all inventories
        foreach (GameObject inv in inventory)
        {
            // set active to false if it is not desired inventory
            inv.SetActive(false);
            // set active to true if it is desired inventory
            if (inv.name == InvkeyName)
            {
                inv.SetActive(true);
            }
        }
    }
}
