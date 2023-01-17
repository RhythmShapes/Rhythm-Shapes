using UnityEngine;

namespace edition
{
    public class TestLine : MonoBehaviour
    {
        public void Init(float posX)
        {
            UpdatePosX(posX);
        }

        public void UpdatePosX(float posX)
        {
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(posX, pos.y, pos.z);
        }
    }
}