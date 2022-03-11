using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class TimeZoneManager : MonoBehaviour
{
    public static List<TimeZone> timeZones { get; set; } = new List<TimeZone>();
    public List<TimeZone> zones = timeZones;
    private void OnDrawGizmos()
    {
        foreach (var zone in timeZones)
        {
            Handles.DrawWireCube(zone.transform.position, zone.Size);
        }
    }
   
}
