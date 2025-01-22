using Unity.Mathematics;
using UnityEngine;

public class FluidSim : MonoBehaviour
{
    FluidCube fluidCube;
    void Start()
    {
        var obst = new float3[8];
        obst[0] = new float3(8, 8, 8);
        obst[1] = new float3(8, 9, 8);
        obst[2] = new float3(9, 8, 8);
        obst[3] = new float3(9, 9, 8);
        obst[4] = new float3(8, 8, 9);
        obst[5] = new float3(8, 9, 9);
        obst[6] = new float3(9, 8, 9);
        obst[7] = new float3(9, 9, 9);
        fluidCube = new FluidCube(16, 0, 0, 0.1f, obst);
    }

    void Update()
    {
        fluidCube.FluidCubeStep();

        for (int x = 0; x < fluidCube.size; x++)
        {
            for (int y = 0; y < fluidCube.size; y++)
            {
                fluidCube.AddVelocity(x, y, 1, 0, 0, 0.1f);
            }
        }
        //fluidCube.AddDensity(6, 6, 1, 100);

    }

    void OnDrawGizmos()
    {
        for (int x = 0; x < fluidCube.size; x++)
        {
            for (int y = 0; y < fluidCube.size; y++)
            {
                for (int z = 0; z < fluidCube.size; z++)
                {
                    var ix = fluidCube.IX(x, y, z);
                    var pos = new Vector3(x, y, z);
                    var ceva = new Vector3(fluidCube.Vx[ix], fluidCube.Vy[ix], fluidCube.Vz[ix]);
                    Gizmos.color = Color.white;
                    //Gizmos.DrawWireCube(pos, Vector3.one);

                    if (ceva == Vector3.zero) continue;
                    ceva += pos;

                    var dir = (ceva - pos).normalized;
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(pos, pos + dir);
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(pos + dir, Vector3.one * 0.1f);
                    //Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
