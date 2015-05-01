//
// ValaEnumerator.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections;

namespace MonoDevelop.ValaBinding.Parser.Echo
{
	/// <summary>
	/// IEnumerator wrapper for (Gee|Vala).Iterator
	/// </summary>
	public class GeeEnumerator: IEnumerator<IntPtr>
	{
		public GeeEnumerator (IntPtr instance)
		{
			this.instance = instance;
		}

		#region IDisposable implementation

		public void Dispose ()
		{
		}

		#endregion

		#region IEnumerator implementation

		object IEnumerator.Current {
			get { return ((IEnumerator<IntPtr>)this).Current; }
		}


		public bool MoveNext ()
		{
			return gee_iterator_next (instance);
		}


		public void Reset ()
		{
			throw new System.NotImplementedException ();
		}

		#endregion

		#region IEnumerator[System.IntPtr] implementation

		IntPtr IEnumerator<IntPtr>.Current {
			get { return gee_iterator_get (instance); }
		}

		#endregion

		#region P/Invoke

		IntPtr instance;

		[DllImport ("libgee")]
		static extern bool gee_iterator_next (IntPtr instance);

		[DllImport ("libgee")]
		static extern IntPtr gee_iterator_get (IntPtr instance);

		#endregion
	}
}

