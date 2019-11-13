using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSaveInventory : MonoBehaviour
{
    [SerializeField] private Inventory[] inventory;

    // prototype of the items
    public GameObject Mana;
    public GameObject Health;
    public GameObject Sword;

    public void SaveInventory()
    {
        int invNum = 0;
        foreach (Inventory inv in inventory)
        {
            // make the string varible to save
            string content = string.Empty;

            for (int i = 0; i < inv.allSlots.Count; i++)
            {
                // get the slot
                Slot tmp = inv.allSlots[i].GetComponent<Slot>();

                if (!tmp.isEmpty)
                {
                    // "slot-type-count"
                    // saving the data in the string
                    content += i + "-" + tmp.CurrentItem.type.ToString() + "-" + tmp.Items.Count.ToString() + ";";
                }
            }

            // saving the content the PlayerPrefs
            PlayerPrefs.SetString("content"+ invNum, content);
            PlayerPrefs.SetInt("slots"+ invNum, inv.slots);
            PlayerPrefs.SetInt("rows" + invNum, inv.rows);
            PlayerPrefs.SetFloat("slotPaddingLeft"+ invNum, inv.slotPaddingLeft);
            PlayerPrefs.SetFloat("slotPaddingTop"+ invNum, inv.slotPaddingTop);
            PlayerPrefs.SetFloat("slotSize"+ invNum, inv.slotSize);
            //PlayerPrefs.SetFloat("xPos", inventoryRect.position.x);
            //PlayerPrefs.SetFloat("yPos", inventoryRect.position.y);

            Debug.Log("Saving");
            PlayerPrefs.Save();
            invNum++;
        }
    }

    public void LoadInventory()
    {
        int invNum = 0;
        foreach (Inventory inv in inventory)
        {
            // load the content
            string content = PlayerPrefs.GetString("content"+ invNum);

            // load the inventory data
            inv.slots = PlayerPrefs.GetInt("slots"+ invNum);
            inv.rows = PlayerPrefs.GetInt("rows" + invNum);
            inv.slotPaddingLeft = PlayerPrefs.GetFloat("slotPaddingLeft"+ invNum);
            inv.slotPaddingTop = PlayerPrefs.GetFloat("slotPaddingTop"+ invNum);
            inv.slotSize = PlayerPrefs.GetFloat("slotSize"+ invNum);

            //inventoryRect.position = new Vector3(PlayerPrefs.GetFloat("xPos"), PlayerPrefs.GetFloat("yPos"), inventoryRect.position.z);


            // create the layout
            inv.CreateLayout();

            // put the content back into the inventory
            //0-Mana-3
            string[] splitContent = content.Split(";".ToCharArray()); // 0-Mana-3

            // loop through the split content
            for (int x = 0; x < splitContent.Length - 1; x++)
            {
                // split the first value 
                string[] splitValues = splitContent[x].Split("-".ToCharArray());

                // get the slot 
                int index = Int32.Parse(splitValues[0]); //"0"

                // get the item type
                ItemTpye type = (ItemTpye) Enum.Parse(typeof(ItemTpye), splitValues[1]); // "mana"

                // get the amount of items in the slot
                int amount = Int32.Parse(splitValues[2]); //"3"

                // place the items in the slot
                for (int i = 0; i < amount; i++)
                {
                    // add the items into the slot of inventory
                    switch (type)
                    {
                        case ItemTpye.Mana:
                            inv.allSlots[index].GetComponent<Slot>().AddItem(Mana.GetComponent<Items>());
                            break;
                        case ItemTpye.Health:
                            inv.allSlots[index].GetComponent<Slot>().AddItem(Health.GetComponent<Items>());
                            break;
                        case ItemTpye.Sword:
                            inv.allSlots[index].GetComponent<Slot>().AddItem(Sword.GetComponent<Items>());
                            break;
                    }
                }

            }
            invNum++;
        }
        Debug.Log("Loading");
    }
}
