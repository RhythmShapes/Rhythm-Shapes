using TMPro;
using UnityEngine;

namespace ui
{
    public class ComboUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private string prefixText = "Combo x";

        void Update()
        {
            var currentCombo = ScoreManager.Instance.Combo;

            if (currentCombo > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = prefixText + currentCombo;
            }
            else
                comboText.gameObject.SetActive(false);
        }
    }
}
