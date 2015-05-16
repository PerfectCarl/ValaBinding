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

//using MonoDevelop.ValaBinding.Parser.Afrodite;
using MonoDevelop.Ide.CodeCompletion;
using ICSharpCode.NRefactory.Completion;

namespace MonoDevelop.ValaBinding.Parser
{


	/// <summary>
	/// Data for Vala completion
	/// </summary>
	internal class CompletionData : MonoDevelop.Ide.CodeCompletion.CompletionData
	{
		static Dictionary<string, CompletionCategory> categories = new Dictionary<string, CompletionCategory> ();

		private string image;
		private string text;
		private string description;
		private string completion_string;
		private string summary;
		private string return_type = "";
		private string signature = "";
		private string categoryName = "";

		public CompletionData (Echo.Symbol symbol)
		{
			this.text = symbol.Name;
			this.completion_string = symbol.Name;
			this.description = symbol.DisplayText;
			this.image = symbol.Icon;
			this.summary = symbol.Description;
			this.signature = GetSignature (symbol);
			if (symbol.ReturnType != null)
				this.return_type = symbol.ReturnType.TypeName;
			// TODO: markup and obsolete
			DisplayFlags = DisplayFlags.None /*& DisplayFlags.DescriptionHasMarkup*/;

			// Parent is null for completion symbol if (symbol.Parent != null)
			this.categoryName = symbol.CompletionParentName;
			if (!string.IsNullOrEmpty (categoryName)) {
				CompletionCategory category;
				categories.TryGetValue (categoryName, out category);
				if (category == null) {
					LoggingService.LogDebug ("Category : " + categoryName);
					category = new ValaCompletionCategory (categoryName, "");
					categories [categoryName] = category;
				}
				this.CompletionCategory = category;
			}
		}

		string GetSignature (Echo.Symbol symbol)
		{
			var result = "<b>" + symbol.Name + "</b>" + symbol.GetParameterDisplayText (true); 

			return result; 
		}

		public override IconId Icon {
			get { return image; }
		}

		public override string DisplayText {
			get { return text; }
		}

		public override string Description {
			get { return description; }
		}

		public override string CompletionText {
			get { return completion_string; }
		}

		/*public override string GetDisplayDescription (bool isSelected)
		{
			var result = base.GetDisplayDescription (isSelected);
			return return_type; 
		}*/

		public override string GetRightSideDescription (bool isSelected)
		{
			// var result = base.GetRightSideDescription (isSelected);
			return return_type; 
		}

		public override TooltipInformation CreateTooltipInformation (bool smartWrap)
		{
			var result = new TooltipInformation ();
			result.SignatureMarkup = signature;

			result.SummaryMarkup = string.Format ("<span face='normal' size='small'>{0}</span>", summary);
			result.FooterMarkup = "<span face='normal' size='small'>Defined in Glib</span>";

			return result;
		}

	}

	public class ValaCompletionDataList : CompletionDataList, IMutableCompletionDataList
	{
		public ValaCompletionDataList ()
			: base ()
		{
			IsChanging = true;
			Add (string.Empty);
		}

		internal virtual void AddRange (IEnumerable<CompletionData> vals)
		{
			foreach (CompletionData item in vals) {
				Add (item);
			}
		}

		#region IMutableCompletionDataList implementation

		public event EventHandler Changed;
		public event EventHandler Changing;


		public bool IsChanging {
			get { return isChanging; }
			set {
				isChanging = value;
				if (value) {
					OnChanging (this, null);
				} else {
					OnChanged (this, null);
				}
			}
		}

		private bool isChanging;

		#endregion

		protected virtual void OnChanging (object sender, EventArgs args)
		{
			if (null != Changing) {
				Changing (sender, args);
			}
		}

		protected virtual void OnChanged (object sender, EventArgs args)
		{
			if (null != Changed) {
				Changed (sender, args);
			}
		}

		public void Dispose ()
		{
		}
	}
	// ValaCompletionDataList

	internal class ValaCompletionCategory : CompletionCategory
	{
		public ValaCompletionCategory (string text, string image)
			: base (text, image)
		{
		}

		public override int CompareTo (CompletionCategory other)
		{
			if (other == null)
				return -1; 
			// Object should be displayed at the bottom
			if (this.DisplayText == "Object")
				return 1;
			return DisplayText.CompareTo (other.DisplayText);
		}
	}



	public class TextCompletion
	{
		//static readonly string[] containerTypes = new string[]{ "class", "struct", "interface" };

