using Michsky.MUIP;
using Seb.Fluid.Simulation;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private Spawner3D _spawner3D;
    [SerializeField] private SliderManager _particles;
    [SerializeField] private SliderManager _density;

    private void Start()
    {
        _particles.mainSlider.value = _spawner3D.debug_num_particles;
        _density.mainSlider.value = _spawner3D.particleSpawnDensity;
    }
}
