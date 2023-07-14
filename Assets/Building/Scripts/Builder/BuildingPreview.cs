
using UnityEngine;
using System.Collections;

public class BuildingPreview : MonoBehaviour {

	[SerializeField] private MeshRenderer _meshRenderer;
	[SerializeField] private SpriteRenderer _spriteRenderer;

	public void SetColor(Color color)
	{
		if(_meshRenderer)
		{
			_meshRenderer.material.color = color;
		}
	}

	bool CheckObjectLayer(GameObject obj)
	{
		if((1 << obj.layer) != 0)
		{
			return true;
		}

		return false;
	}
	
	void OnTriggerStay(Collider other)
	{
		if(CheckObjectLayer(other.gameObject))
		{
			Building.ResetStatus();
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(CheckObjectLayer(other.gameObject))
		{
			Building.ResetStatus();
		}      
	}
}
