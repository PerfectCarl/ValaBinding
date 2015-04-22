//
// CodeDom.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MonoDevelop.ValaBinding.Parser.Afrodite
{
	/// <summary>
	/// Represents a Vala CodeDOM
	/// </summary>
	/// <remarks>
	/// MUST be disposed for parsing to continue
	/// </remarks>
	internal class CodeDom: IDisposable
	{
		CompletionEngine engine;

		/// <summary>
		/// Create a new CodeDOM wrapper
		/// </summary>
		/// <param name="instance">
		/// A <see cref="IntPtr"/>: The native pointer for this CodeDOM
		/// </param>
		/// <param name="engine">
		/// A <see cref="CompletionEngine"/>: The completion engine to which this CodeDOM belongs
		/// </param>
		public CodeDom (IntPtr instance, CompletionEngine engine)
		{
			this.instance = instance;
			this.engine = engine;
		}

		public QueryResult GetSymbolsForPath (string path)
		{
			return new QueryResult (afrodite_code_dom_get_symbols_for_path (instance, new QueryOptions ().Instance, path));
		}

		/// <summary>
		/// Lookup the symbol at a given location
		/// </summary>
		public Symbol LookupSymbolAt (string filename, int line, int column)
		{
			IntPtr symbol = afrodite_code_dom_lookup_symbol_at (instance, filename, line, column);
			return (IntPtr.Zero == symbol)? null: new Symbol (symbol);
		}

		/// <summary>
		/// Lookup a symbol and its parent by fully qualified name
		/// </summary>
		public Symbol Lookup (string fully_qualified_name, out Symbol parent)
		{
			IntPtr parentInstance = IntPtr.Zero,
			result = IntPtr.Zero;

			result = afrodite_code_dom_lookup (instance, fully_qualified_name, out parentInstance);
			parent = (IntPtr.Zero == parentInstance)? null: new Symbol (parentInstance);
			return (IntPtr.Zero == result)? null: new Symbol (result);
		}

		/// <summary>
		/// Lookup a symbol, given a name and source location
		/// </summary>
		public Symbol GetSymbolForNameAndPath (string name, string path, int line, int column)
		{
			IntPtr result = afrodite_code_dom_get_symbol_for_name_and_path (instance, QueryOptions.Standard ().Instance,
				name, path, line, column);
			if (IntPtr.Zero != result) {
				QueryResult qresult = new QueryResult (result);
				if (null != qresult.Children && 0 < qresult.Children.Count)
					return qresult.Children[0].Symbol;
			}

			return null;
		}

		/// <summary>
		/// Get the source files used to create this CodeDOM
		/// </summary>
		public List<SourceFile> SourceFiles {
			get {
				List<SourceFile> files = new List<SourceFile> ();
				IntPtr sourceFiles = afrodite_code_dom_get_source_files (instance);

				if (IntPtr.Zero != sourceFiles) {
					ValaList list = new ValaList (sourceFiles);
					files = list.ToTypedList (delegate (IntPtr item){ return new SourceFile (item); });
				}

				return files;
			}
		}

		/// <summary>
		/// Lookup a source file by filename
		/// </summary>
		public SourceFile LookupSourceFile (string filename)
		{
			IntPtr sourceFile = afrodite_code_dom_lookup_source_file (instance, filename);
			return (IntPtr.Zero == sourceFile)? null: new SourceFile (sourceFile);
		}

		#region P/Invokes

		IntPtr instance;

		internal IntPtr Instance {
			get{ return instance; }
		}

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_code_dom_get_symbols_for_path (IntPtr instance, IntPtr options, string path);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_code_dom_lookup_symbol_at (IntPtr instance, string filename, int line, int column);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_code_dom_lookup (IntPtr instance, string fully_qualified_name, out IntPtr parent);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_code_dom_get_symbol_for_name_and_path (IntPtr instance, IntPtr options,
			string symbol_qualified_name, string path,
			int line, int column);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_code_dom_get_source_files (IntPtr instance);

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_code_dom_lookup_source_file (IntPtr instance, string filename);

		#endregion

		#region IDisposable implementation

		/// <summary>
		/// Release this CodeDOM for reuse
		/// </summary>
		public void Dispose ()
		{
			engine.ReleaseCodeDom (this);
		}

		#endregion
	}
}

