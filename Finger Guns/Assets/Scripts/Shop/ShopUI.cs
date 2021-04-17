using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    #region Variables
    //Public
    public TextMeshProUGUI scoreDisplay;
    public GameObject confirmDialog;
    public GameObject purchaseDialog;
    public TextMeshProUGUI confirmMessage;
    public TextMeshProUGUI purchaseMessage;

    //Private
    private int availablePoints;
    private bool madePurchase = false;
    //PurchaseInfo
    private string itemName;
    private int itemCost;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        availablePoints = FindObjectOfType<GameSession>().GetScore();

        DisplayScore();
    }
    #endregion

    #region Private Methods    
    public void UpdateScore()
    {
        if(madePurchase)
        {            
            FindObjectOfType<GameSession>().SubtractFromScore(itemCost);
            availablePoints -= itemCost;
        }

        madePurchase = false;
        DisplayScore();
    }

    public void DisplayScore()
    {
        scoreDisplay.text = FindObjectOfType<GameSession>().GetScore().ToString();
    }

    public void Purchase(string name, int cost)
    {
        itemName = name;
        itemCost = cost;

        confirmMessage.text = "Do you want to purchase " + name + " for " + cost + " ?";

        if (confirmDialog.activeSelf == false)
            confirmDialog.SetActive(true);
    }

    public void AcceptPurchase()
    {
        if (confirmDialog.activeSelf == true)
        {
            confirmDialog.SetActive(false);            
        }

        //Do we have enough points to purchase?
        if (availablePoints >= itemCost)
        {
            madePurchase = true;
            purchaseMessage.text = "Purchased " + itemName + "!";
            UpdateScore();
        }        
        else
        {
            purchaseMessage.text = "You don't enough points to purchase " + itemName + "!";
        }        
        purchaseDialog.SetActive(true);
    }

    public void CancelPurchase()
    {
        if (confirmDialog.activeSelf == true)
            confirmDialog.SetActive(false);
    }
    #endregion
}