using System;
using System.Collections;
using System.Collections.Generic;
using shape;
using UnityEngine;
using utils.XML;

public class ShapesSpawner : MonoBehaviour
{
    public static ShapesSpawner Instance { get; private set; }
    
    private Queue<ShapeDescription> _shapeDescriptionsQueue;
    private Queue<float> _spawnTimesQueue;
    private Queue<Color> _shapeColorsQueue;

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Init(Queue<ShapeDescription> shapeDescriptions, Queue<float> spwanTimes, Queue<Color> shapeColors)
    {
        _shapeDescriptionsQueue = shapeDescriptions;
        _spawnTimesQueue = spwanTimes;
        _shapeColorsQueue = shapeColors;
    }

    public void SpawnShapes(float globalTime, float deltaTime)
    {
        Shape shape;
        ShapeDescription nextSpawnableShape = _shapeDescriptionsQueue.Peek();
        float nextSpawnableShapeSpawnTime = _spawnTimesQueue.Peek();
        Color nextSpawnableShapeColor = _shapeColorsQueue.Peek();
        // Debug.Log(globalTime);

        if (nextSpawnableShapeSpawnTime > globalTime-deltaTime && nextSpawnableShapeSpawnTime < globalTime+deltaTime)
        {
            // Debug.Log("SpawnShapes -> globalTime : " + globalTime);
            shape = ShapeFactory.Instance.GetShape(nextSpawnableShape.type);
            shape.transform.position = Vector3.zero;
            shape.Init(nextSpawnableShape,nextSpawnableShapeColor);

            switch (nextSpawnableShape.target)
            {
                case Target.Top:
                    GameplayManager.Instance._topTargetQueue.Enqueue(shape);
                    break;
            
                case Target.Bottom:
                    GameplayManager.Instance._bottomTargetQueue.Enqueue(shape);
                    break;
            
                case Target.Right:
                    GameplayManager.Instance._rightTargetQueue.Enqueue(shape);
                    break;
            
                case Target.Left:
                    GameplayManager.Instance._leftTargetQueue.Enqueue(shape);
                    break;
            
                default:
                    Debug.LogError("Unknown Target, using Top as default");
                    GameplayManager.Instance._topTargetQueue.Enqueue(shape);
                    break;
            }

            _shapeDescriptionsQueue.Dequeue();
            _spawnTimesQueue.Dequeue();
            _shapeColorsQueue.Dequeue();
        }
    }
}
