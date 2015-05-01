//
// SourceFile.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MonoDevelop.ValaBinding.Parser.Afrodite
{
	/// <summary>
	/// Class to represent a CodeDOM source file
	/// </summary>
	internal class SourceFile
	{
		public SourceFile (string filename)
			: this (afrodite_source_file_new (filename))
		{
		}

		public SourceFile (IntPtr instance)
		{
			this.instance = instance;
		}

		/// <summary>
		/// Symbols declared in this source file
		/// </summary>
		public List<Symbol> Symbols {
			get {
				List<Symbol> list = new List<Symbol> ();
				IntPtr symbols = afrodite_source_file_get_symbols (instance);

				if (IntPtr.Zero != symbols) {
					list = new GeeList (symbols).ToTypedList (delegate (IntPtr item) {
						return new Symbol (item);
					});
				}

				return list;
			}
		}

		/// <summary>
		/// Using directives in this source file
		/// </summary>
		public List<DataType> UsingDirectives {
			get {
				List<DataType> list = new List<DataType> ();
				IntPtr symbols = afrodite_source_file_get_using_directives (instance);

				if (IntPtr.Zero != symbols) {
					list = new ValaList (symbols).ToTypedList (item => new DataType (item));
				}

				return list;
			}
		}

		/// <summary>
		/// The name of this source file
		/// </summary>
		public string Name {
			get{ return Marshal.PtrToStringAuto (afrodite_source_file_get_filename (instance)); }
		}

		#region P/Invoke

		IntPtr instance;

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_source_file_new (string filename);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_source_file_get_filename (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_source_file_get_symbols (IntPtr instance);

		[DllImport ("libafrodite")]
		static extern IntPtr afrodite_source_file_get_using_directives (IntPtr instance);


		#endregion
	}
}

