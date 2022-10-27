using shape;
using UnityEngine;
using UnityEngine.Events;
using utils.XML;

public class FileLoader : MonoBehaviour
{
    public UnityEvent<LevelDescription> onLoadedEvent;
    
    [Header("Test values")]
    [Space]
    [SerializeField] private TextAsset levelXML;
    [SerializeField] private SpriteRenderer topTargetColor;
    [SerializeField] private SpriteRenderer rightTargetColor;
    [SerializeField] private SpriteRenderer leftTargetColor;
    [SerializeField] private SpriteRenderer bottomTargetColor;
    [SerializeField] private float shapesSpeed;

    private void Awake()
    {
        onLoadedEvent ??= new UnityEvent<LevelDescription>();
    }

    private void Start()
    {
        LevelDescription level = XmlHelpers.DeserializeFromXML<LevelDescription>(levelXML);

        level.targetColors = new TargetDescription[4];
        level.targetColors[0] = new TargetDescription { color = topTargetColor.color, target = Target.Top };
        level.targetColors[1] = new TargetDescription { color = rightTargetColor.color, target = Target.Right };
        level.targetColors[2] = new TargetDescription { color = leftTargetColor.color, target = Target.Left };
        level.targetColors[3] = new TargetDescription { color = bottomTargetColor.color, target = Target.Bottom };

        foreach (ShapeDescription shape in level.shapes)
        {
            shape.pathToFollow = PathsManager.Instance.GetPath(shape.type, shape.target, shape.goRight);
            shape.speed = shapesSpeed;
        }
        
        onLoadedEvent.AddListener(GameplayManager.Instance.Init);
        onLoadedEvent.Invoke(level);
    }
}