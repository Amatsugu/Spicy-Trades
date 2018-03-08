using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ResourceType/Basic")]
public class ResourceType : ScriptableObject
{
	public string ResourceName;
	public ResourceCategory category;
	public Sprite sprite; //TODO: Replace with renderer
	public int requiredWorkers;
	public float yeild;

}

public enum ResourceCategory
{
	Material,
	Food,
	Luxury,
	Fuel,
	Stategic
}
