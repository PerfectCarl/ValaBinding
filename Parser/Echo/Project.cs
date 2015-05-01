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
using MonoDevelop.Core;

namespace MonoDevelop.ValaBinding.Parser.Echo
{
	public class Project
	{
		string name;

		public Project (string name)
		{
			this.name = name;
			this.instance = echo_project_new (name);
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
			LoggingService.Log (MonoDevelop.Core.Logging.LogLevel.Info, "Updating " + name);
			echo_project_update_sync (instance);
		}

		public void UpdateFileContents (string fileFullPath, string content, bool scheduleUpdate)
		{
			echo_project_update_file_contents (instance, fileFullPath, content, scheduleUpdate);
		}

		public List<Symbol> GetAllSymbolsForFile (string fileFullPath, SymbolType type)
		{
			List<Symbol> list = new List<Symbol> ();
			IntPtr items = echo_project_get_all_symbols_for_file (instance, fileFullPath, type);

			if (IntPtr.Zero != items) {
				list = new GeeList (items).ToTypedList (item => new Symbol (item));
			}

			return list;
		}

		public List<ParsingError> GetParsingErrors ()
		{

			List<ParsingError> list = new List<ParsingError> ();
			IntPtr items = echo_project_get_parsing_errors (instance);

			if (IntPtr.Zero != items) {
				list = new GeeList (items).ToTypedList (item => new ParsingError (item));
			}

			return list;
		}

		internal List<Symbol> GetSymbolsForFile (string fileFullPath)
		{
			List<Symbol> list = new List<Symbol> ();
			IntPtr items = echo_project_get_symbols_for_file (instance, fileFullPath);

			if (IntPtr.Zero != items) {
				list = new GeeList (items).ToTypedList (item => new Symbol (item));
			}

			return list;
		}

		public Symbol GetEnclosingSymbolAtPosition (string fileFullPath, int line, int column)
		{
			IntPtr item = echo_project_get_enclosing_symbol_at_position (instance, fileFullPath, line, column);
			return (IntPtr.Zero == item) ? null : new Symbol (item);
		}

		public CompletionReport complete (string fileFullPath, string lineText, char completionChar, int line, int column)
		{
			return new CompletionReport ();
		}

		public bool TargetGlib232 {
			get {
				return echo_project_get_target_glib232 (instance);
			}
			set {
				echo_project_set_target_glib232 (instance, value);
			}
		}

		public List<Symbol> GetConstructorsForClass (string fileFullPath, string className, int line, int column)
		{
			List<Symbol> list = new List<Symbol> ();
			IntPtr items = echo_project_get_constructors_for_class (instance, fileFullPath, className, line, column);

			if (IntPtr.Zero != items) {
				list = new GeeList (items).ToTypedList (item => new Symbol (item));
			}

			return list;
		}

		#region P/Invokes

		IntPtr instance;

		[DllImport ("libecho")]
		static extern IntPtr echo_project_new (string name);

		[DllImport ("libecho")]
		static extern void echo_project_add_external_package (IntPtr instance, string package);

		[DllImport ("libecho")]
		static extern void echo_project_add_file (IntPtr instance, string file_path);

		[DllImport ("libecho")]
		static extern void echo_project_update_sync (IntPtr instance);

		[DllImport ("libecho")]
		static extern IntPtr echo_project_get_symbols_for_file (IntPtr instance, string file_path);

		[DllImport ("libecho")]
		static extern IntPtr echo_project_get_enclosing_symbol_at_position (IntPtr instance, string file_full_path, int line, int column);

		[DllImport ("libecho")]
		static extern IntPtr echo_project_get_all_symbols_for_file (IntPtr instance, string file_full_path, SymbolType type);

		[DllImport ("libecho")]
		static extern IntPtr echo_project_get_constructors_for_class (IntPtr instance, string file_full_path, string class_name, int line, int column);

		[DllImport ("libecho")]
		static extern void echo_project_update_file_contents (IntPtr instance, string file_full_path, string content, bool schedule_update) ;

		[DllImport ("libecho")]
		static extern bool echo_project_get_target_glib232 (IntPtr instance) ;

		[DllImport ("libecho")]
		static extern void echo_project_set_target_glib232 (IntPtr instance, bool target_glib232) ;

		[DllImport ("libecho")]
		static extern IntPtr echo_project_get_parsing_errors (IntPtr instance) ;

		#endregion
	}
}

