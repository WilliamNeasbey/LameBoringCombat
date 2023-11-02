using UnityEngine;
using TMPro;
using System.Collections; // Add this using directive for IEnumerator

public class PointCounter : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    public int points = 0;

    public int pointsIncreasePerSecond = 0; // Adjust this value as needed
    private Coroutine increasePointsCoroutine;

    public int Points // Add a public property to access the points
    {
        get { return points; }
        private set { points = value; }
    }

    private void Start()
    {
        UpdatePointsText();
        
    }

    private void UpdatePointsText()
    {
        pointsText.text = "Points: " + points.ToString();
    }

    public void AddPoints(int amount)
    {
        points += amount;
        UpdatePointsText();
    }

  

    
}
