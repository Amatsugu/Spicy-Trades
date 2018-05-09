using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileRenderer : ScriptableObject {

	public abstract void RenderInit(Tile tile);

	public abstract void PostRender(Tile tile, object renderData);

	public virtual void RenderUpdate(Tile tile, object renderData)
	{

	}

}
