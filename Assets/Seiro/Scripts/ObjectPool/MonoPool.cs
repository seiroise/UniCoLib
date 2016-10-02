using UnityEngine;
using System.Collections.Generic;

namespace Seiro.Scripts.ObjectPool {

	/// <summary>
	/// MonoBehaviourのプール
	/// </summary>
	public abstract class MonoPool<T> : MonoBehaviour where T : MonoBehaviour, IMonoPoolItem<T> {

		public T poolItem;                  //プールするアイテム
		[Range(0, 1024)]
		public int defaultPoolNum = 64;     //プールする数
		[Range(0, 1024)]
		public int addPoolNum = 16;         //不足時に追加する数
		private List<T> pool;               //プール
		private int popIndex;               //取り出しインデックス(巡回)

		#region UnityEvent

		private void Awake() {
			Initialize();
		}

		#endregion

		#region Function

		/// <summary>
		/// 初期化
		/// </summary>
		protected void Initialize() {
			pool = new List<T>();
			Add(defaultPoolNum);
		}

		/// <summary>
		/// 追加
		/// </summary>
		protected void Add(int num) {
			//生成して追加
			for(int i = 0; i < num; ++i) {
				T item = Instantiate<T>(poolItem.GetThis());
				item.gameObject.SetActive(false);
				item.transform.SetParent(transform);
				pool.Add(item);
			}
		}

		/// <summary>
		/// 取り出し
		/// </summary>
		protected T Pop() {
			//deactivatedなものを見つけるまでループ
			int count = pool.Count;
			for(int i = 0; i < count; ++i, ++popIndex) {
				popIndex %= count;
				if(!pool[popIndex].gameObject.activeInHierarchy) {
					return pool[popIndex];
				}
			}

			//一周探して見つからなかった場合は追加
			Add(addPoolNum);
			return pool[popIndex = count];
		}

		/// <summary>
		/// 取り出し
		/// </summary>
		protected List<T> Pop(int num) {
			List<T> items = new List<T>();
			for(int i = 0; i < num; ++i) {
				items.Add(Pop());
			}
			return items;
		}

		#endregion

		#region VirtualFunction

		/// <summary>
		/// アイテムを取り出す
		/// </summary>
		public T PopItem(Vector3 position) {
			T item = Pop();
			item.transform.position = position;
			item.Activate();
			return item;
		}

		/// <summary>
		/// 複数アイテムの取り出し
		/// </summary>
		public List<T> PopItems(int num, Vector3 position) {
			List<T> items = Pop(num);
			for(int i = 0; i < items.Count; ++i) {
				items[i].transform.position = position;
				items[i].Activate();
			}
			return items;
		}

		#endregion
	}
}