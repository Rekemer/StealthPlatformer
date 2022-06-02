using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Visualisation")]
public class VisualisationData : ScriptableObject
{
    [SerializeField] public  float viewDistance = 4;
    [SerializeField] public  float viewAngleInDegrees;
    [SerializeField] public  LayerMask obstacleMask;
    [SerializeField] public  LayerMask visibleTargetMask;
    [SerializeField] public  float meshResolution;
    [SerializeField] public  int edgeResolveIterations;
    [SerializeField] public Material materialOfVisualField;
}
