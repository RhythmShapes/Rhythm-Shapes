using System;
using System.Collections.Generic;
using UnityEngine;
using XML;
using Vector2 = UnityEngine.Vector2;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Targets")]
    [Space]
    [SerializeField] private Transform redTarget;
    [SerializeField] private Transform blueTarget;
    [SerializeField] private Transform greenTarget;
    [SerializeField] private Transform yellowTarget;

    [Space]
    [Header("Roads")]
    [Space]
    [SerializeField] private LineRenderer roadBR;
    [SerializeField] private LineRenderer roadRY;
    [SerializeField] private LineRenderer roadYG;
    [SerializeField] private LineRenderer roadGB;

    [Space]
    [Header("Shape Roads")]
    [Space]
    [SerializeField] private LineRenderer squareShapeRoad;
    [SerializeField] private LineRenderer circleShapeRoad;
    [SerializeField] private LineRenderer diamondShapeRoad;

    [Space]
    [Header("Cross points")]
    [Space]
    [SerializeField] private int squareCrossPoint;
    [SerializeField] private int circleCrossPoint;
    [SerializeField] private int diamondCrossPoint;
    
    [Space]
    [SerializeField] private TextAsset levelXML;

    private LevelDescription _level;
    
    // 8 paths : 0-1 = blue, 2-3 = red, 4-5 = green, 6-7 = yellow
    // even = right, odd = left
    private const int MaxPathPerShape = 8;
    private const int BluePath = 0;
    private const int RedPath = 2;
    private const int GreenPath = 4;
    private const int YellowPath = 6;
    private readonly Vector2[][] _squarePaths = new Vector2[MaxPathPerShape][];
    private readonly Vector2[][] _circlePaths = new Vector2[MaxPathPerShape][];
    private readonly Vector2[][] _diamondPaths = new Vector2[MaxPathPerShape][];

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;

        for (int i = 0; i < MaxPathPerShape; i += 2)
        {
            int color = i / 2;
            bool goRight = true;
            for (int j = i; j < i + 2; j++)
            {
                _squarePaths[j] = CalculatePath(ShapeType.Square, color, goRight);
                _circlePaths[j] = CalculatePath(ShapeType.Circle, color, goRight);
                _diamondPaths[j] = CalculatePath(ShapeType.Diamond, color, goRight);
                
                goRight = false;
            }
        }
    }

    private void Start()
    {
        _level = XmlHelpers.DeserializeDatabaseFromXML<LevelDescription>(levelXML).ToArray()[0];

        foreach (ShapeDescription shape in _level.shapes)
            shape.pathToFollow = GetPath(shape.type, shape.color, true);

        GameplayManager.Instance.Init(_level);
    }
    
    /**
     * Get path from calculated paths
     * Returns an array of Vector2 representing the path
     * type : ShapeType to define which shape road
     * target : Color to define which color to reach (blue, red, green, yellow)
     * goRight : true to take right road at spawn point, false to take left road
    */
    private Vector2[] GetPath(ShapeType type, Color target, bool goRight)
    {
        int targetIndex;
        int direction = goRight ? 1 : 2;

        if (CompareColor(target, Color.blue))
            targetIndex = BluePath;
        else if (CompareColor(target, Color.red))
            targetIndex = RedPath;
        else if (CompareColor(target, Color.green))
            targetIndex = GreenPath;
        else if (CompareColor(target, Color.yellow))
            targetIndex = YellowPath;
        else
        {
            Debug.LogError("Unknown color, using blue as default");
            targetIndex = BluePath;
        }

        switch (type)
        {
            case ShapeType.Square:
                return _squarePaths[targetIndex * direction];
            
            case ShapeType.Circle:
                return _circlePaths[targetIndex * direction];
            
            case ShapeType.Diamond:
                return _diamondPaths[targetIndex * direction];
            
            default:
                Debug.LogError("Unknown ShapeType, using Square as default");
                return _squarePaths[targetIndex * direction];
        }
    }

    private bool CompareColor(Color32 c1, Color32 c2)
    {
        return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
    }

    /**
     * Calculate path from spawn point to color
     * Returns an array of Vector2 representing the path
     * type : ShapeType to define which shape road
     * target : int to define which color to reach (0 = blue, 1 = red, 2 = green, 3 = yellow)
     * goRight : true to take right road at spawn point, false to take left road
    */
    private Vector2[] CalculatePath(ShapeType type, int target, bool goRight)
    {
        List<Vector2> path = new List<Vector2>();
        LineRenderer road;
        LineRenderer shapeRoad;
        Transform targetPosition;
        int crossPoint;

        switch (type)
        {
            case ShapeType.Square:
                shapeRoad = squareShapeRoad;
                crossPoint = squareCrossPoint;
                break;
            
            case ShapeType.Circle:
                //shapeRoad = circleShapeRoad;
                // TODO : circle road not working cause positions not perfectly going through targets
                shapeRoad = squareShapeRoad;
                crossPoint = circleCrossPoint;
                break;
            
            case ShapeType.Diamond:
                shapeRoad = diamondShapeRoad;
                crossPoint = diamondCrossPoint;
                break;
            
            default:
                Debug.LogError("Unknown ShapeType, using Square as default");
                shapeRoad = squareShapeRoad;
                crossPoint = 2;
                break;
        }

        switch (target)
        {
            case 0:
                road = goRight ? roadBR : roadGB;
                targetPosition = blueTarget;
                break;
            
            case 1:
                road = goRight ? roadRY : roadBR;
                targetPosition = redTarget;
                break;
            
            case 2:
                road = goRight ? roadGB : roadYG;
                targetPosition = greenTarget;
                break;
            
            case 3:
                road = goRight ? roadYG : roadRY;
                targetPosition = yellowTarget;
                break;
            
            default:
                Debug.LogError("Unknown color, using blue as default");
                road = goRight ? roadYG : roadRY;
                targetPosition = yellowTarget;
                break;
        }

        // From spawn point to cross point with shape road
        Vector3[] roadPositions = new Vector3[road.positionCount];
        road.GetPositions(roadPositions);

        for (int i = 0; i <= crossPoint && i < roadPositions.Length; i++)
            path.Add(roadPositions[i]);
        
        // From cross point to color
        Vector3[] shapeRoadPositions = new Vector3[shapeRoad.positionCount];
        shapeRoad.GetPositions(shapeRoadPositions);

        int crossPointIndex = 0;
        int addDirection = goRight ? -1 : 1;
        for (; crossPointIndex < shapeRoadPositions.Length; crossPointIndex++)
        {
            if (((Vector2)shapeRoadPositions[crossPointIndex]).Equals(roadPositions[crossPoint]))
            {
                crossPointIndex += addDirection;
                break;
            }

            if (crossPointIndex > 0 && 
                IsBetween(roadPositions[crossPoint], shapeRoadPositions[crossPointIndex - 1], shapeRoadPositions[crossPointIndex]))
            {
                if(goRight)
                    crossPointIndex--;
                break;
            }
        }

        for (int i = crossPointIndex; ; i += addDirection)
        {
            if (i < 0)
                i = shapeRoadPositions.Length - 1;
            else if (i >= shapeRoadPositions.Length)
                i = 0;
            
            path.Add(shapeRoadPositions[i]);
            
            if(((Vector2) shapeRoadPositions[i]).Equals(targetPosition.position))
                break;
        }

        return path.ToArray();
    }
    
    private bool IsBetween(Vector2 value, Vector2 bound1, Vector2 bound2)
    {
        return (value.x >= Math.Min(bound1.x, bound2.x) && value.x <= Math.Max(bound1.x ,bound2.x)) &&
               (value.y >= Math.Min(bound1.y, bound2.y) && value.y <= Math.Max(bound1.y ,bound2.y));
    }
}