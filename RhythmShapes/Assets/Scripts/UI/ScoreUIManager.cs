using TMPro;
using UnityEngine;

namespace ui
{
    public class ScoreUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Animator scoreAnimator;

        private int _lastScore = 0;

        void Update()
        {
            var currentScore = ScoreManager.Instance.GetScore();
            scoreText.text = currentScore.ToString();
            
            if(currentScore > _lastScore)
                scoreAnimator.SetTrigger("OnIncreased");

            _lastScore = currentScore;
        }
    }
}
