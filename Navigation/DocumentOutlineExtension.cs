//
// ValaOutline.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran

using System;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.DesignerSupport;
using Gtk;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide;
using MonoDevelop.Core;

// using MonoDevelop.ValaBinding.Parser.Afrodite;
using MonoDevelop.ValaBinding.Parser.Echo;
using MonoDevelop.ValaBinding.Parser;
using MonoDevelop.Ide.Gui.Components;

namespace MonoDevelop.ValaBinding.Navigation
{
	/// <summary>
	/// 'Overrides' MonoDevelop's own document outline.
	/// In the class itself, a tree widget is generated.
	/// The active document's D Syntax Tree will be scanned, whereas all its child items become added to the tree.
	/// </summary>
	public class DocumentOutlineExtension : TextEditorExtension, IOutlinedDocument
	{
		TreeModelSort sortedTreestore;
		TreeStore treestore;
		PadTreeView treeview;
		Widget[] toolbarWidgets;
		bool outlineReady;
		bool dontJumpToDeclaration;
		bool clickedOnOutlineItem;
		// ValaOutlineComparer comparer;

		void IOutlinedDocument.ReleaseOutlineWidget ()
		{
			if (treeview == null)
				return;
			var w = (ScrolledWindow)treeview.Parent;
			w.Destroy ();

			treestore.Dispose ();
			treestore = null;
			treeview = null;
			//settings = null;
			if (toolbarWidgets != null)
				foreach (var tw in toolbarWidgets)
					w.Destroy ();
			toolbarWidgets = null;
			//comparer = null;
		}

		public IEnumerable<Widget> GetToolbarWidgets ()
		{
			if (toolbarWidgets != null)
				return toolbarWidgets;

			var sortAlphabeticallyToggleButton = new ToggleButton () {
				Image = new Image (Ide.Gui.Stock.SortAlphabetically, IconSize.Menu),
				TooltipText = GettextCatalog.GetString ("Sort entries alphabetically"),
				Active = IsSorted,
			};	
			sortAlphabeticallyToggleButton.Toggled += delegate {
				if (sortAlphabeticallyToggleButton.Active == IsSorted)
					return;
				IsSorted = sortAlphabeticallyToggleButton.Active;
				UpdateSorting ();
			};

			return toolbarWidgets = new Widget[] {
				//groupToggleButton,
				sortAlphabeticallyToggleButton,
				//new VSeparator (),
				//preferencesButton,
			};
		}

		bool IsSorted { get ; set ; }

		void UpdateSorting ()
		{
			if (IsSorted) {
				// Sort the model, sort keys may have changed.
				// Only setting the column again does not re-sort so we set the function instead.
				//outlineTreeModelSort.SetSortFunc (0, comparer.CompareNodes);
				//treeview.Model = outlineTreeModelSort;
			} else {
				// treeview.Model = treestore;
			}

			// Because sorting the tree by setting the sort function also collapses the tree view we expand
			// the whole tree.
			treeview.ExpandAll ();
		}

		public override bool ExtendsEditor (Document doc, IEditableTextBuffer editor)
		{
			return true;
		}

		public override void Initialize ()
		{
			base.Initialize ();
			if (Document != null) {
				Document.DocumentParsed += UpdateDocumentOutline;
				Document.Editor.Caret.PositionChanged += UpdateOutlineSelection;
			}
		}

		public Widget GetOutlineWidget ()
		{
			if (treeview != null)
				return treeview;

			treestore = new TreeStore (typeof(object));
			sortedTreestore = new TreeModelSort (treestore);
			//comparer = new ClassOutlineNodeComparer (GetAmbience (), settings, sortedTreestore);

			//sortedTreestore.SetSortFunc (0, comparer.CompareNodes);
			//sortedTreestore.SetSortColumnId (0, SortType.Ascending);

			treeview = new MonoDevelop.Ide.Gui.Components.PadTreeView (treestore);

			var pixRenderer = new CellRendererPixbuf ();
			pixRenderer.Xpad = 0;
			pixRenderer.Ypad = 0;

			treeview.TextRenderer.Xpad = 0;
			treeview.TextRenderer.Ypad = 0;

			TreeViewColumn treeCol = new TreeViewColumn ();
			treeCol.PackStart (pixRenderer, false);

			treeCol.SetCellDataFunc (pixRenderer, new TreeCellDataFunc (OutlineTreeIconFunc));
			treeCol.PackStart (treeview.TextRenderer, true);

			treeCol.SetCellDataFunc (treeview.TextRenderer, new TreeCellDataFunc (OutlineTreeTextFunc));
			treeview.AppendColumn (treeCol);

			treeview.TextRenderer.Editable = false;

			treeview.HeadersVisible = false;

			treeview.Selection.Changed += delegate {
				if (dontJumpToDeclaration || !outlineReady)
					return;

				clickedOnOutlineItem = true;
				JumpToDeclaration (true);
				clickedOnOutlineItem = false;
			};

			treeview.Realized += delegate {
				RefillOutlineStore ();
			};

			var sw = new CompactScrolledWindow ();
			sw.Add (treeview);
			sw.ShowAll ();
			return sw;
		}


