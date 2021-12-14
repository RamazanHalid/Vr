using UnityEngine;
using System.Collections;

public class WindController : MonoBehaviour
{
    public float windStrength;
    public float windHeading;
    private Vector3 wind;
    private Vector3 windDirection;

    void Start()
    {

        windDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * windHeading), 0, Mathf.Cos(Mathf.Deg2Rad * windHeading));

        //Yalnızca vektör yönünü kullanın ve m/s cinsinden rüzgar kuvvetiyle çarpın.
        wind = windDirection.normalized * windStrength;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F2))
        {
            windStrength += 0.5f;
            wind = windDirection.normalized * windStrength;
        }
        else if (Input.GetKeyUp(KeyCode.F1) && windStrength > 0)
        {
            windStrength -= 0.5f;
            wind = windDirection.normalized * windStrength;
        }
    }

    void FixedUpdate()
    {
       
        foreach (IFlyingObjects obj in ConstantValues.flyables)
        { 

            obj.applyWind(GetWindAtPos(obj.GetWorldPosition()));
           
        }
    }

    public Vector3 GetVelocity(IFlyingObjects obj)
    {
        
        return GetWindAtPos(obj.GetWorldPosition());
    }


    public float HeightToGround(Vector3 pos)
    {
        return pos.y - Terrain.activeTerrain.SampleHeight(pos);
    }

    private Vector3 GetWindAtPos(Vector3 pos)
    {
        float maxSoarAltitude = 80 * windStrength;

        float heightFactor = 1 - (HeightToGround(pos) / maxSoarAltitude);
        float upProjection = (GetTerrainGradient(pos) * windStrength * heightFactor).y;
        Vector3 normalWindDir = (upProjection * Vector3.up + wind).normalized;

       
        return normalWindDir * windStrength;
    }

    private Vector3 GetTerrainGradient(Vector3 worldPos)
    {

       
        Vector3 deltaWindDir = wind.normalized * 0.1f;

        Vector3 hPos = new Vector3(worldPos.x + deltaWindDir.x, worldPos.y, worldPos.z + deltaWindDir.z);

        Vector3 currentGroundPos = new Vector3(worldPos.x, Terrain.activeTerrain.SampleHeight(worldPos), worldPos.z);

        Vector3 deltaGroundPos = new Vector3(hPos.x, Terrain.activeTerrain.SampleHeight(hPos), hPos.z);

        return ((deltaGroundPos - currentGroundPos).normalized);
    }
}
