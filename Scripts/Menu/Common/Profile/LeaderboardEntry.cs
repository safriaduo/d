using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : UserEntry
{
    public TMP_Text scoreText;
    public TMP_Text rankText;
    public GameObject ownerBackground;

    public Color firstPlace;
    public Color secondPlace;
    public Color thirdPlace;
    public Color defaultPlace;

    public void Initialize(string username, string score, string rank, bool isOwner)
    {
        scoreText.text = $"{score}";
        rankText.text = $"#{rank}";

        ownerBackground.SetActive(isOwner);

        if (int.TryParse(rank, out int rankNum))
        {
            Color bgColor = Color.white; 

            switch (rankNum)
            {
                case 1:
                    bgColor = firstPlace;
                    break;

                case 2:
                    bgColor = secondPlace;
                    break;

                case 3:
                    bgColor = thirdPlace;
                    break;

                default:
                    bgColor = defaultPlace;
                    break;
            }

            base.Initialize(username, bgColor);
        }
    }
}