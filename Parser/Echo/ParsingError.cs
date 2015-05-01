//
// ParsingError.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System;
using MonoDevelop.Ide.Tasks;
using System.Runtime.InteropServices;

namespace MonoDevelop.ValaBinding.Parser.Echo
{
	public class ParsingError
	{
		public ParsingError (IntPtr instance)
		{
			this.instance = instance;
		}

		public string FileFullPath {
			get{ return Marshal.PtrToStringAuto (echo_parsing_error_get_file_full_path (instance)); }
		}

		public int Line {
			get{ return echo_parsing_error_get_line (instance); }
		}

		public int Column {
			get{ return echo_parsing_error_get_column (instance); }
		}

		public string Message {
			get{ return Marshal.PtrToStringAuto (echo_parsing_error_get_message (instance)); }
		}

		public TaskSeverity Severity {
			get { 
				int type = echo_parsing_error_get_error_type (instance); 
				// Note
				if (type == 1)
					return TaskSeverity.Comment; 
				// Deprecated 
				if (type == 2)
					return TaskSeverity.Comment; 
				// Warning 
				if (type == 4)
					return TaskSeverity.Warning; 
				// Error 
				return TaskSeverity.Error; 
			}
		}

		#region P/Invokes

		IntPtr instance;

		[DllImport ("libecho")]
		static extern IntPtr echo_parsing_error_get_file_full_path (IntPtr instance);

		[DllImport ("libecho")]
		static extern int echo_parsing_error_get_line (IntPtr instance);

		[DllImport ("libecho")]
		static extern int echo_parsing_error_get_column (IntPtr instance);

		[DllImport ("libecho")]
		static extern IntPtr echo_parsing_error_get_message (IntPtr instance);

		[DllImport ("libecho")]
		static extern int echo_parsing_error_get_error_type (IntPtr instance);

		#endregion

	}
}

