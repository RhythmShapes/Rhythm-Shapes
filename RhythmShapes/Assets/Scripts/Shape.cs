using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] private ShapeType _shapeType;
    [SerializeField] private Color _color;
    [SerializeField] private List<Vector2> _pathToFollow;
    [SerializeField] private float _speed;
    [SerializeField] private float _timeToPress;
    private SpriteRenderer _spriteRenderer;

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

    public List<Vector2> PathToFollow
    {
        get => _pathToFollow;
        set => _pathToFollow = value;
    }

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public float TimeToPress
    {
        get => _timeToPress;
        set => _timeToPress = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _color = _spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColor();
    }

    protected void UpdateColor()
    {
        if (_color != _spriteRenderer.color)
        {
            _spriteRenderer.color = _color;
        }
    }
}