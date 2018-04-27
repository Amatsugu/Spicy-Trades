using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ResourceIdentifier
{
	public string resource;
	public float count;
	public NeedType type;
	[HideInInspector]
	public ActiveEvent source;

	public bool Match(ResourceTileInfo resource)
	{
		if (string.IsNullOrEmpty(this.resource) && count == 0)
			return true;
		if (type == NeedType.Category) //Categoric Needs
		{
			return resource.category == (ResourceCategory)Enum.Parse(typeof(ResourceCategory), this.resource);
		}
		else if (type == NeedType.Tag) //Tagged
		{
			return resource.tags.Any(tag => tag == this.resource);
		}
		else //Resources
		{
			return resource.name == this.resource;
		}
	}
}

public enum NeedType
{
	Resource,
	Category,
	Tag,
	Money
}
