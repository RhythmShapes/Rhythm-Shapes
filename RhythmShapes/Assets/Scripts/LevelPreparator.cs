using System;
using models;
using shape;
using UnityEngine;
using UnityEngine.Events;
using utils;
using utils.XML;

[RequireComponent(typeof(AudioSource))]
public class LevelPreparator : MonoBehaviour
{
    [SerializeField] private UnityEvent onReady;
    [SerializeField] private SpriteRenderer topTargetColor;
    [SerializeField] private SpriteRenderer rightTargetColor;
    [SerializeField] private SpriteRenderer leftTargetColor;
    [SerializeField] private SpriteRenderer bottomTargetColor;
    [SerializeField] private float shapesSpeed;

    public const float TravelTime = 0.85f;
    
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        onReady ??= new UnityEvent();
    }

    public void Init(LevelDescription level)
    {
        float squareSpeedAdjustment = PathsManager.Instance.GetPathTotalDistance(ShapeType.Square) /
                                      PathsManager.Instance.GetPathTotalDistance(ShapeType.Diamond);
        float circleSpeedAdjustment = PathsManager.Instance.GetPathTotalDistance(ShapeType.Circle) /
                                      PathsManager.Instance.GetPathTotalDistance(ShapeType.Diamond);

        if (level.shapes != null)
        {
            GameModel.Instance.Reset();
            Array.Sort(level.shapes, (shape, compare) => shape.timeToPress.CompareTo(compare.timeToPress));

            foreach (var shapeDescription in level.shapes)
            {
                shapeDescription.timeToPress = Utils.RoundTime(shapeDescription.timeToPress);
                float timeToPress = shapeDescription.timeToPress + GameInfo.AudioCalibration;
                
                if (timeToPress < 0f || timeToPress > _audioSource.clip.length)
                    continue;
                
                Vector2[] path = PathsManager.Instance.GetPath(shapeDescription.type, shapeDescription.target,
                    shapeDescription.goRight);
                Color color = GetShapeColor(shapeDescription.target);
                float speed = shapesSpeed * shapeDescription.type switch // speedAdjustment according to shape
                {
                    ShapeType.Square => squareSpeedAdjustment,
                    ShapeType.Circle => circleSpeedAdjustment,
                    _ => 1
                };
                
                float timeToSpawn = GetShapeTimeToSpawn(shapeDescription.timeToPress) + GameInfo.AudioCalibration;

                if (timeToSpawn < 0f || timeToSpawn > _audioSource.clip.length)
                    continue;
                
                GameModel.Instance.PushShapeModel(
                    new ShapeModel(shapeDescription.type, shapeDescription.target, color, path,
                        shapeDescription.timeToPress, timeToSpawn, speed)
                );
            }
        }
        
        onReady.Invoke();
    }

    private Color GetShapeColor(Target target)
    {
        return target switch
        {
            Target.Top => topTargetColor.color,
            Target.Right => rightTargetColor.color,
            Target.Left => leftTargetColor.color,
            Target.Bottom => bottomTargetColor.color,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static float GetShapeTimeToSpawn(float timeToPress, float travelTime = TravelTime)
    {
        return timeToPress - travelTime;
    }
}
