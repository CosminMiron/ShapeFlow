using Michsky.MUIP;
using Seb.Fluid.Rendering;
using Seb.Fluid.Simulation;
using UnityEngine;
using UnityEngine.UI;

public class FluidSettingsUI : MonoBehaviour
{
    [SerializeField] private FluidSim _fluidSim;
    [SerializeField] private Transform _simObject;
    [SerializeField] private Toggle _particlesToggle;
    [SerializeField] private CustomToggle _trailsToggle;
    [SerializeField] private CustomToggle _flowFieldToggle;
    [SerializeField] private SliderManager _scaleSlider;
    [SerializeField] private SliderManager _velocitySlider;
    [SerializeField] private ParticleDisplay3D _particleDisplay3D;
    [SerializeField] private GameObject _flowTrails;
    [SerializeField] private GameObject _flowField;
    [SerializeField] private SliderManager _speedSlider;
    [SerializeField] private SliderManager _viscositySlider;
    [SerializeField] private SliderManager _densitySlider;
    [SerializeField] private SliderManager _pressureMultiplier;
    [SerializeField] private SliderManager _iterations;
    [SerializeField] private SliderManager _smoothing;
    [SerializeField] private SliderManager _rotation;
    [SerializeField] private SliderManager _width;
    [SerializeField] private SliderManager _height;
    [SerializeField] private SliderManager _position;

    private void Start()
    {
        _scaleSlider.mainSlider.value = _particleDisplay3D.scale;
        _velocitySlider.mainSlider.value = _particleDisplay3D.velocityDisplayMax;
        _trailsToggle.toggleObject.isOn = _flowTrails.activeSelf;
        _flowFieldToggle.toggleObject.isOn = _flowField.activeSelf;
        _speedSlider.mainSlider.value = _fluidSim.speed;
        _viscositySlider.mainSlider.value = _fluidSim.viscosityStrength;
        _densitySlider.mainSlider.value = _fluidSim.targetDensity;
        _pressureMultiplier.mainSlider.value = _fluidSim.pressureMultiplier;
        _iterations.mainSlider.value = _fluidSim.iterationsPerFrame;
        _smoothing.mainSlider.value = _fluidSim.smoothingRadius;
        _rotation.mainSlider.value = _simObject.rotation.y;
        _width.mainSlider.value = _fluidSim.transform.localScale.x;
        _height.mainSlider.value = _fluidSim.transform.localScale.y;
        _position.mainSlider.value = _fluidSim.transform.position.x;
        _trailsToggle.UpdateState();
        _flowFieldToggle.UpdateState();
        _particlesToggle.onValueChanged.AddListener(ChangeParticleState);
        _scaleSlider.sliderEvent.AddListener(ChangeParticleScale);
        _velocitySlider.sliderEvent.AddListener(ChangeVelocityMax);
        _trailsToggle.toggleObject.onValueChanged.AddListener(ChangeTrailsState);
        _flowFieldToggle.toggleObject.onValueChanged.AddListener(ChangeFlowFieldState);
        _speedSlider.sliderEvent.AddListener(ChangeFluidSpeed);
        _viscositySlider.sliderEvent.AddListener(ChangeFluidVIscosity);
        _densitySlider.sliderEvent.AddListener(ChangeFluidDensity);
        _pressureMultiplier.sliderEvent.AddListener(ChangePressure);
        _iterations.sliderEvent.AddListener(ChangeIterations);
        _smoothing.sliderEvent.AddListener(ChangeSmoothing);
        _rotation.sliderEvent.AddListener(ChangeRotation);
        _width.sliderEvent.AddListener(ChangeWidth);
        _height.sliderEvent.AddListener(ChangeHeight);
        _position.sliderEvent.AddListener(ChangePos);
    }

    private void OnDestroy()
    {
        _particlesToggle.onValueChanged.RemoveListener(ChangeParticleState);
        _scaleSlider.sliderEvent.RemoveListener(ChangeParticleScale);
        _velocitySlider.sliderEvent.RemoveListener(ChangeVelocityMax);
        _trailsToggle.toggleObject.onValueChanged.RemoveListener(ChangeTrailsState);
        _flowFieldToggle.toggleObject.onValueChanged.RemoveListener(ChangeFlowFieldState);
        _speedSlider.sliderEvent.RemoveListener(ChangeFluidSpeed);
        _viscositySlider.sliderEvent.RemoveListener(ChangeFluidVIscosity);
        _densitySlider.sliderEvent.RemoveListener(ChangeFluidDensity);
        _pressureMultiplier.sliderEvent.RemoveListener(ChangePressure);
        _iterations.sliderEvent.RemoveListener(ChangeIterations);
        _smoothing.sliderEvent.RemoveListener(ChangeSmoothing);
        _rotation.sliderEvent.RemoveListener(ChangeRotation);
        _width.sliderEvent.RemoveListener(ChangeWidth);
        _height.sliderEvent.RemoveListener(ChangeHeight);
        _position.sliderEvent.RemoveListener(ChangePos);
    }

    private void ChangeParticleState(bool state)
    {
        _particleDisplay3D.enabled = state;
    }

    private void ChangeParticleScale(float scale)
    {
        _particleDisplay3D.scale = scale;
    }

    private void ChangeVelocityMax(float velocity)
    {
        _particleDisplay3D.velocityDisplayMax = velocity;
    }

    private void ChangeTrailsState(bool state)
    {
        _flowTrails.SetActive(state);
    }

    private void ChangeFlowFieldState(bool state)
    {
        _flowField.SetActive(state);
    }

    private void ChangeFluidSpeed(float speed)
    {
        _fluidSim.speed = speed;
    }

    private void ChangeFluidVIscosity(float viscosity)
    {
        _fluidSim.viscosityStrength = viscosity;
    }

    private void ChangeFluidDensity(float density)
    {
        _fluidSim.targetDensity = density;
    }

    private void ChangePressure(float pressure)
    {
        _fluidSim.pressureMultiplier = pressure;
    }

    private void ChangeIterations(float iterations)
    {
        _fluidSim.iterationsPerFrame = (int)iterations;
    }

    private void ChangeSmoothing(float smoothing)
    {
        _fluidSim.smoothingRadius = smoothing;
    }

    private void ChangeRotation(float roatation)
    {
        _simObject.eulerAngles = new Vector3(0, roatation, 0);
    }

    private void ChangeWidth(float width)
    {
        _fluidSim.transform.localScale = new Vector3(width, _fluidSim.transform.localScale.y, _fluidSim.transform.localScale.z);
    }

    private void ChangeHeight(float height)
    {
        _fluidSim.transform.localScale = new Vector3(_fluidSim.transform.localScale.y, height, _fluidSim.transform.localScale.z);
    }

    private void ChangePos(float pos)
    {
        _fluidSim.transform.position = new Vector3(pos, _fluidSim.transform.position.y, _fluidSim.transform.position.z);
    }
}
