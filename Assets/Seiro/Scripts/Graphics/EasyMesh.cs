using UnityEngine;
using System;
using System.Collections.Generic;

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

		public EasyMesh(Vector3[] verts, Color[] colors, int[] indices) {
			this.verts = verts;
			this.colors = colors;
			this.indices = indices;
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
		/// 座標リストから二次元ポリラインを作成
		/// </summary>
		public static EasyMesh MakePolyLine2D(List<Vector2> points, float width, Color color) {

			//下準備
			int size = points.Count;
			Vector3[] verts = new Vector3[size * 4];
			Color[] colors = new Color[verts.Length];
			int[] indices = new int[size * 6];

			float halfWidth = width * 0.5f;
			for(int i = 1; i < size; ++i) {
				Vector2 a = points[i - 1];
				Vector2 b = points[i];

				Vector2 vertical = Quaternion.AngleAxis(90f, Vector3.forward) * (a - b).normalized * halfWidth;

				int vIndex = i * 4;
				verts[vIndex + 0] = a + vertical;
				verts[vIndex + 1] = a - vertical;
				verts[vIndex + 2] = b - vertical;
				verts[vIndex + 3] = b + vertical;

				colors[vIndex + 0] = color;
				colors[vIndex + 1] = color;
				colors[vIndex + 2] = color;
				colors[vIndex + 3] = color;

				int iIndex = i * 6;
				indices[iIndex + 0] = vIndex + 0;
				indices[iIndex + 1] = vIndex + 3;
				indices[iIndex + 2] = vIndex + 1;
				indices[iIndex + 3] = vIndex + 1;
				indices[iIndex + 4] = vIndex + 3;
				indices[iIndex + 5] = vIndex + 2;
			}

			EasyMesh eMesh = new EasyMesh();
			eMesh.verts = verts;
			eMesh.colors = colors;
			eMesh.indices = indices;

			//頂点間
			return eMesh;
		}

		/// <summary>
		/// 複数のEasyMeshを結合してMeshを作成
		/// </summary>
		public static Mesh ToMesh(EasyMesh[] eMeshes) {
			Mesh mesh = new Mesh();
			mesh.Clear();
			mesh.name = "Connected Mesh";
			//カウント
			int vertCount = 0;
			int colorCount = 0;
			int indexCount = 0;
			foreach(EasyMesh e in eMeshes) {
				if(e == null) continue;
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
			mesh.RecalculateBounds();
			return mesh;
		}

		#endregion
	}
}