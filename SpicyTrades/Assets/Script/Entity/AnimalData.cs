using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalData : MonoBehaviour
{
	public GameObject GameObject { get; set; }
	public Transform Transform { get; set; }
	public SpriteRenderer SpriteRenderer { get; set; }
	public float NextActionTime { get; set; }
	public float Speed { get; set; }

	private bool _isWalking = false;
	private Coroutine _runningRoutine;

	public void WalkTo(Vector3 destination)
	{
		if(_isWalking)
			StopCoroutine(_runningRoutine);
		_isWalking = true;
		_runningRoutine = StartCoroutine(Walk(destination));
	}

	private IEnumerator Walk(Vector3 destination)
	{
		var t = 0f;
		while (t < 1)
		{
			Transform.localPosition = Vector3.Lerp(Transform.localPosition, destination, t);
			t += Time.deltaTime * Speed;
			yield return new WaitForEndOfFrame();
		}
		_isWalking = false;
	}
}
