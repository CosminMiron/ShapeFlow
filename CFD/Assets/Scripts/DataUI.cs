using Michsky.MUIP;
using UnityEngine;

public class DataUI : MonoBehaviour
{
    [SerializeField] private VoxelizedMesh _voxelizedMesh;
    [SerializeField] private SliderManager _dragCoefficient;
    [SerializeField] private SliderManager _frontalArea;
    [SerializeField] private SliderManager _dragForce;

    private void Start()
    {
        var data = _voxelizedMesh.Voxelize(100);
        _dragCoefficient.mainSlider.value = data.CalculateDragCoefficient(_voxelizedMesh.forward)/10f;
        _frontalArea.mainSlider.value = data.CalculateFrontalArea();
        _dragForce.mainSlider.value = data.CalculateObjectDragForce(_voxelizedMesh.forward, 10, 1.2f);
    }
}
