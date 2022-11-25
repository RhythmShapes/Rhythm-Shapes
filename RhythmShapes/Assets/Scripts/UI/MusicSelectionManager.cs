using System.IO;
using ui;
using UnityEngine;

public class MusicSelectionManager : MonoBehaviour
{
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject buttonPrefab;

    private void Start()
    {
        foreach (string levelName in Directory.GetDirectories(Path.Combine(Application.persistentDataPath, "Levels")))
        {
            Instantiate(buttonPrefab, content).GetComponent<MenuButton>().Init(Path.GetFileName(levelName));
        }
    }
}
