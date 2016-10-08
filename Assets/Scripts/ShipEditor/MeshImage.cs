using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 指定したメッシュ(2D)を描画
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer), typeof(RectTransform))]
public class MeshImage : Graphic {

	public Mesh mesh;
	public bool drawCenter = true;		//中央に描画
	private CanvasRenderer renderer;

	private void OnEnable() {
		UpdateGeometry();
	}

	public void SetMesh(Mesh mesh) {
		this.mesh = mesh;
		UpdateGeometry();
	}

	protected override void UpdateGeometry() {
		base.UpdateGeometry();
		if(mesh == null) return;
		if(renderer == null) renderer = GetComponent<CanvasRenderer>();
		Vector3 offset = Vector3.zero;
		if(drawCenter) {
			Mesh temp = new Mesh();
			//オフセットの計算
			offset = -mesh.bounds.center;
			Vector3[] vertices = mesh.vertices;
			for(int i = 0; i < vertices.Length; ++i) {
				vertices[i] += offset;
			}
			//頂点などの設定
			temp.vertices = vertices;
			temp.uv = mesh.uv;
			temp.colors = mesh.colors;
			temp.SetIndices(mesh.GetIndices(0), mesh.GetTopology(0), 0);
			//再計算
			temp.RecalculateBounds();
			temp.RecalculateNormals();
			renderer.SetMesh(temp);
		} else {
			renderer.SetMesh(mesh);
		}
	}
}