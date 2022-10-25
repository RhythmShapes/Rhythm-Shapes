using System;
using System.Collections.Generic;
using shape;
using UnityEngine;

public class PathsManager : MonoBehaviour
{
    public static PathsManager Instance { get; private set; }

    [Header("Targets")]
    [Space]
    [SerializeField] private Transform topTarget;
    [SerializeField] private Transform rightTarget;
    [SerializeField] private Transform leftTarget;
    [SerializeField] private Transform bottomTarget;

    [Space]
    [Header("Roads")]
    [Space]
    [SerializeField] private LineRenderer roadTopRight;
    [SerializeField] private LineRenderer roadTopLeft;
    [SerializeField] private LineRenderer roadBottomRight;
    [SerializeField] private LineRenderer roadBottomLeft;

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

    // 8 paths : 0-1 = top, 2-3 = right, 4-5 = left, 6-7 = bottom
    // even = right, odd = left
    private const int MaxPathPerShape = 8;
    private const int TopPath = 0;
    private const int RightPath = 2;
    private const int LeftPath = 4;
    private const int BottomPath = 6;
    private readonly Vector2[][] _squarePaths = new Vector2[MaxPathPerShape][];
    private readonly Vector2[][] _circlePaths = new Vector2[MaxPathPerShape][];
    private readonly Vector2[][] _diamondPaths = new Vector2[MaxPathPerShape][];

    private void Awake()
    {
        Debug.Assert(Instance == null);
        Instance = this;

        Target[] targets = { Target.Top, Target.Right, Target.Left, Target.Bottom };
        for (int i = 0; i < MaxPathPerShape; i += 2)
        {
            Target target = targets[i / 2];
            bool goRight = true;
            for (int j = i; j < i + 2; j++)
            {
                _squarePaths[j] = CalculatePath(ShapeType.Square, target, goRight);
                _circlePaths[j] = CalculatePath(ShapeType.Circle, target, goRight);
                _diamondPaths[j] = CalculatePath(ShapeType.Diamond, target, goRight);
                
                goRight = false;
            }
        }
    }
    
    /**
     * Get path from calculated paths
     * Returns an array of Vector2 representing the path
     * type : ShapeType to define which shape road
     * target : Target to define which target to reach
     * goRight : true to take right road at spawn point, false to take left road
    */
    public Vector2[] GetPath(ShapeType type, Target target, bool goRight)
    {
        int targetIndex;
        int direction = goRight ? 0 : 1;
        
        switch (target)
        {
            case Target.Top:
                targetIndex = TopPath;
                break;
            
            case Target.Bottom:
                targetIndex = BottomPath;
                break;
            
            case Target.Right:
                targetIndex = RightPath;
                break;
            
            case Target.Left:
                targetIndex = LeftPath;
                break;
            
            default:
                Debug.LogError("Unknown Target, using Top as default");
                targetIndex = TopPath;
                break;
        }

        switch (type)
        {
            case ShapeType.Square:
                return _squarePaths[targetIndex + direction];
            
            case ShapeType.Circle:
                return _circlePaths[targetIndex + direction];
            
            case ShapeType.Diamond:
                return _diamondPaths[targetIndex + direction];
            
            default:
                Debug.LogError("Unknown ShapeType, using Square as default");
                return _squarePaths[targetIndex + direction];
        }
    }

    /**
     * Calculate path from spawn point to color
     * Returns an array of Vector2 representing the path
     * type : ShapeType to define which shape road
     * target : Target to define which target to reach
     * goRight : true to take right road at spawn point, false to take left road
    */
    private Vector2[] CalculatePath(ShapeType type, Target target, bool goRight)
    {
        List<Vector2> path = new List<Vector2>();
        LineRenderer road;
        LineRenderer shapeRoad;
        Vector3 targetPosition;
        int crossPoint;

        switch (type)
        {
            case ShapeType.Square:
                shapeRoad = squareShapeRoad;
                crossPoint = squareCrossPoint;
                break;
            
            case ShapeType.Circle:
                shapeRoad = circleShapeRoad;
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
            case Target.Top:
                road = goRight ? roadTopRight : roadTopLeft;
                targetPosition = topTarget.position;
                break;
            
            case Target.Bottom:
                road = goRight ? roadBottomLeft : roadBottomRight;
                targetPosition = bottomTarget.position;
                break;
            
            case Target.Right:
                road = goRight ? roadBottomRight : roadTopRight;
                targetPosition = rightTarget.position;
                break;
            
            case Target.Left:
                road = goRight ? roadTopLeft : roadBottomLeft;
                targetPosition = leftTarget.position;
                break;
            
            default:
                Debug.LogError("Unknown Target, using Top as default");
                road = goRight ? roadTopRight : roadTopLeft;
                targetPosition = topTarget.position;
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
        int addDirection = 1;
        for (int pointA = 0, pointB = 1; pointA <= shapeRoadPositions.Length; pointA++, pointB++)
        {
            if (pointB >= shapeRoadPositions.Length)
                pointB = 0;
            
            Vector2 roadPointPositionA = shapeRoadPositions[pointA];
            Vector2 roadPointPositionB = shapeRoadPositions[pointB];
            
            if (IsBetween(roadPositions[crossPoint], roadPointPositionA, roadPointPositionB))
            {
                if (Vector2.Distance(roadPointPositionA, targetPosition) < 
                    Vector2.Distance(roadPointPositionB, targetPosition))
                {
                    addDirection = -1;
                    crossPointIndex = pointA;
                } else
                    crossPointIndex = pointB;
                
                if ((Vector2) shapeRoadPositions[crossPointIndex] == (Vector2) roadPositions[crossPoint])
                    crossPointIndex += addDirection;

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
            
            if((Vector2) shapeRoadPositions[i] == (Vector2) targetPosition)
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