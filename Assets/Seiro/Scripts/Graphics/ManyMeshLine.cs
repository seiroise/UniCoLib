using UnityEngine;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics {

	/// <summary>
	/// とりあえず仮
	/// </summary>
	public class ManyMeshLine : MonoBehaviour {

		/// <summary>
		/// 線連結
		/// </summary>
		public class LineChain {

			private List<Vector3> vertices;				//頂点リスト
			private List<LineChainUpdater> updaters;	//更新機リスト

			#region Constructors

			public LineChain(List<Vector3> vertices) {
				this.vertices = vertices;
			}

			#endregion

			#region Function

			/// <summary>
			/// 更新
			/// </summary>
			public List<Vector3> Update() {
				//頂点の更新
				for(int i = 0; i < updaters.Count; ++i) {
					updaters[i].Update(vertices);
				}
				//更新した頂点を返す
				return vertices;
			}

			#endregion
		}

		/// <summary>
		/// 線連結の更新機
		/// </summary>
		public class LineChainUpdater {
			public void Update(List<Vector3> vertices) {
				
			}
		}
	}
}