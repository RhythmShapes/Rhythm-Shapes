using UnityEngine;

namespace edition
{
    public class ZoomVariables : MonoBehaviour
    {
        public bool scaleY = true;
        public float zoomForce = .1f;
        public Vector2 minZoom = new(.01f, .45f);
        public Vector2 maxZoom = new(5f, 5f);
    }
}