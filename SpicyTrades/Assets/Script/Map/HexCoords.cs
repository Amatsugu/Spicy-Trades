using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoords
{
	public int X { get; private set; }
	public int Y { get; private set; }
	public int Z
	{
		get
		{
			return -X - Y;
		}
	}

	public HexCoords(int x, int y)
	{
		X = x;
		Y = y;
	}

	public static HexCoords FromOffsetCoords(int x, int y)
	{
		return new HexCoords(x - y / 2, y);
	}

	public int OffsetX
	{
		get
		{
			return X + Y / 2;
		}
	}

	public int OffsetY
	{
		get
		{
			return Y;
		}
	}

	public static HexCoords FromPosition(Vector3 position)
	{
		float x = position.x / (MapRenderer.Instance.generator.InnerRadius * 2f);
		float z = -x;
		float offset = position.y / (MapRenderer.Instance.generator.InnerRadius * 3f);
		z -= offset;
		x -= offset;
		int iX = Mathf.RoundToInt(x);
		int iZ = Mathf.RoundToInt(z);
		int iY = Mathf.RoundToInt(-x -z);
		if (iX + iY + iZ != 0)
			Debug.LogWarning("Rounding error");
		return new HexCoords(iX, iY);
	}

	public int ToIndex()
	{
		return X + Y * (int)MapRenderer.Instance.generator.Size.x + Y / 2;
	}

	public override string ToString()
	{
		return "(" + X +", " + Y + ", " + Z + ")";
	}

	public static bool operator ==(HexCoords a, HexCoords b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(HexCoords a, HexCoords b)
	{
		return !a.Equals(b);
	}

	// override object.Equals
	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}

		var h = (HexCoords)obj;
		return (h.X == X && h.Y == Y);
	}

	// override object.GetHashCode
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
