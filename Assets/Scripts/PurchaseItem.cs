using UnityEngine;
using TMPro;

public class PurchaseItem : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public int itemCost = 200;
    public GameObject[] itemsToEnable; // Array of objects to enable
    public GameObject[] itemsToDisable; // Array of objects to disable
    public GameObject purchaseUI;
    public GameObject alreadyPurchasedUI;

    public PointCounter pointCounter; // Reference to the PointCounter script
    private bool isPlayerInsideTrigger = false;
    private bool hasPurchasedItem = false;

    void Start()
    {
        // Find the PointCounter script in the scene and store a reference to it
        pointCounter = FindObjectOfType<PointCounter>();

        if (pointCounter == null)
        {
            Debug.LogError("PointCounter script not found in the scene.");
        }

        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isPlayerInsideTrigger)
        {
            if (CanAffordItem() && !hasPurchasedItem)
            {
                if (pointCounter != null)
                {
                    if (pointCounter.points >= itemCost)
                    {
                        pointCounter.AddPoints(-itemCost);
                        pointsText.text = "Points: " + pointCounter.points.ToString();
                    }
                    else
                    {
                        Debug.Log("Not enough points to purchase the item.");
                        return;
                    }
                }
                else
                {
                    Debug.LogError("PointCounter reference is null.");
                    return;
                }

                foreach (GameObject itemToEnable in itemsToEnable)
                {
                    itemToEnable.SetActive(true);
                }

                foreach (GameObject itemToDisable in itemsToDisable)
                {
                    if (itemToDisable != null)
                    {
                        itemToDisable.SetActive(false); // Disable the specified object
                    }
                }

                hasPurchasedItem = true;
                UpdateUI();
            }
            else
            {
                Debug.Log("Not enough points to purchase the item.");
            }
        }
    }

    void UpdateUI()
    {
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
        if (pointCounter != null)
        {
            return pointCounter.points >= itemCost;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;
            UpdateUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
            UpdateUI();
        }
    }
}
