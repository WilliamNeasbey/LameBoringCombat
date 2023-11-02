using UnityEngine;
using TMPro;

public class PointCounter : MonoBehaviour
{
    public TextMeshProUGUI pointsText;
    private int points = 0;

    public int pointsIncreasePerSecond = 100; // Adjust this value as needed
    private Coroutine increasePointsCoroutine;

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

  

    private void StopIncreasingPoints()
    {
        if (increasePointsCoroutine != null)
        {
            StopCoroutine(increasePointsCoroutine);
            increasePointsCoroutine = null;
        }
    }

   

    private void OnDestroy()
    {
        StopIncreasingPoints();
    }
}
