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

namespace MonoDevelop.ValaBinding.Parser.Echo
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
				return Marshal.PtrToStringAuto (echo_source_reference_get_file_full_path (instance));
			}
		}

		public int FirstLine {
			get{ return echo_source_reference_get_line (instance); }
		}

		/*public int LastLine {
			get{ return afrodite_source_reference_get_last_line (instance); }
		}*/

		public int FirstColumn {
			get{ return echo_source_reference_get_column (instance); }
		}

		/*public int LastColumn {
			get{ return afrodite_source_reference_get_last_column (instance); }
		}*/

		#region P/Invoke

		IntPtr instance;

		[DllImport("libecho")]
		static extern IntPtr echo_source_reference_get_file_full_path (IntPtr instance);

		[DllImport("libecho")]
		static extern int echo_source_reference_get_line (IntPtr instance);

		[DllImport("libecho")]
		static extern int echo_source_reference_get_column (IntPtr instance);

		#endregion
	}
}

