using CFD.GAS;
using Michsky.MUIP;
using UnityEngine;

public class ShapeImproverUI : MonoBehaviour
{
    [SerializeField] private SliderManager _population;
    [SerializeField] private SliderManager _actions;
    [SerializeField] private SliderManager _mutations;
    [SerializeField] private SliderManager _generations;
    [SerializeField] private ButtonManager _start;
    [SerializeField] private GeneticAlgorithm _geneticAlgorithm;

    private void Start()
    {
        _start.onClick.AddListener(StartAlg);
    }

    private void OnDestroy()
    {
        _start.onClick.RemoveListener(StartAlg);

    }

    private void StartAlg()
    {
        _geneticAlgorithm.StartGA((int)_population.mainSlider.value, (int)_actions.mainSlider.value, _mutations.mainSlider.value, (int)_generations.mainSlider.value);
    }
}
