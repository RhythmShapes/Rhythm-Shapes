using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainButtonAnimation : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
    string GOname;
    Color[] colors;
    public Button button;
    float interpolant;
    int index;
    int colors_length;
    private Vector3 scale;
    private Vector3 original_scale;

    void Start()
    {
        GOname = gameObject.name;
        colors = new Color[] {
            new Color(126f/255f, 125f/255f, 1f),
            new Color(191f/255f, 84f/255f, 155f/255f),
            new Color(255f/255f, 73/255f, 73/255f),
            new Color(255f/255f, 165/255f, 89/255f),
            new Color(255f/255f, 255/255f, 122/255f),
            new Color(194f/255f, 255/255f, 117f/255f),
            new Color(131f/255f, 255/255f, 131/255f),
            new Color(117f/255f, 181f/255f, 180f/255f)
        };
        colors_length= colors.Length;
        original_scale = transform.localScale;
        ResetValues();
    }

    void Update()
    {
        
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null)
        {
            if (currentSelected.name == GOname)
            {
                //Rainbow Color Effect
                ColorBlock colorVar = button.colors;
                colorVar.selectedColor = Color.Lerp(colors[index % colors_length], colors[(index + 1) % colors_length], interpolant/2f);
                button.colors = colorVar;

                //Change Scale
                scale.x = scale.y = 1 + Mathf.Sin(Mathf.PI * interpolant) / 20f;
                transform.localScale = scale;

                interpolant += 0.002f;
                if (interpolant >= 2f)
                {
                    index++; // On change de couleur dans le Lerp
                    interpolant = 0f;
                }
            }
            else
            {
                ResetValues();
            }
        }
        
    }

    void ResetValues()
    {
        index = 0;
        interpolant = 0f;
        transform.localScale = original_scale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting)
            EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.GetComponent<Selectable>().OnPointerExit(null);
    }
}