using UnityEngine;
using TMPro;

public class PurchaseItem : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public int itemCost = 200; // Adjust the cost as needed
    public GameObject itemToEnable;
    public GameObject purchaseUI;
    public GameObject alreadyPurchasedUI;

    private int points = 1000; // Initialize with an initial point value
    private bool isPlayerInsideTrigger = false; // Flag to track if the player is inside the trigger
    private bool hasPurchasedItem = false; // Flag to track if the player has purchased the item

    void Start()
    {
        UpdatePointsText();
        UpdateUI();
    }

    void Update()
    {
        // Check if the player presses the F key and is inside the trigger
        if (Input.GetKeyDown(KeyCode.F) && isPlayerInsideTrigger)
        {
            if (CanAffordItem() && !hasPurchasedItem)
            {
                // Deduct the cost of the item
                points -= itemCost;
                UpdatePointsText();

                // Enable the item in the hierarchy
                itemToEnable.SetActive(true);

                // Set the flag to indicate the item has been purchased
                hasPurchasedItem = true;

                // Update the UI
                UpdateUI();
            }
            else
            {
                Debug.Log("Not enough points to purchase the item.");
            }
        }
    }

    void UpdatePointsText()
    {
        pointsText.text = "Points: " + points.ToString();
    }

    void UpdateUI()
    {
        // Show the appropriate UI based on whether the item has been purchased
        if (!hasPurchasedItem)
        {
            purchaseUI.SetActive(isPlayerInsideTrigger);
            alreadyPurchasedUI.SetActive(false);
        }
        else
        {
            purchaseUI.SetActive(false);
            alreadyPurchasedUI.SetActive(isPlayerInsideTrigger);
        }
    }

    bool CanAffordItem()
    {
        return points >= itemCost;
    }

    public void AddPoints(int amount)
    {
        points += amount;
        UpdatePointsText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger zone.");
            isPlayerInsideTrigger = true;
            // Update the UI when the player enters the trigger zone
            UpdateUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger zone.");
            isPlayerInsideTrigger = false;
            // Update the UI when the player exits the trigger zone
            UpdateUI();
        }
    }
}
