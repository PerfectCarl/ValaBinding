//
// CompletionEngine.cs
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

namespace MonoDevelop.ValaBinding.Parser.Afrodite
{
	/// <summary>
	/// Afrodite completion engine - interface for queueing source and getting CodeDOMs
	/// </summary>
	internal class CompletionEngine
	{
		public CompletionEngine (string id)
		{
			instance = afrodite_completion_engine_new (id);
		}

		/// <summary>
		/// Queue a new source file for parsing
		/// </summary>
		public void QueueSourcefile (string path)
		{
			QueueSourcefile (path, !string.IsNullOrEmpty (path) && path.EndsWith (".vapi", StringComparison.OrdinalIgnoreCase), false);
		}

		/// <summary>
		/// Queue a new source file for parsing
		/// </summary>
		public void QueueSourcefile (string path, bool isVapi, bool isGlib)
		{
			afrodite_completion_engine_queue_sourcefile (instance, path, null, isVapi, isGlib);
		}

		/// <summary>
		/// Attempt to acquire the current CodeDOM
		/// </summary>
		/// <returns>
		/// A <see cref="CodeDom"/>: null if unable to acquire
		/// </returns>
		public CodeDom TryAcquireCodeDom ()
		{
			IntPtr codeDom = afrodite_completion_engine_get_codedom (instance);
			return (codeDom == IntPtr.Zero)? null: new CodeDom (codeDom, this);
		}

		/// <summary>
		/// Release the given CodeDOM (required for continued parsing)
		/// </summary>
		public void ReleaseCodeDom (CodeDom codeDom)
		{
			// Obsolete
		}

		#region P/Invokes

		IntPtr instance;

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_completion_engine_new (string id);

		[DllImport("libafrodite")]
		static extern void afrodite_completion_engine_queue_sourcefile (IntPtr instance, string path, string content, 
			bool is_vapi, bool is_glib);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_completion_engine_get_codedom (IntPtr instance);

		#endregion
	}
}

