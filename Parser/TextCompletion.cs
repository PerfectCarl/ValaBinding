//
// TextCompletion.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
using System;
using System.Collections.Generic;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.ValaBinding.Parser.Afrodite ;

namespace MonoDevelop.ValaBinding.Parser
{

	public class TextCompletion
	{
		static readonly string[] containerTypes = new string[]{ "class", "struct", "interface" };

		ProjectInformation projectInfo ; 
		private CompletionEngine afroditeEngine;

		internal TextCompletion (ProjectInformation projectInfo, CompletionEngine afroditeEngine )
		{
			this.projectInfo = projectInfo; 
			this.afroditeEngine = afroditeEngine;
		}

		/// <summary>
		/// Get constructors for a given expression
		/// </summary>
		internal List<Afrodite.Symbol> GetConstructorsForExpression (string expression, string filename, int line, int column, ValaCompletionDataList results)
		{
			string typename = projectInfo.GetExpressionType (expression, filename, line, column);
			return GetConstructorsForType (typename, filename, line, column, results);
		}// GetConstructorsForExpression

		/// <summary>
		/// Get a list of classes declared in a given file
		/// </summary>
		internal List<Afrodite.Symbol> GetClassesForFile (string file)
		{
			return projectInfo.GetSymbolsForFile (file, containerTypes);
		}// GetClassesForFile

		/// <summary>
		/// Get a list of namespaces declared in a given file
		/// </summary>
		internal List<Afrodite.Symbol> GetNamespacesForFile (string file)
		{
			return projectInfo.GetSymbolsForFile (file, new string[]{ "namespace" });
		}

		/// <summary>
		/// Get types visible from a given source location
		/// </summary>
		internal void GetTypesVisibleFrom (string filename, int line, int column, ValaCompletionDataList results)
		{
			if (afroditeEngine == null){ return; }

			// Add contents of parents
			ICollection<Afrodite.Symbol> containers = GetClassesForFile (filename);
			AddResults (containers, results);
			foreach (Afrodite.Symbol klass in containers) {
				// TODO: check source references once afrodite reliably captures the entire range
				for (Afrodite.Symbol parent = klass.Parent;
					parent != null;
					parent = parent.Parent)
				{
					AddResults (parent.Children.FindAll (delegate (Afrodite.Symbol sym){
						return 0 <= Array.IndexOf (containerTypes, sym.MemberType.ToLower ());
					}), results);
				}
			}
			//if( afroditeEngine != null )
				using (Afrodite.CodeDom parseTree = afroditeEngine.TryAcquireCodeDom ()) {
					if (null == parseTree){ return; }

					AddResults (GetNamespacesForFile (filename), results);
					AddResults (GetClassesForFile (filename), results);
					Afrodite.SourceFile file = parseTree.LookupSourceFile (filename);
					if (null != file) {
						Afrodite.Symbol parent;
						foreach (Afrodite.DataType directive in file.UsingDirectives) {
							if (directive.Symbol == null) { continue; }
							Afrodite.Symbol ns = parseTree.Lookup (directive.Symbol.FullyQualifiedName, out parent);
							if (null != ns) {
								containers = new List<Afrodite.Symbol> ();
								AddResults (new Afrodite.Symbol[]{ ns }, results);
								foreach (Afrodite.Symbol child in ns.Children) {
									foreach (string containerType in containerTypes) {
										if (containerType.Equals (child.MemberType, StringComparison.OrdinalIgnoreCase))
											containers.Add (child);
									}
								}
								AddResults (containers, results);
							}
						}
					}
				}
		}// GetTypesVisibleFrom

		/// <summary>
		/// Get symbols visible from a given source location
		/// </summary>
		internal void GetSymbolsVisibleFrom (string filename, int line, int column, ValaCompletionDataList results) 
		{
			GetTypesVisibleFrom (filename, line, column, results);
			Complete ("this", filename, line, column, results);
		}// GetSymbolsVisibleFrom

		/// <summary>
		/// Add results to a ValaCompletionDataList on the GUI thread
		/// </summary>
		private static void AddResults (IEnumerable<Afrodite.Symbol> list, ValaCompletionDataList results) 
		{
			if (null == list || null == results)
			{
				LoggingService.LogDebug ("AddResults: null list or results!");
				return;
			}

			List<CompletionData> data = new List<CompletionData> ();
			foreach (Afrodite.Symbol symbol in list) {
				data.Add (new CompletionData (symbol));
			}

			DispatchService.GuiDispatch (delegate {
				results.IsChanging = true;
				results.AddRange (data);
				results.IsChanging = false;
			});
		}// AddResults

		/// <summary>
		/// Gets the completion list for a given type name in a given file
		/// </summary>
		internal List<Afrodite.Symbol> CompleteType (string typename, string filename, int linenum, int column, ValaCompletionDataList results)
		{
			List<Afrodite.Symbol> nodes = new List<Afrodite.Symbol> ();
			if (afroditeEngine == null){ return nodes; }
			//if (afroditeEngine != null) { 
				using (Afrodite.CodeDom parseTree = afroditeEngine.TryAcquireCodeDom ()) {
					if (null != parseTree) {
						Afrodite.Symbol symbol = parseTree.GetSymbolForNameAndPath (typename, filename, linenum, column);
						if (null == symbol) {
							LoggingService.LogDebug ("CompleteType: Unable to lookup {0} in {1} at {2}:{3}", typename, filename, linenum, column);
						} else {
							nodes = symbol.Children;
						}
					} else {
						LoggingService.LogDebug ("CompleteType: Unable to acquire codedom");
					}
				}
			//}
			return nodes;
		}


		/// <summary>
		/// Get constructors for a given type
		/// </summary>
		internal List<Afrodite.Symbol> GetConstructorsForType (string typename, string filename, int line, int column, ValaCompletionDataList results)
		{
			List<Afrodite.Symbol> functions = new List<Afrodite.Symbol> ();
			foreach (Afrodite.Symbol node in CompleteType (typename, filename, line, column, null)) {
				if ("constructor".Equals (node.MemberType, StringComparison.OrdinalIgnoreCase) || 
					"creationmethod".Equals (node.MemberType, StringComparison.OrdinalIgnoreCase)) {
					functions.Add (node);
				}
			}

			AddResults ((IList<Afrodite.Symbol>)functions, results);

			return functions;
		}// GetConstructorsForType

		/// <summary>
		/// Gets the completion list for a given symbol at a given location
		/// </summary>
		internal List<Afrodite.Symbol> Complete (string symbol, string filename, int line, int column, ValaCompletionDataList results)
		{
			List<Afrodite.Symbol> nodes = new List<Afrodite.Symbol> ();
			if (afroditeEngine == null){ return nodes; }

			//if( afroditeEngine != null )
			using (Afrodite.CodeDom parseTree = afroditeEngine.TryAcquireCodeDom ()) {
					if (null != parseTree) {
						LoggingService.LogDebug ("Complete: Looking up symbol at {0}:{1}:{2}", filename, line, column);
						Afrodite.Symbol sym = parseTree.GetSymbolForNameAndPath (symbol, filename, line, column);
						LoggingService.LogDebug ("Complete: Got {0}", (null == sym)? "null": sym.Name);
						if (null != sym) {
							nodes = sym.Children;
							AddResults (nodes, results);
						}
					} else {
						LoggingService.LogDebug ("Complete: Unable to acquire codedom");
					}
				}

			return nodes;
		}// Complete

	}
}

