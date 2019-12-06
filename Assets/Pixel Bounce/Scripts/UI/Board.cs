using UnityEngine;

public class Board : MonoBehaviour
{
    public TextMesh nameText;
    public TextMesh scoreText;
    public TextMesh scoreShadowText;

    public void UpdateText(string name,int score)
    {
        nameText.text = name;
        scoreText.text = scoreShadowText.text = score.ToString("0000");
    }
}
