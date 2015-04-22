//
// ValaOutline.cs
//
// Author:
//       cran <>
//
// Copyright (c) 2015 cran
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.DesignerSupport;
using Gtk;
using System.Collections.Generic;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

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
			MonoDevelop.Core.LoggingService.Log (MonoDevelop.Core.Logging.LogLevel.Warn, "MEMEME");
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

			/*TreeView.Selection.Changed += delegate {
				if (dontJumpToDeclaration || !outlineReady)
					return;

				clickedOnOutlineItem = true;
				JumpToDeclaration (true);
				clickedOnOutlineItem = false;
			};*/

			TreeView.Realized += delegate { RefillOutlineStore(); };

			var sw = new CompactScrolledWindow();
			sw.Add(TreeView);
			sw.ShowAll();
			return sw;
		}

		public bool RefillOutlineStore()
		{
			return false;
		}

		void OutlineTreeIconFunc(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{}

		void OutlineTreeTextFunc (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{}
	}

}