		uint refillOutlineStoreId;

		void UpdateDocumentOutline (object sender, EventArgs args)
		{
			RefillOutlineStore ();
		}

		void RemoveRefillOutlineStoreTimeout ()
		{
			if (refillOutlineStoreId == 0)
				return;
			GLib.Source.Remove (refillOutlineStoreId);
			refillOutlineStoreId = 0;
		}

		public bool RefillOutlineStore ()
		{
			DispatchService.AssertGuiThread ();
			Gdk.Threads.Enter ();

			//refreshingOutline = false;
			if (treestore == null || !treeview.IsRealized) {
				refillOutlineStoreId = 0;
				return false;
			}

			outlineReady = false;

			// Save last selection
			int[] lastSelectedItem;
			TreeIter i;

			if (treeview.Selection.GetSelected (out i))
				lastSelectedItem = treestore.GetPath (i).Indices;
			else
				lastSelectedItem = null;

			// Save previously expanded items if wanted
			var lastExpanded = new List<int[]> ();
			if (/*DCompilerService.Instance.Outline.ExpansionBehaviour == DocOutlineCollapseBehaviour.ReopenPreviouslyExpanded && */
				treestore.GetIterFirst (out i)) {
				do {
					var path = treestore.GetPath (i);
					if (treeview.GetRowExpanded (path))
						lastExpanded.Add (path.Indices);
				} while(treestore.IterNext (ref i));
			}

			// Clear the tree
			treestore.Clear ();
			if (ProjectInfo == null)
				return false;
			try {
				// Build up new tree
				var caretLocation = Document.Editor.Caret.Location;
				foreach (var symbol in ProjectInfo.GetRootSymbolsForFile (FileName.CanonicalPath)) {
					BuildTreeChildren (TreeIter.Zero, symbol, caretLocation.Column, caretLocation.Line);
				}
				treeview.ExpandAll ();
			} catch (Exception ex) {
				LoggingService.LogError ("Error while updating document outline panel", ex);
			} finally {
				// Re-Expand tree items
				/*switch(DCompilerService.Instance.Outline.ExpansionBehaviour)
				{
				case DocOutlineCollapseBehaviour.ExpandAll:
					TreeView.ExpandAll();
					break;
				case DocOutlineCollapseBehaviour.ReopenPreviouslyExpanded:
					foreach (var path in lastExpanded)
						TreeView.ExpandToPath(new TreePath(path));
					break;
				}*/

				// Restore selection
				if (lastSelectedItem != null) {
					try {
						treeview.ExpandToPath (new TreePath (lastSelectedItem));
					} catch {
					}
				}

				outlineReady = true;
			}
			Gdk.Threads.Leave ();

			//stop timeout handler
			refillOutlineStoreId = 0;
			return false;
		}

		private ProjectInformation ProjectInfo {
			get {
				ValaProject project = Document.Project as ValaProject;
				return (null == project) ? null : ProjectInformationManager.Instance.Get (project);
			}
		}

		/*private SourceReference GetFirstReference (Symbol symbol) {
			if (symbol.SourceReferences.Count == 0)
				return null;
			return symbol.SourceReferences[0] ;
		}*/

