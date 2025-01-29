using AwesomeCharts;
using UnityEngine;
using System.Linq;

public class SimController : MonoBehaviour
{
    [SerializeField] private VoxelizedMesh _voxelizedMesh;
    [SerializeField] private LineChart _lineChart;
    [SerializeField] private Color _fillColor = Defaults.CHART_BACKGROUND_COLOR;
    [SerializeField] private Texture _fillTexture;

    private AeroData _aeroData = new AeroData();

    [ContextMenu("Create")]
    private void CalculateForObject()
    {
        var data = _voxelizedMesh.Voxelize();

        _aeroData.CalculateData(data);

        var speedPoints = (from speed in _aeroData.SpeedTrapPoints select speed.ToString()).ToList();
        _lineChart.AxisConfig.HorizontalAxisConfig.ValueFormatterConfig.CustomValues = speedPoints;

        LineDataSet set = new LineDataSet();
        foreach (var speed in _aeroData.SpeedToDragForce)
        {
            set.AddEntry(new LineEntry(speed.Key, speed.Value));
        }
        set.LineColor = Color.red;
        set.LineThickness = 4;
        set.UseBezier = true;
        set.FillColor = _fillColor;
        set.FillTexture = _fillTexture;
        _lineChart.GetChartData().DataSets.Clear();
        _lineChart.GetChartData().DataSets.Add(set);
        _lineChart.SetDirty();
    }
}
