//
// Query.cs
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
	/// Wrapper class for Afrodite query results
	/// </summary>
	internal class QueryResult
	{
		public QueryResult (IntPtr instance)
		{
			this.instance = instance;
		}

		/// <summary>
		/// ResultItems contained in this query result
		/// </summary>
		public List<ResultItem> Children {
			get {
				List<ResultItem> list = new List<ResultItem> ();
				IntPtr children = afrodite_query_result_get_children (instance);

				if (IntPtr.Zero != children) {
					list = new ValaList (children).ToTypedList (delegate (IntPtr item){ return new ResultItem (item); });
				}

				return list;
			}
		}

		#region P/Invokes

		IntPtr instance;

		internal IntPtr Instance {
			get{ return instance; }
		}

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_query_result_get_children (IntPtr instance);

		#endregion
	}

	/// <summary>
	/// A single result from a query
	/// </summary>
	internal class ResultItem
	{
		public ResultItem (IntPtr instance)
		{
			this.instance = instance;
		}

		public Symbol Symbol {
			get {
				IntPtr symbol = afrodite_result_item_get_symbol (instance);
				return (IntPtr.Zero == symbol)? null: new Symbol (symbol);
			}
		}

		#region P/Invokes

		IntPtr instance;

		internal IntPtr Instance {
			get{ return instance; }
		}

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_result_item_get_symbol (IntPtr instance);

		#endregion
	}

	/// <summary>
	/// Options for querying a CodeDOM
	/// </summary>
	internal class QueryOptions
	{
		public QueryOptions (): this (afrodite_query_options_new ())
		{
		}

		public QueryOptions (IntPtr instance)
		{
			this.instance = instance;
		}

		public static QueryOptions Standard ()
		{
			return new QueryOptions (afrodite_query_options_standard ());
		}

		#region P/Invokes

		IntPtr instance;

		internal IntPtr Instance {
			get{ return instance; }
		}

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_query_options_new ();

		[DllImport("libafrodite")]
		static extern IntPtr afrodite_query_options_standard ();

		#endregion
	}
}

