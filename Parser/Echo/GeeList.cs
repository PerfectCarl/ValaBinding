//
// ValaList.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MonoDevelop.Ide.Gui;
using System.Collections;

namespace MonoDevelop.ValaBinding.Parser.Echo
{
	/// <summary>
	/// IList wrapper for (Gee|Vala).List
	/// </summary>
	public class GeeList: IList<IntPtr>
	{
		public GeeList (IntPtr instance)
		{
			this.instance = instance;
		}

		#region ICollection[System.IntPtr] implementation

		public void Add (IntPtr item)
		{
			gee_collection_add (instance, item);
		}


		public void Clear ()
		{
			gee_collection_clear (instance);
		}


		public bool Contains (IntPtr item)
		{
			return gee_collection_contains (instance, item);
		}


		public void CopyTo (IntPtr[] array, int arrayIndex)
		{
			if (Count < array.Length - arrayIndex)
				throw new ArgumentException ("Destination array too small", "array");
			for (int i = 0; i < Count; ++i)
				array [i + arrayIndex] = this [i];
		}


		public int Count {
			get { 
				return gee_collection_get_size (instance); 
			}
		}


		public bool IsReadOnly {
			get { return false; }
		}


		public bool Remove (IntPtr item)
		{
			return gee_collection_remove (instance, item);
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((IEnumerable<IntPtr>)this).GetEnumerator ();
		}

		#endregion

		#region IList[System.IntPtr] implementation

		public int IndexOf (IntPtr item)
		{
			return gee_list_index_of (instance, item);
		}


		public void Insert (int index, IntPtr item)
		{
			gee_list_insert (instance, index, item);
		}


		public IntPtr this [int index] {
			get { return gee_list_get (instance, index); }
			set { gee_list_set (instance, index, value); }
		}


		public void RemoveAt (int index)
		{
			gee_list_remove_at (instance, index);
		}

		#endregion

		#region IEnumerable[System.IntPtr] implementation

		IEnumerator<IntPtr> IEnumerable<IntPtr>.GetEnumerator ()
		{
			return new GeeEnumerator (gee_iterable_iterator (instance));
		}

		#endregion

		internal List<T> ToTypedList<T> (Func<IntPtr,T> factory)
		{
			List<T> list = new List<T> (Math.Max (0, Count));
			foreach (IntPtr item in this) {
				list.Add (factory (item));
			}
			return list;
		}

		#region P/Invoke

		IntPtr instance;

		[DllImport ("libgee")]
		static extern bool gee_collection_add (IntPtr instance, IntPtr item);

		[DllImport ("libgee")]
		static extern void gee_collection_clear (IntPtr instance);

		[DllImport ("libgee")]
		static extern bool gee_collection_contains (IntPtr instance, IntPtr item);

		[DllImport ("libgee")]
		static extern int gee_collection_get_size (IntPtr instance);

		[DllImport ("libgee")]
		static extern bool gee_collection_remove (IntPtr instance, IntPtr item);

		[DllImport ("libgee")]
		static extern IntPtr gee_iterable_iterator (IntPtr instance);

		[DllImport ("libgee")]
		static extern int gee_list_index_of (IntPtr instance, IntPtr item);

		[DllImport ("libgee")]
		static extern void gee_list_insert (IntPtr instance, int index, IntPtr item);

		[DllImport ("libgee")]
		static extern IntPtr gee_list_get (IntPtr instance, int index);

		[DllImport ("libgee")]
		static extern void gee_list_set (IntPtr instance, int index, IntPtr item);

		[DllImport ("libgee")]
		static extern void gee_list_remove_at (IntPtr instance, int index);

		#endregion
	}

}

