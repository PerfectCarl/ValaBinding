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
using MonoDevelop.ValaBinding.Parser.Afrodite;
using MonoDevelop.ValaBinding.Parser;

namespace MonoDevelop.ValaBinding
{
	/// <summary>
	/// 'Overrides' MonoDevelop's own document outline.
	/// In the class itself, a tree widget is generated.
	/// The active document's D Syntax Tree will be scanned, whereas all its child items become added to the tree.
	/// </summary>
	public class ValaOutlineExtension : TextEditorExtension, IOutlinedDocument
	{
		TreeStore TreeStore;
		MonoDevelop.Ide.Gui.Components.PadTreeView TreeView;
		Widget[] toolbarWidgets;
		bool outlineReady;
		bool dontJumpToDeclaration;
		bool clickedOnOutlineItem; 
		void IOutlinedDocument.ReleaseOutlineWidget()
		{
			if (TreeView == null)
				return;
			var w = (ScrolledWindow)TreeView.Parent;
			w.Destroy();

			TreeStore.Dispose();
			TreeStore = null;
			TreeView = null;
			//settings = null;
			if(toolbarWidgets!=null)
				foreach (var tw in toolbarWidgets)
					w.Destroy();
			toolbarWidgets = null;
			//comparer = null;
		}

		public IEnumerable<Widget> GetToolbarWidgets()
		{
			return null;
		}

		public override bool ExtendsEditor(Document doc, IEditableTextBuffer editor)
		{
			return true;
		}
			
		public override void Initialize()
		{
			base.Initialize();
			if (Document != null)
			{
				Document.DocumentParsed += UpdateDocumentOutline;
				// CARL Document.Editor.Caret.PositionChanged += UpdateOutlineSelection;
			}
		}
		public Widget GetOutlineWidget()
		{
			if (TreeView != null)
				return TreeView;

			TreeStore = new TreeStore(typeof(object));

			TreeView = new MonoDevelop.Ide.Gui.Components.PadTreeView(TreeStore);

			var pixRenderer = new CellRendererPixbuf();
			pixRenderer.Xpad = 0;
			pixRenderer.Ypad = 0;

			TreeView.TextRenderer.Xpad = 0;
			TreeView.TextRenderer.Ypad = 0;

			TreeViewColumn treeCol = new TreeViewColumn();
			treeCol.PackStart(pixRenderer, false);

			treeCol.SetCellDataFunc(pixRenderer, new TreeCellDataFunc(OutlineTreeIconFunc));
			treeCol.PackStart(TreeView.TextRenderer, true);

			treeCol.SetCellDataFunc(TreeView.TextRenderer, new TreeCellDataFunc(OutlineTreeTextFunc));
			TreeView.AppendColumn(treeCol);

			TreeView.TextRenderer.Editable = false;

			TreeView.HeadersVisible = false;

			TreeView.Selection.Changed += delegate {
				if (dontJumpToDeclaration || !outlineReady)
					return;

				clickedOnOutlineItem = true;
				JumpToDeclaration (true);
				clickedOnOutlineItem = false;
			};

			TreeView.Realized += delegate { RefillOutlineStore(); };

			var sw = new CompactScrolledWindow();
			sw.Add(TreeView);
			sw.ShowAll();
			return sw;
		}


		uint refillOutlineStoreId;
		void UpdateDocumentOutline(object sender, EventArgs args)
		{
			RefillOutlineStore();
		}

		void RemoveRefillOutlineStoreTimeout()
		{
			if (refillOutlineStoreId == 0)
				return;
			GLib.Source.Remove(refillOutlineStoreId);
			refillOutlineStoreId = 0;
		}

		public bool RefillOutlineStore()
		{
			DispatchService.AssertGuiThread();
			Gdk.Threads.Enter();

			//refreshingOutline = false;
			if (TreeStore == null || !TreeView.IsRealized)
			{
				refillOutlineStoreId = 0;
				return false;
			}

			outlineReady = false;

			// Save last selection
			int[] lastSelectedItem;
			TreeIter i;

			if (TreeView.Selection.GetSelected(out i))
				lastSelectedItem = TreeStore.GetPath(i).Indices;
			else 
				lastSelectedItem = null;

			// Save previously expanded items if wanted
			var lastExpanded = new List<int[]>();
			if (/*DCompilerService.Instance.Outline.ExpansionBehaviour == DocOutlineCollapseBehaviour.ReopenPreviouslyExpanded && */
				TreeStore.GetIterFirst(out i))
			{
				do{
					var path = TreeStore.GetPath(i);
					if(TreeView.GetRowExpanded(path))
						lastExpanded.Add(path.Indices);
				}
				while(TreeStore.IterNext(ref i));
			}

			// Clear the tree
			TreeStore.Clear();

			try
			{
				// Build up new tree
				var caretLocation = Document.Editor.Caret.Location;
				foreach (var symbol in Parser.GetRootSymbolsForFile (FileName.CanonicalPath)) {
					BuildTreeChildren(TreeIter.Zero, symbol, caretLocation.Column, caretLocation.Line);
				}
			}
			catch (Exception ex)
			{
				LoggingService.LogError("Error while updating document outline panel", ex);
			}
			finally
			{
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
				if (lastSelectedItem != null)
				{
					try
					{
						TreeView.ExpandToPath(new TreePath(lastSelectedItem));
					}
					catch { }
				}

				outlineReady = true;
			}
			Gdk.Threads.Leave();

			//stop timeout handler
			refillOutlineStoreId = 0;
			return false;
		}

