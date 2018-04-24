using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class ListProvider<T> : ScriptableObject
{
	public T[] items;

	public virtual T[] GetResourceList()
	{
		return items;
	}

}
