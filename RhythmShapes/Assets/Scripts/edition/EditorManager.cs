using UnityEngine;

namespace edition
{
    public class EditorManager : MonoBehaviour
    {
        [Header("Dev variables")]
        [Space]
        [SerializeField] private bool useDevVariables;
        [SerializeField] private string devLevelName;
        [SerializeField] private bool isNewMusic = true;

        private void Start()
        {
            string levelName = useDevVariables ? devLevelName : GameInfo.LevelName;
            InspectorPanel.Instance.SetShape(null);
        
            if (GameInfo.IsNewMusic || (useDevVariables && isNewMusic))
                EditorPanel.Instance.SetLevel(null);
            else
                LevelLoader.Instance.LoadLevelFromFile(levelName);
        }
    }
}