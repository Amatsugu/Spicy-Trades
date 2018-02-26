using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuminousVector;

[CreateAssetMenu(menuName = "Map Generator/Random")]
public class RandomGenerator : MapGenerator {

	[Range(10f, 1000f)]
	public float resolution = 512;
	public float frequency = 1f;
	[Range(1, 8)]
	public int octaves = 1;
	[Range(1f, 4f)]
	public float lacunarity = 2f;
	[Range(0f, 1f)]
	public float persistence = .5f;
	[Range(0f, 1f)]
	public float strength = 1;
	public bool damping;
	public NoiseMethodType type;
	[Range(1, 3)]
	public int demensions = 3;
	public Vector3 position;
	public Vector3 rotation;
	public bool coloredStrength;


	private float _stepSize
	{
		get
		{
			return 1f / resolution;
		}
	}

	private NoiseMethod _method
	{
		get
		{
			return Noise.noiseMethods[(int)type][demensions - 1];
		}
	}
	public override Tile Generate(int x, int y, Transform parent = null)
	{
		var q = Quaternion.Euler(rotation);
		Vector3 p00 = q * (new Vector3(-.5f, -.5f) + position);
		Vector3 p10 = q * (new Vector3(.5f, -.5f) + position);
		Vector3 p01 = q * (new Vector3(-.5f, .5f) + position);
		Vector3 p11 = q * (new Vector3(.5f, .5f) + position);
		Vector3 p0 = Vector3.Lerp(p00, p01, (y + .5f) * _stepSize);
		Vector3 p1 = Vector3.Lerp(p10, p11, (y + .5f) * _stepSize);
		Vector3 p = Vector3.Lerp(p0, p1, (x + .5f) * _stepSize);
		float amplitude = damping ? strength / frequency : strength;
		float sample = Noise.Sum(_method, p, frequency, octaves, lacunarity, persistence);
		if (type != NoiseMethodType.Value)
			sample = sample * .5f + .5f;
		Color col;
		if (coloredStrength)
		{
			sample *= amplitude;
			col = tileMapper.GetColor(sample);
		}
		else
		{
			col = tileMapper.GetColor(sample);
			sample *= amplitude;
		}
		return CreateTile(tileMapper.GetTile(sample), x, y, parent, col);//.SetWeight(tileMapper.GetMoveCost(sample));
	}
}