		private ProjectInformation Parser
		{
			get
			{
				ValaProject project = Document.Project as ValaProject;
				return (null == project) ? null : ProjectInformationManager.Instance.Get(project);
			}
		}

		void BuildTreeChildren(TreeIter ParentTreeNode, Symbol symbol, int column, int line)
		{
			TreeIter childIter; 

			if (!ParentTreeNode.Equals(TreeIter.Zero))
				childIter = TreeStore.AppendValues(ParentTreeNode, symbol);
			else
				childIter = TreeStore.AppendValues(symbol);

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

		void OutlineTreeIconFunc(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			var pixRenderer = (CellRendererPixbuf)cell;
			/*object o = model.GetValue(iter, 0);
			string id = null;

			if (o is DNode)
			{
				var icon = DIcons.GetNodeIcon(o as DNode);
				if (!icon.IsNull)
					id = icon.Name;
			}
			else if (o is StatementContainingStatement)
				id = "gtk-add";
*/
			var symbol = model.GetValue (iter, 0) as Symbol;
			var id = symbol.Icon;
			if (id != null)
			{
				var img = ImageService.GetIcon(id);
				if (img != null)
					pixRenderer.Pixbuf = img.ToPixbuf(IconSize.Menu);
			}

		}

		void JumpToDeclaration(bool focusEditor)
		{
			if (!outlineReady)
				return;

			TreeIter iter;
			if (!TreeView.Selection.GetSelected(out iter))
				return;

			var symbol = TreeStore.GetValue(iter, 0) as Symbol;

			if(symbol==null)
				return;

			int line = 0; 
			int column = 0; 
			string filename = "";
			foreach (var source in symbol.SourceReferences) {
				if( source != null )
				{
					filename = source.File; 
					line = source.FirstLine; 
					column = source.FirstColumn;
					break; 
				}
			}
			var openedDoc=IdeApp.Workbench.GetDocument(filename);

			if (openedDoc == null)
				return;


			openedDoc.Editor.SetCaretTo(line, column);
			openedDoc.Editor.ScrollToCaret();

			if (focusEditor)
			{
				IdeApp.Workbench.ActiveDocument.Select();
			}

			openedDoc.Editor.Document.EnsureOffsetIsUnfolded(
				openedDoc.Editor.LocationToOffset(
					line,
					column
				));
		}

		void OutlineTreeTextFunc (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			var symbol = model.GetValue (iter, 0) as Symbol;

			string label;

			/*var dm = n as DMethod;
			if (dm != null) {
				switch (dm.SpecialType) {
				case DMethod.MethodType.Unittest:
					label = "(Unittest)";
					break;
				case DMethod.MethodType.ClassInvariant:
					label = "(Class Invariant)";
					break;
				case DMethod.MethodType.Allocator:
					label = "(Class Allocator)";
					break;
				case DMethod.MethodType.Deallocator:
					label = "(Class Deallocator)";
					break;
				default:
					label = DCompilerService.Instance.Outline.ShowFuncParams ? dm.ToString (false, false) : n.Name;
					break;
				}
			} else*/
			label = symbol.DisplayText ?? string.Empty;

			/*if (DCompilerService.Instance.Outline.ShowBaseTypes)
				label = String.Format ("{0} {1}", (n as DNode).Type, label);*/

			// if (DCompilerService.Instance.Outline.GrayOutNonPublic)
			{
				// symbol.Accessibility
				//var dn = n as DNode;
				//if (dn != null)
				{
					if (symbol.Accessibility != SymbolAccessibility.Public)
						(cell as CellRendererText).Foreground = "#606060";
					else
						(cell as CellRendererText).Foreground = "black";
				}
			}

			(cell as CellRendererText).Text = label;
		}
	}

}