		//ProjectInformation projectInfo;
		//private CompletionEngine afroditeEngine;
		private readonly Echo.Project echoProject;

		internal TextCompletion (ProjectInformation projectInfo, /*CompletionEngine afroditeEngine,*/Echo.Project echoProject)
		{
			//this.projectInfo = projectInfo; 
			//this.afroditeEngine = afroditeEngine;
			this.echoProject = echoProject;
		}

		public List<Echo.Symbol> GetClassesForFile (string file)
		{
			//return projectInfo.GetSymbolsForFile (file, containerTypes);

			return echoProject.GetAllSymbolsForFile (file, Echo.SymbolType.CLASS); 
		
		}
		//		/// <summary>
		//		/// Get a list of namespaces declared in a given file
		//		/// </summary>
		//		[Obsolete]
		//		internal List<Afrodite.Symbol> GetNamespacesForFile (string file)
		//		{
		//			return projectInfo.GetSymbolsForFile (file, new string[]{ "namespace" });
		//		}

		//		/// <summary>
		//		/// Get types visible from a given source location
		//		/// </summary>
		//		internal void GetTypesVisibleFrom (string filename, int line, int column, ValaCompletionDataList results)
		//		{
		//			if (afroditeEngine == null) {
		//				return;
		//			}
		//
		//			// Add contents of parents
		//			ICollection<Afrodite.Symbol> containers = GetClassesForFile (filename);
		//			AddResults (containers, results);
		//			foreach (Afrodite.Symbol klass in containers) {
		//				// TODO: check source references once afrodite reliably captures the entire range
		//				for (Afrodite.Symbol parent = klass.Parent;
		//					parent != null;
		//					parent = parent.Parent) {
		//					AddResults (parent.Children.FindAll (delegate (Afrodite.Symbol sym) {
		//						return 0 <= Array.IndexOf (containerTypes, sym.MemberType.ToLower ());
		//					}), results);
		//				}
		//			}
		//			//if( afroditeEngine != null )
		//			using (Afrodite.CodeDom parseTree = afroditeEngine.TryAcquireCodeDom ()) {
		//				if (null == parseTree) {
		//					return;
		//				}
		//
		//				AddResults (GetNamespacesForFile (filename), results);
		//				AddResults (GetClassesForFile (filename), results);
		//				Afrodite.SourceFile file = parseTree.LookupSourceFile (filename);
		//				if (null != file) {
		//					Afrodite.Symbol parent;
		//					foreach (Afrodite.DataType directive in file.UsingDirectives) {
		//						if (directive.Symbol == null) {
		//							continue;
		//						}
		//						Afrodite.Symbol ns = parseTree.Lookup (directive.Symbol.FullyQualifiedName, out parent);
		//						if (null != ns) {
		//							containers = new List<Afrodite.Symbol> ();
		//							AddResults (new Afrodite.Symbol[]{ ns }, results);
		//							foreach (Afrodite.Symbol child in ns.Children) {
		//								foreach (string containerType in containerTypes) {
		//									if (containerType.Equals (child.MemberType, StringComparison.OrdinalIgnoreCase))
		//										containers.Add (child);
		//								}
		//							}
		//							AddResults (containers, results);
		//						}
		//					}
		//				}
		//			}
		//		}
		//// GetTypesVisibleFrom

		private static void AddResults (ValaCompletionDataList results, List<Echo.Symbol> symbols)
		{
			if (null == symbols || null == results) {
				//LoggingService.LogDebug ("AddResults: null list or results!");
				return;
			}
		
			List<CompletionData> data = new List<CompletionData> ();
			foreach (var symbol in symbols) {
				var item = new CompletionData (symbol);
				data.Add (item);
			}
			//DispatchService.GuiDispatch (delegate {
			results.IsChanging = true;
			results.AddRange (data);
			results.IsChanging = false;
			//});
		}

		public void Complete (ValaCompletionDataList results, MonoDevelop.Core.FilePath filePath, string lineText, char completionChar, int line, int column)
		{
			//LoggingService.LogDebug ("COMPLETING: {0} {1} {2}", lineText, line, column);
			//var result = new ValaCompletionDataList ();
			var symbols = echoProject.Complete (filePath, line, column, lineText);
			/*foreach (var sym in symbols) {
				LoggingService.LogDebug ("COMPLETE: " + sym.Name);
			}*/
			//return result;
			AddResults (results, symbols);
		}

