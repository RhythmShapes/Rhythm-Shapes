using TMPro;
using UnityEngine;

namespace ui
{
    public class ScoreUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        //TODO: remplace <ScoreManager> par ta classe
        //[SerializeField] private <ScoreManager> scoreManager;

        void Update()
        {
            //TODO: remplace <ScoreManager>.Score par ta classe et ta propriété pour chopper le score enfion bref t'as compris
            //scoreText = <scoreManager>.Score;
        }
    }
}
