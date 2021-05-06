using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    #region Variables
    private string itemName;
    private int itemCost;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        if (transform.Find("Weapon Name")) //If the upgrade is a weapon type
        {
            //Name
            itemName = transform.Find("Weapon Name").GetComponent<TextMeshProUGUI>().text;
            //Cost
            string costText = transform.Find("Weapon Price BG").Find("Weapon Price Text").
                GetComponent<TextMeshProUGUI>().text;
            itemCost = int.Parse(costText);
        }
        else if (transform.Find("Attribute Name")) //If the upgrade is an attribute type
        {
            //Name
            itemName = transform.Find("Attribute Name").GetComponent<TextMeshProUGUI>().text;
            //Cost
            string costText = transform.Find("Attribute Price BG").Find("Attribute Price Text").
                GetComponent<TextMeshProUGUI>().text;
            itemCost = int.Parse(costText);
        }
    }
    #endregion

    #region Private Methods
    public void ConfirmPurchase()
    {
        FindObjectOfType<ShopUI>().Purchase(itemName, itemCost);
    }
    #endregion
}