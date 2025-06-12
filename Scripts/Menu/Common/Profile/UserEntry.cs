using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserEntry : MonoBehaviour
{
    public TMP_Text usernameText;
    public Image backgroundImage;

    public void Initialize(string username, Color color)
    {
        usernameText.text = username;
        backgroundImage.color = color;
    }
}