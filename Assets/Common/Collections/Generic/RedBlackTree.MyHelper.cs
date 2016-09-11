using System;
using System.Collections.Generic;

namespace Common.Collections.Generic {

	public sealed partial class RedBlackTree<T> where T : IComparable<T> {
		public T MostLeft () {
			if (this.Root == null)
				throw new NullReferenceException ();

			RedBlackNode<T> current = this.Root;
			while (current.Left != null) {
				current = current.Left;
			}

			return current.Value;
		}

		public T MostRight () {
			if (this.Root == null)
				throw new NullReferenceException ();

			RedBlackNode<T> current = this.Root;
			while (current.Right != null) {
				current = current.Right;
			}

			return current.Value;
		}

		public void Higher (T item) {

		}
		public void Lower (T item) {

		}
	}
}
