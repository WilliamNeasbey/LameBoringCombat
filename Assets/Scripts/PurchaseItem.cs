using UnityEngine;
using TMPro;

public class PurchaseItem : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public int itemCost = 200; // Adjust the cost as needed
    public GameObject itemToEnable;

    private int points = 1000; // Initialize with an initial point value

    void Start()
    {
        UpdatePointsText();
    }

    void Update()
    {
        // Check if the player presses the F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CanAffordItem())
            {
                // Deduct the cost of the item
                points -= itemCost;
                UpdatePointsText();

                // Enable the item in the hierarchy
                itemToEnable.SetActive(true);
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
            // Optionally, you can display a message or UI prompt for the player here.
        }
    }
}
