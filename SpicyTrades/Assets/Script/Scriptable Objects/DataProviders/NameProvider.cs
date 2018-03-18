﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Providers/Name Provider")]
public class NameProvider : ScriptableObject
{
	public string[] names;

	public NameList GetNameList()
	{
		return new NameList(names);
	}
}
