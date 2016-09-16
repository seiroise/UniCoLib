using UnityEngine;
using System;

namespace Seiro.Scripts.Graphics {

	/// <summary>
	/// 簡易メッシュ
	/// </summary>
	public class EasyMesh {

		public Vector3[] verts;
		public Color[] colors;
		public int[] indices;

		#region Constructor

		public EasyMesh() {
			verts = new Vector3[0];
			colors = new Color[0];
			indices = new int[0];
		}

		#endregion

		#region Function

		/// <summary>
		/// Unity上で使える形式に変換
		/// </summary>
		public Mesh ToMesh() {
			Mesh mesh = new Mesh();
			mesh.vertices = verts;
			mesh.colors = colors;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			return mesh;
		}

		#endregion

		#region Static Function

		/// <summary>
		/// 複数のEasyMeshを結合してMeshを作成
		/// </summary>
		public static Mesh ToMesh(EasyMesh[] eMeshes) {
			Mesh mesh = new Mesh();
			mesh.name = "Connected Mesh";
			//カウント
			int vertCount = 0;
			int colorCount = 0;
			int indexCount = 0;
			foreach(EasyMesh e in eMeshes) {
				vertCount += e.verts.Length;
				colorCount += e.colors.Length;
				indexCount += e.indices.Length;
			}
			//メッシュの作成
			Vector3[] vertices = new Vector3[vertCount];
			Color[] colors = new Color[colorCount];
			int[] indices = new int[indexCount];
			int vc = 0, cc = 0, ic = 0;
			foreach(EasyMesh e in eMeshes) {
				if(e == null) continue;
				//verts
				for(int i = 0; i < e.verts.Length; ++i) {
					vertices[vc + i] = e.verts[i];
				}
				//colors
				for(int i = 0; i < e.colors.Length; ++i) {
					colors[cc + i] = e.colors[i];
				}
				//indices
				for(int i = 0; i < e.indices.Length; ++i) {
					indices[ic + i] = e.indices[i] + vc;
				}
				//カウント
				cc += e.colors.Length;
				vc += e.verts.Length;
				ic += e.indices.Length;
			}
			//uv
			Vector2[] uvs = new Vector2[vertices.Length];
			for(int i = 0; i < vertices.Length; ++i) {
				uvs[i] = Vector2.one;
			}
			//メッシュの登録
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.colors = colors;
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
			mesh.RecalculateNormals();
			return mesh;
		}

		#endregion
	}
}