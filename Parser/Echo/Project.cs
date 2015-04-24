//
// Project.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MonoDevelop.ValaBinding.Parser.Echo
{
	public class Project
	{
		public Project ()
		{
			this.instance = echo_project_new ();
		}

		public void AddExternalPackage (string vala_package)
		{
			echo_project_add_external_package (instance, vala_package);
		}

		public void AddFile (string filePath)
		{
			echo_project_add_file (instance, filePath);
		}

		public void UpdateSync ()
		{
			echo_project_update_sync (instance);
		}

		internal List<Symbol> GetSymbolsForFile (string file_full_path) {
			List<Symbol> list = new List<Symbol> ();
			IntPtr children = echo_project_get_symbols_for_file (instance, file_full_path);

			if (IntPtr.Zero != children) {
				list = new ValaList (children).ToTypedList (item => new Symbol (item));
			}

			return list;
		}

		#region P/Invokes

		IntPtr instance;

		[DllImport("libecho")]
		static extern IntPtr echo_project_new ();

		[DllImport("libecho")]
		static extern void echo_project_add_external_package (IntPtr instance, string package);

		[DllImport("libecho")]
		static extern void echo_project_add_file (IntPtr instance, string file_path);

		[DllImport("libecho")]
		static extern void echo_project_update_sync (IntPtr instance);

		[DllImport("libecho")]
		static extern IntPtr echo_project_get_symbols_for_file (IntPtr instance, string file_path);

		#endregion
	}
}

