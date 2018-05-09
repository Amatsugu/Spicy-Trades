using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Audio/Audio Provider")]
public class AudioProvider : ScriptableObject
{
	public AudioClip[] audioClips;
	public float minPitch = 1;
	public float maxPitch = 1;
	public static System.Random random;
	private AudioSource previewSource;

	public void OnEnable()
	{
		if (random == null)
			random = new System.Random();
		if(!Application.isPlaying)
		{
			if(previewSource != null)
			{
				var g = new GameObject();
				previewSource = g.AddComponent<AudioSource>();
			}
		}
	}

	public void OnDisable()
	{
		if (previewSource != null)
			Destroy(previewSource.gameObject);
	}

	public void OnDestroy()
	{
		if (previewSource != null)
			Destroy(previewSource.gameObject);
	}

	public void Play(AudioSource source)
	{
		Debug.Log($"Play: {name}");
		source.pitch = (float)MathUtils.Map(random.NextDouble(), 0, 1, minPitch, maxPitch);
		source.PlayOneShot(audioClips[random.Next(audioClips.Length)]);
	}
}