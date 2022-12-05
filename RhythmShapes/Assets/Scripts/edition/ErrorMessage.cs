using TMPro;
using UnityEngine;

namespace edition
{
    public class ErrorMessage : MonoBehaviour
    {
        [SerializeField] private GameObject errorMessage;
        [SerializeField] private TextMeshProUGUI errorText;

        public void ShowError(string message)
        {
            errorText.text = message;
            errorMessage.SetActive(true);
        }

        public void HideError()
        {
            errorMessage.SetActive(false);
        }
    }
}