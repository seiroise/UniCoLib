using UnityEngine;
using System;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics.ChainLine {

	/// <summary>
	/// 線連結
	/// </summary>
	[Serializable]
	public class ChainLine {

		[SerializeField]
		private List<Vertex> vertices;                              //頂点リスト
		private IChainLineUpdater[] updaters;                       //更新機リスト
		private Dictionary<Type, IChainLineUpdater> updaterDic;     //更新機辞書
		private bool vertsZeroWithDeath = false;                    //頂点が無くなったら削除
		public bool VertsZeroWithDeath { get { return vertsZeroWithDeath; } set { vertsZeroWithDeath = value; } }

		#region Constructors

		public ChainLine(List<Vector3> vertices) : this(vertices, Color.white, null) { }

		public ChainLine(List<Vector3> vertices, Color color) : this(vertices, color, null) { }

		public ChainLine(List<Vector3> vertices, Color color, params IChainLineUpdater[] updaters) {
			//頂点リスト
			this.vertices = new List<Vertex>();
			if(vertices != null) {
				for(int i = 0; i < vertices.Count; ++i) this.vertices.Add(new Vertex(vertices[i], color));
			}
			//更新機
			this.updaterDic = new Dictionary<Type, IChainLineUpdater>();
			if(updaters != null) {
				this.updaters = updaters;
				foreach(var e in updaters) {
					this.updaterDic.Add(e.GetType(), e);
				}
				foreach(var e in updaters) {
					e.Init(this);
				}
			} else {
				this.updaters = new IChainLineUpdater[0];
			}
			//オプション
			this.vertsZeroWithDeath = false;
		}

		#endregion

		#region Function

		/// <summary>
		/// 更新
		/// </summary>
		public List<Vertex> Update() {
			//頂点の更新
			foreach(var e in updaters) {
				e.Update(vertices);
			}
			//経過時間の更新
			UpdateTime();
			//更新した頂点を返す
			return vertices;
		}

		/// <summary>
		/// 経過時間の更新
		/// </summary>
		private void UpdateTime() {
			for(int i = 0; i < vertices.Count; ++i) vertices[i].time += Time.deltaTime;
		}

		/// <summary>
		/// 更新クラスの取得
		/// </summary>
		public IChainLineUpdater GetUpdater<T>() where T : IChainLineUpdater {
			Type t = typeof(T);
			if(updaterDic.ContainsKey(t)) {
				return updaterDic[t];
			}
			return null;
		}

		/// <summary>
		/// 頂点の追加
		/// </summary>
		public void AddVertex(Vector3 position) {
			vertices.Add(new Vertex(position));
		}

		/// <summary>
		/// 頂点の追加
		/// </summary>
		public void AddVertex(Vector3 position, Color color) {
			vertices.Add(new Vertex(position, color));
		}

		/// <summary>
		/// 頂点の削除
		/// </summary>
		public void RemoveVertex(int index) {
			if(index < 0 || vertices.Count <= index) {
				throw new IndexOutOfRangeException();
			}
			vertices.RemoveAt(index);
		}

		#endregion
	}
}