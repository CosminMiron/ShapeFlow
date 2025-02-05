using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private CinemachineSplineDolly _cinemachineSplineDolly;

    public float CameraPosition => _cinemachineSplineDolly.CameraPosition;
    public float CameraFov => _cinemachineCamera.Lens.FieldOfView;

    public void SetFov(float fov)
    {
        _cinemachineCamera.Lens.FieldOfView = fov;
    }

    public void SetPosition(float position)
    {
        position = Mathf.Clamp(position, 0, 1);
        _cinemachineSplineDolly.CameraPosition = position;
    }
}
