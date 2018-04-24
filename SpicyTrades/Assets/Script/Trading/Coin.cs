using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public struct Coin
{
	[JsonIgnore]
	public int Chip { get { return (int)((Value - (Royal * silverPerGold) - Silver) * chipPerSilver); } }
	[JsonIgnore]
	public int Silver { get { return (int)(Value - (Royal * silverPerGold)); } }
	[JsonIgnore]
	public int Royal { get { return (int)(Value / silverPerGold); } }

	public float Value { get; private set; }

	private const float chipPerSilver = 50;
	private const float silverPerGold = 10;

	public Coin(int chip = 0, int silver = 0, int gold = 0)
	{
		if(chip >= chipPerSilver)
		{
			silver += (int)(chip / chipPerSilver);
			chip = (int)(chip % chipPerSilver);
		}
		if(silver >= silverPerGold)
		{
			gold += (int)(silver / silverPerGold);
			silver = (int)(silver % silverPerGold);
		}
		Value = gold * silverPerGold;
		Value += silver;
		Value += chip / chipPerSilver;
	}

	public Coin(float value)
	{
		Value = value;
	}

	public static Coin operator +(Coin a, Coin b)
	{
		return new Coin(a.Value + b.Value);
	}
	public static Coin operator +(Coin a, float b)
	{
		return new Coin(a.Value + b);
	}
	public static Coin operator +(float a, Coin b)
	{
		return new Coin(a + b.Value);
	}

	public static Coin operator -(Coin a, Coin b)
	{
		return new Coin(a.Value - b.Value);
	}

	public static Coin operator -(Coin a, float b)
	{
		return new Coin(a.Value - b);
	}
	public static Coin operator -(float a, Coin b)
	{
		return new Coin(a - b.Value);
	}

	public static Coin operator *(Coin a, float b)
	{
		return new Coin(a.Value * b);
	}

	public static Coin operator *(float a, Coin b)
	{
		return b * a;
	}

	public static Coin operator /(Coin a, float b)
	{
		return a * (1 / b);
	}

	public static bool operator >(Coin a, Coin b)
	{
		return a.Value > b.Value;
	}
	public static bool operator >(float a, Coin b)
	{
		return a > b.Value;
	}

	public static bool operator >(Coin a, float b)
	{
		return a.Value > b;
	}

	public static bool operator >=(Coin a, Coin b)
	{
		return (a > b) || (a == b);
	}

	public static bool operator >=(float a, Coin b)
	{
		return (a > b) || (a == b);
	}

	public static bool operator >=(Coin a, float b)
	{
		return (a > b) || (a == b);
	}

	public static bool operator <(Coin a, Coin b)
	{
		return b > a;
	}

	public static bool operator <(float a, Coin b)
	{
		return a < b.Value;
	}

	public static bool operator <(Coin a, float b)
	{
		return a.Value < b;
	}

	public static bool operator <=(Coin a, Coin b)
	{
		return (a > b) || (a == b);
	}

	public static bool operator <=(float a, Coin b)
	{
		return (a > b) || (a == b);
	}

	public static bool operator <=(Coin a, float b)
	{
		return (a > b) || (a == b);
	}

	public static bool operator ==(Coin a, Coin b)
	{
		return a.Value == b.Value;
	}

	public static bool operator ==(Coin a, float b)
	{
		return a.Value == b;
	}

	public static bool operator ==(float a, Coin b)
	{
		return a == b.Value;
	}

	public static bool operator !=(Coin a, Coin b)
	{
		return !(a == b);
	}

	public static bool operator !=(Coin a, float b)
	{
		return !(a.Value == b);
	}

	public static bool operator !=(float a, Coin b)
	{
		return !(a == b.Value);
	}

	// override object.Equals
	public override bool Equals(object obj)
	{
		if (obj == null || typeof(Coin) != obj.GetType())
		{
			return false;
		}
		var c = (Coin)obj;
		return this == c;
	}

	// override object.GetHashCode
	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override string ToString()
	{
		return ToString("\n");
	}

	public string ToString(string separator)
	{
		string[] s = new string[3];
		if(Royal != 0)
			s[0] = "<color=#fcc100>" + Royal + "</color>";
		if(Silver != 0)
			s[1] = "<color=#cccccc>" + Silver + "</color>";
		if(Chip != 0)
			s[2] = "<color=#8c4800>" + Chip + "</color>";
		return string.Join(separator, s.Where(i => i != null).ToArray());
	}
}
