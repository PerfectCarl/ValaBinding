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

		public List<Symbol> GetAllSymbolsForFile (string fileFullPath, SymbolType type)
		{
			List<Symbol> list = new List<Symbol> ();
			IntPtr items = echo_project_get_all_symbols_for_file (instance, fileFullPath,type);

			if (IntPtr.Zero != items) {
				list = new ValaList (items).ToTypedList (item => new Symbol (item));
			}

			return list;
		}

		internal List<Symbol> GetSymbolsForFile (string fileFullPath) {
			List<Symbol> list = new List<Symbol> ();
			IntPtr items = echo_project_get_symbols_for_file (instance, fileFullPath);

			if (IntPtr.Zero != items) {
				list = new ValaList (items).ToTypedList (item => new Symbol (item));
			}

			return list;
		}

		public Symbol GetSymbolAtPosition (string symbolName, string fileFullPath, int line, int column)
		{
			IntPtr item = echo_project_get_symbol_at_position (instance, symbolName, fileFullPath, line, column);
			return (IntPtr.Zero == item)? null: new Symbol (item);
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

		[DllImport("libecho")]
		static extern IntPtr echo_project_get_symbol_at_position (IntPtr instance, string symbol_name, string file_full_path, int line, int column);

		[DllImport("libecho")]
		static extern IntPtr echo_project_get_all_symbols_for_file (IntPtr instance, string file_full_path, SymbolType type);


		#endregion
	}
}

