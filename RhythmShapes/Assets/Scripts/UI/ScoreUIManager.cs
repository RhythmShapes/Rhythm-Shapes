using TMPro;
using UnityEngine;

namespace ui
{
    public class ScoreUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        void Update()
        {
            var currentScore = ScoreManager.Instance.GetScore();
            scoreText.text = currentScore.ToString();


        }
    }
}
