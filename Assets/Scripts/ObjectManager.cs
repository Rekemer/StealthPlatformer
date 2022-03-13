using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


    public class ObjectManager<T, U>  :MonoBehaviour where T: MonoBehaviour where U: MonoBehaviour,IHaveSize
    {
        public static List<U> objects { get; set; } = new List<U>();
        public List<U> zones = objects;
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var obj in objects)
            {
                Handles.DrawWireCube(obj.transform.position, obj.Size);
            }
        }
#endif
    }
