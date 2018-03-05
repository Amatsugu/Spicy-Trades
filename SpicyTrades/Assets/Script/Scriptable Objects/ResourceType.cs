using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceType : ScriptableObject
{
	public string ResourceName;
	public ResourceCategory category;
	public Sprite sprite; //TODO: Replace with renderer


}

public enum ResourceCategory
{
	Material,
	Food,
	Luxury,
	Fuel,
	Stategic
}
