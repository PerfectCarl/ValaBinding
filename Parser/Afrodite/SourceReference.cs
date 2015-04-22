//
// SourceReference.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System;
using System.Runtime.InteropServices;

namespace MonoDevelop.ValaBinding.Parser.Afrodite
{
	/// <summary>
	/// Class to represent a reference area in a source file
	/// </summary>
	internal class SourceReference
	{
		public SourceReference (IntPtr instance)
		{
			this.instance = instance;
		}

		public string File {
			get { 
				IntPtr sourcefile = afrodite_source_reference_get_file (instance);
				return (IntPtr.Zero == sourcefile)? string.Empty: new SourceFile (sourcefile).Name;
			}
		}

		public int FirstLine {
			get{ return afrodite_source_reference_get_first_line (instance); }
		}

		public int LastLine {
			get{ return afrodite_source_reference_get_last_line (instance); }
		}

		public int FirstColumn {
			get{ return afrodite_source_reference_get_first_column (instance); }
		}

		public int LastColumn {
			get{ return afrodite_source_reference_get_last_column (instance); }
		}

		#region P/Invoke

		IntPtr instance;

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_source_reference_get_file (IntPtr instance);

		[DllImport("libafrodite")]
		static extern int afrodite_source_reference_get_first_line (IntPtr instance);

		[DllImport("libafrodite")]
		static extern int afrodite_source_reference_get_last_line (IntPtr instance);

		[DllImport("libafrodite")]
		static extern int afrodite_source_reference_get_first_column (IntPtr instance);

		[DllImport("libafrodite")]
		static extern int afrodite_source_reference_get_last_column (IntPtr instance);


		#endregion
	}
}

