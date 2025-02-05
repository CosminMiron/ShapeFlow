using Michsky.MUIP;
using UnityEngine;

public class CameraUI : MonoBehaviour
{
    [SerializeField] private SliderManager _positionSlider;
    [SerializeField] private SliderManager _fovSlider;
    [SerializeField] private CameraController _cameraController;

    private void Awake()
    {
        _positionSlider.mainSlider.value = _cameraController.CameraPosition;
        _fovSlider.mainSlider.value = _cameraController.CameraFov;
        _positionSlider.sliderEvent.AddListener(SetPosition);
        _fovSlider.sliderEvent.AddListener(SetFov);
    }

    private void OnDestroy()
    {
        _positionSlider.sliderEvent.RemoveListener(SetPosition);
        _fovSlider.sliderEvent.RemoveListener(SetFov);
    }

    private void SetPosition(float pos)
    {
        _cameraController.SetPosition(pos);
    }

    private void SetFov(float fov)
    {
        _cameraController.SetFov(fov);
    }
}
