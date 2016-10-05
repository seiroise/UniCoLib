using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 指定したメッシュ(2D)を描画
/// </summary>
[RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
public class MeshImage : Graphic {

	private Mesh mesh;

	public void SetMesh(Mesh mesh) {
		this.mesh = mesh;
		OnPopulateMesh(new VertexHelper());
	}

	protected override void OnPopulateMesh(VertexHelper vh) {
		if(mesh == null) return;
		vh = new VertexHelper(mesh);
		return;
		vh.Clear();
		Vector3[] vertices = mesh.vertices;
		Color[] colors = mesh.colors;

		//作成準備
		List<UIVertex> uiVerts = new List<UIVertex>();
		List<int> uiIndices = new List<int>(mesh.GetIndices(0));

		for(int i = 0; i < vertices.Length; ++i) {
			UIVertex uiVert = UIVertex.simpleVert;
			uiVert.position = vertices[i];
			uiVert.color = colors[i];
		}
		//作成
		vh.AddUIVertexStream(uiVerts, uiIndices);
	}
}