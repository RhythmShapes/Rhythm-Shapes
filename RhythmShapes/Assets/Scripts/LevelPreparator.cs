using System;
using models;
using shape;
using UnityEngine;
using UnityEngine.Events;
using utils.XML;

public class LevelPreparator : MonoBehaviour
{
    [SerializeField] private UnityEvent onReady;
    [SerializeField] private SpriteRenderer topTargetColor;
    [SerializeField] private SpriteRenderer rightTargetColor;
    [SerializeField] private SpriteRenderer leftTargetColor;
    [SerializeField] private SpriteRenderer bottomTargetColor;
    [SerializeField] private float shapesSpeed;
    
    public void Init(LevelDescription level)
    {
        float squareSpeedAdjustment = PathsManager.Instance.GetPathTotalDistance(ShapeType.Square) /
                                      PathsManager.Instance.GetPathTotalDistance(ShapeType.Diamond);
        float circleSpeedAdjustment = PathsManager.Instance.GetPathTotalDistance(ShapeType.Circle) /
                                      PathsManager.Instance.GetPathTotalDistance(ShapeType.Diamond);
        
        foreach (var shapeDescription in level.shapes)
        {
            Vector2[] path = PathsManager.Instance.GetPath(shapeDescription.type, shapeDescription.target, shapeDescription.goRight);
            Color color = GetShapeColor(shapeDescription.target);
            float speed = shapesSpeed * shapeDescription.type switch // speedAdjustment according to shape
            {
                ShapeType.Square =>  squareSpeedAdjustment,
                ShapeType.Circle => circleSpeedAdjustment,
                _ => 1
            };
            float timeToSpawn = GetShapeTimeToSpawn(shapeDescription.type, shapeDescription.timeToPress, speed);
            
            if(timeToSpawn < 0)
                continue;
            
            GameModel.Instance.PushShapeModel(
                new ShapeModel(shapeDescription.type, shapeDescription.target, color, path, shapeDescription.timeToPress, timeToSpawn, speed)
            );
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

    private float GetShapeTimeToSpawn(ShapeType type, float timeToPress, float speed)
    {
        return timeToPress - PathsManager.Instance.GetPathTotalDistance(type) / speed;
    }
}
