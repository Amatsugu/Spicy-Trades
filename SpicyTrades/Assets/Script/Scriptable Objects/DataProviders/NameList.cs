using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameList
{
	private List<string> _names;

	public NameList(string[] names)
	{
		_names = new List<string>();
		_names.AddRange(names);
	}

	public NameList(List<string> names)
	{
		_names = new List<string>();
		_names.AddRange(names);
	}

	public string GetNextName()
	{
		var i = Random.Range(0, _names.Count - 1);
		var name = _names[i];
		_names.RemoveAt(i);
		return name;
	}
	
}
