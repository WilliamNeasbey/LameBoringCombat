using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> names;
    [SerializeField]
    private List<TextMeshProUGUI> scores;

    public string publicLeaderboardKey = "e7f53c0e17e3c7af2c7b6af298d9fd014350f5428f95d0d17cdd68dd2f62ceb5";
    // FNAF secret key 046d52f62cb0958e27c6a8270b0e753a53186eb35353f401283f7c7dfe36c37e31f7d3a70129209afed637c336743e33e6846ed984c26284c8c83d962fea2800268e725268ceb8351459c51f2f19353b3e6d53ad07a32af6d864179691064a793616367e1217df1f7accb485f365fc54562e1b866157c891854975b46ff657aa
    //Secret key because I lost the old one for some reason 30b3d48ef62d6fec0d8014888fb8937ec065e5445e4c29b1911200efb9f60ad3c796ee3417e4ded5a865157308e5c5143ea89a0f7a8e8908df7bf6ab89733191d7cd158a8489cf4b7de783803d8419fe70a373c73208f87a8da69d7ce459c243764059bf38dfab7f63f6ea8fd138cf9aa75480d9d371ab0f6c823d81da6e1bf8
    private void Start()
    {
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) =>
        {
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;
            for (int i = 0; i < loopLength; ++i)
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();

            }
        }));
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, (msg) =>
         {
             //username character limit
            // username.Substring(0, 15);
             GetLeaderboard();
             LeaderboardCreator.ResetPlayer();
         });
        
    }

}
