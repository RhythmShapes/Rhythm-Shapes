using System.Collections;
using TMPro;
using UnityEngine;

namespace ui
{
    public class ComboUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private Animator comboAnimator;
        [SerializeField] private string prefixText = "Combo x";
        [SerializeField] private float lostComboAnimDuration = 0.02f;
        [SerializeField] private float lostComboFallSpeed = 5f;

        private int _lastCombo = 0;
        private bool _isAnimated;

        void Update()
        {
            var currentCombo = ScoreManager.Instance.Combo;

            if (currentCombo > 1)
            {
                comboText.gameObject.SetActive(true);
                comboText.text = prefixText + currentCombo;
            
                if(currentCombo > _lastCombo)
                    comboAnimator.SetTrigger("OnIncreased");
            }
            else if (currentCombo < _lastCombo)
            {
                StartCoroutine(LostComboAnim());
            }
            else if(currentCombo == _lastCombo && currentCombo == 0 && !_isAnimated)
                comboText.gameObject.SetActive(false);

            _lastCombo = currentCombo;
        }

        private IEnumerator LostComboAnim()
        {
            _isAnimated = true;
            float alphaDecrease = comboText.alpha / lostComboAnimDuration;
            Vector3 originPos = comboText.transform.position;
            float originAlpha = comboText.alpha;

            while (comboText.alpha > 0f)
            {
                comboText.alpha = Mathf.Max(0f, comboText.alpha - alphaDecrease * Time.deltaTime);
                Vector3 pos = comboText.transform.position;
                pos.y -= lostComboFallSpeed * Time.deltaTime;
                comboText.transform.position = pos;
                yield return null;
            }
            
            comboText.gameObject.SetActive(false);
            comboText.alpha = originAlpha;
            comboText.transform.position = originPos;
            _isAnimated = false;
            yield return null;
        }
    }
}
