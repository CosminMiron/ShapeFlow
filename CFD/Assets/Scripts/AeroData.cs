using System.Collections.Generic;
using UnityEngine;

public class AeroData
{
    private List<float> _speedTrapPoints = new List<float> { 5, 10, 15, 20, 25, 30 };
    private Dictionary<float, float> _speedToDragForce = new Dictionary<float, float>();
    private float _dragCoefficient;
    private float _airDensity = 1.2f;
    private Vector3Int _forward = new Vector3Int(0, 0, 1);

    public List<float> SpeedTrapPoints { get => _speedTrapPoints; }
    public Dictionary<float, float> SpeedToDragForce { get => _speedToDragForce; }

    public void CalculateData(VoxelizedData voxelizedData)
    {
        foreach (var speed in _speedTrapPoints)
        {
            _speedToDragForce[speed] = voxelizedData.CalculateObjectDragForce(_forward, speed, _airDensity);
        }

        _dragCoefficient = voxelizedData.CalculateDragCoefficient(_forward, _speedTrapPoints[0], _airDensity);
    }
}
