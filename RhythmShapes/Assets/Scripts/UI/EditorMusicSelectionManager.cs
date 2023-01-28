using UI;
using UnityEngine;

public class EditorMusicSelectionManager : MusicSelectionManager
{
    public new static EditorMusicSelectionManager Instance { get; private set; }

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }
}
