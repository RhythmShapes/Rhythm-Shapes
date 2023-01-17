using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace edition
{
    [RequireComponent(typeof(Image))]
    public class Graduation : TestLine
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public void Init(float posX, string value, Color color)
        {
            UpdateAll(posX, value);
            text.color = color;
            GetComponent<Image>().color = color;
        }

        public void UpdateAll(float posX, string value)
        {
            UpdatePosX(posX);
            text.SetText(value);
        }
    }
}