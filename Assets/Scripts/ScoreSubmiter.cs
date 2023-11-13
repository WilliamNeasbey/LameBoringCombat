using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreSubmiter : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI inputScore;
    [SerializeField]
    private TMP_InputField inputName;

    public UnityEvent<string, int> submitScoreEvent;

    public void SubmitScore()
    {
        submitScoreEvent.Invoke(inputName.text, int.Parse(inputScore.text));
    }

    private void Update()
    {
        // Get the player's score from the PointsCounter script 
        int playerScore = FindObjectOfType<PointCounter>().points;

        inputScore.text = "" + playerScore;
        
    }

}
