using System;
using UnityEngine;
using XML;

[RequireComponent(typeof(SpriteRenderer))]
[Serializable]
public class Shape : MonoBehaviour
{
    [SerializeField] private ShapeType type;
    public ShapeType Type { get; private set; }

    private SpriteRenderer _spriteRenderer;
    private Vector2[] _pathToFollow;
    private float _timeToPress;
    private float _speed;

    public void Init(ShapeDescription description)
    {
        _pathToFollow = description.pathToFollow;
        _spriteRenderer.color = description.color;
        _timeToPress = description.timeToPress;
        _speed = description.speed;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}