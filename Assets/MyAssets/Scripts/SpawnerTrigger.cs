using UnityEngine;
using System.Collections;

public class SpawnerTrigger : MonoBehaviour {


    public bool areaClear = true;
    private int containedColliders = 0;

    public void OnTriggerEnter(Collider col)
    {
        containedColliders += 1;
    }

    public void OnTriggerExit(Collider col)
    {
        containedColliders -= 1;
        if (containedColliders == 0)
        {
            areaClear = true;
        }
        else
        {
            areaClear = false;
        }
    }
}
