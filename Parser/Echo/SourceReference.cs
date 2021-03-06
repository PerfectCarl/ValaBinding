﻿//
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
	public class SourceReference
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

		public int LastLine {
			get{ return echo_source_reference_get_last_line (instance); }
		}

		public int FirstColumn {
			get{ return echo_source_reference_get_column (instance); }
		}

		/*public int LastColumn {
			get{ return afrodite_source_reference_get_last_column (instance); }
		}*/

		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			SourceReference source = (SourceReference)obj;
			return source.File == this.File &&
			source.FirstLine == this.FirstLine &&
			source.FirstColumn == this.FirstColumn &&
			source.LastLine == this.LastLine;
		}

		public override int GetHashCode ()
		{
			return new { File, FirstLine, FirstColumn, LastLine  }.GetHashCode ();
		}

		#region P/Invoke

		IntPtr instance;

		[DllImport ("libecho")]
		static extern IntPtr echo_source_reference_get_file_full_path (IntPtr instance);

		[DllImport ("libecho")]
		static extern int echo_source_reference_get_line (IntPtr instance);

		[DllImport ("libecho")]
		static extern int echo_source_reference_get_last_line (IntPtr instance);

		[DllImport ("libecho")]
		static extern int echo_source_reference_get_column (IntPtr instance);

		#endregion
	}
}

