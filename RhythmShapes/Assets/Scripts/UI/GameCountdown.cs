using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ui
{
    public class GameCountdown : MonoBehaviour
    {
        [SerializeField] private GameObject background;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private int startValue;
        [SerializeField] private UnityEvent onCountdownFinished;

        private void Awake()
        {
            onCountdownFinished ??= new UnityEvent();
        }

        public void StartCountdown()
        {
            StartCoroutine(CountdownCo());
        }

        private IEnumerator CountdownCo()
        {
            background.SetActive(true);
            countdownText.gameObject.SetActive(true);
        
            for (int i = startValue; i >= 0; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }
        
            background.SetActive(false);
            countdownText.gameObject.SetActive(false);
            onCountdownFinished.Invoke();
            yield return null;
        }
    }
}
