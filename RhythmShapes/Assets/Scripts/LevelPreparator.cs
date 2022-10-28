using System;
using System.Collections;
using System.Collections.Generic;
using shape;
using UnityEngine.Events;
using UnityEngine;
using utils.XML;

public class LevelPreparator : MonoBehaviour
{
    
    public UnityEvent<Queue<ShapeDescription>,Queue<float>,Queue<Color>> onLevelUnityEvent;
    
    private Queue<ShapeDescription> _shapeDescriptionsQueue;
    private Queue<float> _spawnTimesQueue;
    private Queue<Color> _shapeColorsQueue;
    private Queue<Queue<float>> _timeToTargetQueues;

    private void Awake()
    {
        _timeToTargetQueues = new Queue<Queue<float>>();
        _shapeDescriptionsQueue = new Queue<ShapeDescription>();
        _spawnTimesQueue = new Queue<float>();
        _shapeColorsQueue = new Queue<Color>();
        
        onLevelUnityEvent ??= new UnityEvent<Queue<ShapeDescription>,Queue<float>,Queue<Color>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Init(LevelDescription level)
    {
        Debug.Log("levelTitle : " + level.title);
        for (int i = 0; i < level.shapes.Length; i++)
        {
            ShapeDescription shapeDescription = level.shapes[i];
            
            // Debug.Log("iteration :" + i);
            // _spawnTimes.Enqueue(level.shapes[i].timeToPress);
            _shapeDescriptionsQueue.Enqueue(shapeDescription);
            _shapeColorsQueue.Enqueue(level.GetTargetColor(shapeDescription.target));
            
            float totalPathDistance = ComputeShapeElement(shapeDescription); 
            Debug.Log("shapeDescription.pathToFollow.Length : " + shapeDescription.pathToFollow.Length);
            float speedAdjustment;
            switch (shapeDescription.type) // speedAdjustment according to shape
            {
                case ShapeType.Square :
                    speedAdjustment = 14.48528f/8.485281f; 
                    shapeDescription.speed *= speedAdjustment;
                    break;
                case ShapeType.Circle :
                    speedAdjustment = 10.7225f/8.485281f;
                    shapeDescription.speed *= speedAdjustment;
                    break;
                case ShapeType.Diamond :
                    speedAdjustment = 1;
                    shapeDescription.speed *= speedAdjustment;
                    break;
                default:
                    Debug.LogError("Unknown Type, using Square as default");
                    speedAdjustment = 14.48528f/8.485281f;
                    shapeDescription.speed *= speedAdjustment;
                    break;
            }
            // Debug.Log("TotalPathDistance : " + totalPathDistance);
            // Debug.Log("SpawnTimes : " + (shapeDescription.timeToPress - totalPathDistance / shapeDescription.speed));
            _spawnTimesQueue.Enqueue(shapeDescription.timeToPress - totalPathDistance / shapeDescription.speed);
        }
        Debug.Log("help me please");
        
        onLevelUnityEvent.Invoke(_shapeDescriptionsQueue,_spawnTimesQueue,_shapeColorsQueue);
    }
    
    //  !!!!!!!! should stock the different distancies instead of calculating them !!!!!!!!!
    private float ComputeShapeElement(ShapeDescription shapeDescription)
    {
        float totalPathDistance = 0;
        Vector2 lastElement = Vector2.zero;
        Queue<float> timeTotargetQueue = new Queue<float>();
        
        foreach (var element in shapeDescription.pathToFollow)
        {
            float distanceI = Vector3.Distance(
                lastElement, 
                element);
            totalPathDistance +=
                distanceI;
            float timeI = distanceI / shapeDescription.speed;
            timeTotargetQueue.Enqueue(timeI);
            // Debug.Log("CalculatePathLength -> lastElement : "+lastElement+", element : " + element);
            lastElement = element;
        }

        _timeToTargetQueues.Enqueue(timeTotargetQueue);
        return totalPathDistance;
    }
}
