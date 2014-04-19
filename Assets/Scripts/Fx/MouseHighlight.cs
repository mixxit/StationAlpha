using UnityEngine;
using System.Collections;

public class MouseHighlight : MonoBehaviour {

	// Use this for initialization
	void Start () {
		shader = this.renderer.material.shader;
		outline = Shader.Find("Mobile/Bumped Specular");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private Color startcolor;

	Shader outline;
	Shader shader;

	void OnMouseEnter ()
	{
		Debug.Log ("Outlining object");
		this.renderer.material.shader = outline;
	}
	void OnMouseExit()
	{
		this.renderer.material.shader = shader;
	}
}
