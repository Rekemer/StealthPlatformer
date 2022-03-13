using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class Detector : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            //groundCheck.position = 
            Handles.color = Color.red;
            var radius = new Vector2(1.53f, 0.57f);
            Handles.DrawWireCube(transform.position, radius);
        }
#endif
    }
}