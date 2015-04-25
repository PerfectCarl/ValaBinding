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
using MonoDevelop.Core;

namespace MonoDevelop.ValaBinding.Parser.Afrodite
{
	/// <summary>
	/// Afrodite completion engine - interface for queueing source and getting CodeDOMs
	/// </summary>
	internal class CompletionEngine
	{
		static private bool vtgInstalled = false;
		static private bool checkedVtgInstalled = false;

		public CompletionEngine (string id)
		{
			instance = afrodite_completion_engine_new (id);
		}

		//// <value>
		/// Checks whether <see cref="http://code.google.com/p/vtg/">Vala Toys for GEdit</see> 
		/// is installed.
		/// </value>
		static internal bool DepsInstalled {
			get {
				if (!checkedVtgInstalled) {
					checkedVtgInstalled = true;
					vtgInstalled = false;
					try {
						Afrodite.Utils.GetPackagePaths ("glib-2.0");
						return (vtgInstalled = true);
					} catch (DllNotFoundException e) {
						LoggingService.LogWarning ("Cannot update Vala parser database because libafrodite (VTG) is not installed: {0}{1}{2}{3}", 
							Environment.NewLine, "http://code.google.com/p/vtg/",
							Environment.NewLine, "Note: If you're using Vala 0.10 or higher, you may need to symlink libvala-YOUR_VERSION.so to libvala.so");
						LoggingService.LogError ("Cannot load libafrodite", e);
					} catch (Exception ex) {
						LoggingService.LogError ("ValaBinding: Error while checking for libafrodite", ex);
					}
				}
				return vtgInstalled;
			}
			set {
				//don't assume that the caller is correct :-)
				if (value)
					checkedVtgInstalled = false; //will re-determine on next getting
				else
					vtgInstalled = false;
			}
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