		void BuildTreeChildren (TreeIter ParentTreeNode, Symbol symbol, int column, int line)
		{
			TreeIter childIter = ParentTreeNode; 
			// var source = GetFirstReference (symbol);
			//if (source.File == FileName.CanonicalPath) {
			if (!ParentTreeNode.Equals (TreeIter.Zero))
				childIter = treestore.AppendValues (ParentTreeNode, symbol);
			else
				childIter = treestore.AppendValues (symbol);
			//}
			foreach (var child in symbol.Children) {
				BuildTreeChildren (childIter, child, column, line);

				/*if (editorSelectionLocation >= symbol.Location &&
					editorSelectionLocation < symbol.EndLocation)
					TreeView.Selection.SelectIter(childIter);*/
			}
			/* if (ParentAstNode == null)
				return;

			if (ParentAstNode is DMethod)
			{
				if (DCompilerService.Instance.Outline.ShowFuncParams)
				{
					var dm = ParentAstNode as DMethod;

					if (dm.Parameters != null)
						foreach (var p in dm.Parameters)
							if (p.Name != "")
							{
								TreeIter childIter;
								if (!ParentTreeNode.Equals(TreeIter.Zero))
									childIter = TreeStore.AppendValues(ParentTreeNode, p);
								else
									childIter = TreeStore.AppendValues(p);

								if (editorSelectionLocation >= p.Location &&
									editorSelectionLocation < p.EndLocation)
									TreeView.Selection.SelectIter(childIter);
							}
				}
			}

			foreach (var n in ParentAstNode)
			{
				if (n is DEnum && (n as DEnum).IsAnonymous)
				{
					BuildTreeChildren(ParentTreeNode, n as IBlockNode, editorSelectionLocation);
					continue;
				}

				if (!DCompilerService.Instance.Outline.ShowFuncVariables && 
					ParentAstNode is DMethod && 
					n is DVariable)
					continue;

				if (n is DMethod && string.IsNullOrEmpty(n.Name)) // Check against delegates & unittests
					continue;

				TreeIter childIter;
				if (!ParentTreeNode.Equals(TreeIter.Zero))
					childIter = TreeStore.AppendValues(ParentTreeNode,n);
				else
					childIter = TreeStore.AppendValues(n);

				if (editorSelectionLocation >= n.Location && 
					editorSelectionLocation < n.EndLocation)
					TreeView.Selection.SelectIter(childIter);

				BuildTreeChildren(childIter, n as IBlockNode,editorSelectionLocation);
			}*/
		}

		void JumpToDeclaration (bool focusEditor)
		{
			if (!outlineReady)
				return;

			TreeIter iter;
			if (!treeview.Selection.GetSelected (out iter))
				return;

			var symbol = treestore.GetValue (iter, 0) as Symbol;

			if (symbol == null)
				return;

			int line = 0; 
			int column = 0; 
			string filename = "";
			var source = symbol.Declaration; // GetFirstReference (symbol);
			filename = source.File; 
			line = source.FirstLine; 
			column = source.FirstColumn;
			//line = symbol.
			var openedDoc = IdeApp.Workbench.GetDocument (filename);

			if (openedDoc == null)
				return;


			openedDoc.Editor.SetCaretTo (line, column);
			openedDoc.Editor.ScrollToCaret ();

			if (focusEditor) {
				IdeApp.Workbench.ActiveDocument.Select ();
			}

			openedDoc.Editor.Document.EnsureOffsetIsUnfolded (
				openedDoc.Editor.LocationToOffset (
					line,
					column
				));
		}

		void UpdateOutlineSelection (object sender, Mono.TextEditor.DocumentLocationEventArgs e)
		{
			//var ast = SyntaxTree;

			if (clickedOnOutlineItem || treestore == null)
				return;
			if (ProjectInfo == null)
				return;
			var caretLocation = Document.Editor.Caret.Location;
			var selectedSymbol = ProjectInfo.GetEnclosingSymbolAtPosition (FileName.FullPath, caretLocation.Line, caretLocation.Column);
			treestore.Foreach ((TreeModel model, TreePath path, TreeIter iter) => {
				var symbol = model.GetValue (iter, 0);
				if (symbol.Equals (selectedSymbol)) {
					dontJumpToDeclaration = true;
					//var parentPath = path.Copy().Up();

					//treeview.ExpandToPath (path);
					treeview.Selection.SelectIter (iter);
					dontJumpToDeclaration = false;

					return true;
				}

				return false;
			});
		}

		// Draw the icon of the item displayed in the document outline
		void OutlineTreeIconFunc (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			var pixRenderer = (CellRendererPixbuf)cell;
			var symbol = model.GetValue (iter, 0) as Symbol;
			var id = symbol.Icon;
			if (id != null) {
				var img = ImageService.GetIcon (id);
				if (img != null)
					pixRenderer.Pixbuf = img.ToPixbuf (IconSize.Menu);
			}

		}

		// Returns
		string GetDisplayText (Symbol symbol)
		{
			var result = "<b>" + GLib.Markup.EscapeText (symbol.Name) + "</b>" + GLib.Markup.EscapeText (symbol.GetParameterDisplayText (false)); 

			return result; 
		}

		// Draw the text of the item displayed in the document outline
		void OutlineTreeTextFunc (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			var symbol = model.GetValue (iter, 0) as Symbol;
			string label;
			var renderer = (CellRendererText)cell;

			label = GetDisplayText (symbol) ?? string.Empty;
			if (symbol.Accessibility != SymbolAccessibility.Public)
				renderer.Foreground = "#606060";
			else
				renderer.Foreground = "black";
			renderer.Markup = label;
			//renderer.Text = label;
		}
	}

}

