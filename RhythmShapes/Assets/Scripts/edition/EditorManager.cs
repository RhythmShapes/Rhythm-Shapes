using UnityEngine;

namespace edition
{
    public class EditorManager : MonoBehaviour
    {
        [SerializeField] private LevelLoader levelLoader;
        [Header("Dev variables")]
        [Space]
        [SerializeField] private bool useDevVariables;
        [SerializeField] private string devLevelName;
        [SerializeField] private bool isNewLevel = true;

        private void Start()
        {
            string levelName = useDevVariables ? devLevelName : GameInfo.LevelName;
            GameInfo.IsNewLevel = useDevVariables && isNewLevel;
            EditorPanel.Init();
        
            if (!GameInfo.IsNewLevel)
                levelLoader.LoadLevelFromFile(levelName);
        }
    }
}