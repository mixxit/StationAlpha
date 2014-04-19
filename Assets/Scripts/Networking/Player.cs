using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public Transform spawnPoint;

	void Start ()
	{
		Debug.Log("Start called.");
		this.spawnPoint = this.gameObject.transform;
	}


}
