using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] private ShapeType _shapeType;
    [SerializeField] private Color _color;
    [SerializeField] private int _pathToFollow;
    [SerializeField] private float _speed;
    [SerializeField] private float _timeToSpawn;

    public ShapeType ShapeType
    {
        get => _shapeType;
        set => _shapeType = value;
    }

    public Color Color
    {
        get => _color;
        set => _color = value;
    }

    public int PathToFollow
    {
        get => _pathToFollow;
        set => _pathToFollow = value;
    }

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public float TimeToSpawn
    {
        get => _timeToSpawn;
        set => _timeToSpawn = value;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