		/// <summary>
		/// Get symbols visible from a given source location
		/// </summary>
		public void GetSymbolsVisibleFrom (string filename, int line, int column, ValaCompletionDataList results)
		{
			/*results.Add ("visible1");
			results.Add ("visible2");
			results.Add ("visible3");*/
			LoggingService.LogDebug ("GetSymbolsVisibleFrom: " + filename);
			var data = new List<CompletionData> ();
			// foreach (Afrodite.Symbol symbol in list) {
			//				// FIXME CARL data.Add (new CompletionData (symbol));
			//			}
			DispatchService.GuiDispatch (delegate {
				results.IsChanging = true;
				results.AddRange (data);
				results.IsChanging = false;
			});
			// TODO
			//GetTypesVisibleFrom (filename, line, column, results);
			//Complete ("this", filename, line, column, results);
		}
		// GetSymbolsVisibleFrom

		/// <summary>
		/// Add results to a ValaCompletionDataList on the GUI thread
		/// </summary>
		//		[Obsolete]
		//		private static void AddResults (IEnumerable<Afrodite.Symbol> list, ValaCompletionDataList results)
		//		{
		//			if (null == list || null == results) {
		//				LoggingService.LogDebug ("AddResults: null list or results!");
		//				return;
		//			}
		//
		//			List<CompletionData> data = new List<CompletionData> ();
		//			foreach (Afrodite.Symbol symbol in list) {
		//				// FIXME CARL data.Add (new CompletionData (symbol));
		//			}
		//
		//			DispatchService.GuiDispatch (delegate {
		//				results.IsChanging = true;
		//				results.AddRange (data);
		//				results.IsChanging = false;
		//			});
		//		}
		// AddResults

		/// <summary>
		/// Gets the completion list for a given type name in a given file
		/// </summary>
		//		[Obsolete]
		//		internal List<Afrodite.Symbol> CompleteType (string typename, string filename, int linenum, int column, ValaCompletionDataList results)
		//		{
		//			List<Afrodite.Symbol> nodes = new List<Afrodite.Symbol> ();
		//			if (afroditeEngine == null) {
		//				return nodes;
		//			}
		//			//if (afroditeEngine != null) { 
		//			using (Afrodite.CodeDom parseTree = afroditeEngine.TryAcquireCodeDom ()) {
		//				if (null != parseTree) {
		//					Afrodite.Symbol symbol = parseTree.GetSymbolForNameAndPath (typename, filename, linenum, column);
		//					if (null == symbol) {
		//						LoggingService.LogDebug ("CompleteType: Unable to lookup {0} in {1} at {2}:{3}", typename, filename, linenum, column);
		//					} else {
		//						nodes = symbol.Children;
		//					}
		//				} else {
		//					LoggingService.LogDebug ("CompleteType: Unable to acquire codedom");
		//				}
		//			}
		//			//}
		//			return nodes;
		//		}


		/// <summary>
		/// Get constructors for a given type
		/// </summary>
		//		[Obsolete]
		//		internal /*List<Afrodite.Symbol>*/ void GetConstructorsForType (string typename, string filename, int line, int column, ValaCompletionDataList results)
		//		{
		//			List<Afrodite.Symbol> functions = new List<Afrodite.Symbol> ();
		//			foreach (Afrodite.Symbol node in CompleteType (typename, filename, line, column, null)) {
		//				if ("constructor".Equals (node.MemberType, StringComparison.OrdinalIgnoreCase) ||
		//				    "creationmethod".Equals (node.MemberType, StringComparison.OrdinalIgnoreCase)) {
		//					functions.Add (node);
		//				}
		//			}
		//
		//			AddResults ((IList<Afrodite.Symbol>)functions, results);
		//
		//			//return functions;
		//		}
		// GetConstructorsForType

		public List<Echo.Symbol> GetConstructorsForType (string className, string fileFullPath, int line, int column, ValaCompletionDataList results)
		{
			//List<Echo.Symbol> results = new List<Echo.Symbol> ();
			/*foreach (Afrodite.Symbol node in CompleteType (typename, filename, line, column, null)) {
				if ("constructor".Equals (node.MemberType, StringComparison.OrdinalIgnoreCase) || 
					"creationmethod".Equals (node.MemberType, StringComparison.OrdinalIgnoreCase)) {
					functions.Add (node);
				}
			}

			AddResults ((IList<Echo.Symbol>)functions, results);
			*/
			//return results;
			LoggingService.LogDebug ("GetConstructorsForType: " + className);
			return echoProject.GetConstructorsForClass (fileFullPath, className, line, column);
		}

	}
}

