using UnityEngine;
using System.Collections;

public class ExplosionRemover : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(RemoveAfter(3.0f));
	}

    private IEnumerator RemoveAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(this.gameObject);
    }
}
