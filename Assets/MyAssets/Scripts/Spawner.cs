using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    private float[] timer = {0.0f, 0.0f};
    private float[] nextSpawnIn = {0.0f, 0.0f};

    public Transform[] spawnerTransform;
    public float[] rotateFor = {0, 0};

    private int noTargetsSpawned = 0;


    public GameObject carPrefab;

    public enum VehicleTag {Vehicle01, Vehicle02};
    public VehicleTag vehicleTag;
    private string selectedVehicleTag;

    private float spawnInMin = 4.0f;
    private float spawnInMax = 7.0f;
    private float targetProbability = 0.35f;
    private float truckProbability = 0.45f;

    private bool spawnerEnabled = false;

    public GameObject vehiclesRoot;

    

    public void EnableSpawner()
    {
        spawnerEnabled = true;

        // Other stuff
        ResetSpawner();
    }

    public void DisableSpawner()
    {
        spawnerEnabled = false;

        // Other stuff
        ResetSpawner();
    }

    public void ResetSpawner()
    {
        for (int i = 0; i < timer.Length; i++)
        {
            timer[i] = 0.0f;
            nextSpawnIn[i] = 0.0f;
        }
    }
    

	void Start () {

        // Set the appropriate tag
        if (vehicleTag == VehicleTag.Vehicle01)
        {
            selectedVehicleTag = "Vehicles01";
        }
        else if (vehicleTag == VehicleTag.Vehicle02)
        {
            selectedVehicleTag = "Vehicles02";
        }
	}
	
	
	void Update () {



        if (spawnerEnabled)
        {
            for (int i = 0; i < timer.Length; i++)
            {
                timer[i] += Time.deltaTime;

                if (timer[i] > nextSpawnIn[i])
                {
                    if (spawnerTransform[i].gameObject.GetComponent<SpawnerTrigger>().areaClear)
                    {

                        GameObject go = (GameObject)Instantiate(carPrefab, spawnerTransform[i].position, Quaternion.Euler(new Vector3(0, rotateFor[i], 0)));
                        go.tag = selectedVehicleTag;
                        // Randomly create trucks too
                        if (Random.Range(0.0f, 1.0f) < truckProbability)
                        {
                            go.GetComponent<VehicleController>().vehicleType = VehicleController.VehicleType.Truck;
                        }

                        // Randomly tag a vehicle as a target
                        if (Random.Range(0.0f, 1.0f) < (targetProbability + 0.05f * noTargetsSpawned))
                        {
                            go.GetComponent<VehicleController>().isTarget = true;

                            noTargetsSpawned = 0;
                        }
                        else
                        {
                            noTargetsSpawned += 1;
                        }

                        // All vehicles have the same parent for easier control
                        go.transform.parent = vehiclesRoot.transform;

                        timer[i] = 0.0f;


                        nextSpawnIn[i] = Random.Range(spawnInMin, spawnInMax);
                    }
                }
            }
        }

	}

    public void OnTriggerEnter(Collider col)
    {
      //  Debug.Log("Something's in the spawner");
    }
}
