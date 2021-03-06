
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.ValaBinding
{
	public partial class OutputOptionsPanel
	{
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.Table table1;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Entry outputPathTextEntry;
		
		private global::Gtk.Button browseButton;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Entry outputNameTextEntry;
		
		private global::Gtk.Entry parametersTextEntry;
		
		private global::Gtk.CheckButton externalConsoleCheckbox;
		
		private global::Gtk.CheckButton pauseCheckbox;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.ValaBinding.OutputOptionsPanel
			global::Stetic.BinContainer.Attach (this);
			this.Name = "MonoDevelop.ValaBinding.OutputOptionsPanel";
			// Container child MonoDevelop.ValaBinding.OutputOptionsPanel.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.vbox2.BorderWidth = ((uint)(3));
			// Container child vbox2.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(4)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			this.table1.BorderWidth = ((uint)(3));
			// Container child table1.Gtk.Table+TableChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.outputPathTextEntry = new global::Gtk.Entry ();
			this.outputPathTextEntry.CanFocus = true;
			this.outputPathTextEntry.Name = "outputPathTextEntry";
			this.outputPathTextEntry.IsEditable = true;
			this.outputPathTextEntry.InvisibleChar = '●';
			this.hbox1.Add (this.outputPathTextEntry);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.outputPathTextEntry]));
			w1.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.browseButton = new global::Gtk.Button ();
			this.browseButton.CanFocus = true;
			this.browseButton.Name = "browseButton";
			this.browseButton.UseUnderline = true;
			this.browseButton.Label = global::Mono.Unix.Catalog.GetString ("_Browse");
			this.hbox1.Add (this.browseButton);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.browseButton]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.table1.Add (this.hbox1);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.hbox1]));
			w3.TopAttach = ((uint)(2));
			w3.BottomAttach = ((uint)(3));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Output</b>");
			this.label1.UseMarkup = true;
			this.table1.Add (this.label1);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.label1]));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Output Name:");
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.XPadding = ((uint)(15));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Output Path:");
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.label3]));
			w6.TopAttach = ((uint)(2));
			w6.BottomAttach = ((uint)(3));
			w6.XPadding = ((uint)(15));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Parameters:");
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w7.TopAttach = ((uint)(3));
			w7.BottomAttach = ((uint)(4));
			w7.XPadding = ((uint)(15));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.outputNameTextEntry = new global::Gtk.Entry ();
			this.outputNameTextEntry.CanFocus = true;
			this.outputNameTextEntry.Name = "outputNameTextEntry";
			this.outputNameTextEntry.IsEditable = true;
			this.outputNameTextEntry.InvisibleChar = '●';
			this.table1.Add (this.outputNameTextEntry);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.outputNameTextEntry]));
			w8.TopAttach = ((uint)(1));
			w8.BottomAttach = ((uint)(2));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.parametersTextEntry = new global::Gtk.Entry ();
			this.parametersTextEntry.CanFocus = true;
			this.parametersTextEntry.Name = "parametersTextEntry";
			this.parametersTextEntry.IsEditable = true;
			this.parametersTextEntry.InvisibleChar = '●';
			this.table1.Add (this.parametersTextEntry);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.parametersTextEntry]));
			w9.TopAttach = ((uint)(3));
			w9.BottomAttach = ((uint)(4));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox2.Add (this.table1);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.table1]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.externalConsoleCheckbox = new global::Gtk.CheckButton ();
			this.externalConsoleCheckbox.CanFocus = true;
			this.externalConsoleCheckbox.Name = "externalConsoleCheckbox";
			this.externalConsoleCheckbox.Label = global::Mono.Unix.Catalog.GetString ("Run on e_xternal console");
			this.externalConsoleCheckbox.Active = true;
			this.externalConsoleCheckbox.DrawIndicator = true;
			this.externalConsoleCheckbox.UseUnderline = true;
			this.vbox2.Add (this.externalConsoleCheckbox);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.externalConsoleCheckbox]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.pauseCheckbox = new global::Gtk.CheckButton ();
			this.pauseCheckbox.Sensitive = false;
			this.pauseCheckbox.CanFocus = true;
			this.pauseCheckbox.Name = "pauseCheckbox";
			this.pauseCheckbox.Label = global::Mono.Unix.Catalog.GetString ("Pause _console output");
			this.pauseCheckbox.DrawIndicator = true;
			this.pauseCheckbox.UseUnderline = true;
			this.vbox2.Add (this.pauseCheckbox);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.pauseCheckbox]));
			w12.Position = 2;
			w12.Expand = false;
			w12.Fill = false;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Show ();
			this.browseButton.Clicked += new global::System.EventHandler (this.OnBrowseButtonClick);
			this.externalConsoleCheckbox.Clicked += new global::System.EventHandler (this.OnExternalConsoleCheckboxClicked);
		}
	}
}
