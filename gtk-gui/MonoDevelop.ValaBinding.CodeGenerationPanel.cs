
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.ValaBinding
{
	public partial class CodeGenerationPanel
	{
		private global::Gtk.Notebook notebook1;
		
		private global::Gtk.VBox vbox6;
		
		private global::Gtk.Table table1;
		
		private global::Gtk.Label label10;
		
		private global::Gtk.Label label11;
		
		private global::Gtk.Label label13;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.CheckButton linkMathsLib;
		
		private global::Gtk.SpinButton optimizationSpinButton;
		
		private global::Gtk.ComboBox targetComboBox;
		
		private global::Gtk.CheckButton targetGlib232;
		
		private global::Gtk.CheckButton threadingCheckbox;
		
		private global::Gtk.VBox vbox1;
		
		private global::Gtk.RadioButton noWarningRadio;
		
		private global::Gtk.RadioButton normalWarningRadio;
		
		private global::Gtk.RadioButton allWarningRadio;
		
		private global::Gtk.CheckButton warningsAsErrorsCheckBox;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Label label12;
		
		private global::Gtk.Entry defineSymbolsTextEntry;
		
		private global::Gtk.HBox hbox3;
		
		private global::Gtk.Label label14;
		
		private global::Gtk.Entry gettextID;
		
		private global::Gtk.Frame frame2;
		
		private global::Gtk.Alignment GtkAlignment;
		
		private global::Gtk.Table table5;
		
		private global::Gtk.Label label7;
		
		private global::Gtk.ScrolledWindow scrolledwindow4;
		
		private global::Gtk.TextView extraCompilerTextView;
		
		private global::Gtk.Label GtkLabel12;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Table table2;
		
		private global::Gtk.Button addLibButton;
		
		private global::Gtk.Label label8;
		
		private global::Gtk.Entry libAddEntry;
		
		private global::Gtk.ScrolledWindow scrolledwindow1;
		
		private global::Gtk.TreeView libTreeView;
		
		private global::Gtk.VBox vbox4;
		
		private global::Gtk.Button browseButton;
		
		private global::Gtk.Button removeLibButton;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.VBox vbox7;
		
		private global::Gtk.Table table3;
		
		private global::Gtk.Button includePathAddButton;
		
		private global::Gtk.Entry includePathEntry;
		
		private global::Gtk.Label label9;
		
		private global::Gtk.ScrolledWindow scrolledwindow2;
		
		private global::Gtk.TreeView includePathTreeView;
		
		private global::Gtk.VBox vbox5;
		
		private global::Gtk.Button includePathBrowseButton;
		
		private global::Gtk.Button includePathRemoveButton;
		
		private global::Gtk.Label label3;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.ValaBinding.CodeGenerationPanel
			global::Stetic.BinContainer.Attach (this);
			this.Name = "MonoDevelop.ValaBinding.CodeGenerationPanel";
			// Container child MonoDevelop.ValaBinding.CodeGenerationPanel.Gtk.Container+ContainerChild
			this.notebook1 = new global::Gtk.Notebook ();
			this.notebook1.CanFocus = true;
			this.notebook1.Name = "notebook1";
			this.notebook1.CurrentPage = 0;
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.vbox6 = new global::Gtk.VBox ();
			this.vbox6.Name = "vbox6";
			this.vbox6.Spacing = 3;
			// Container child vbox6.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(7)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(5));
			this.table1.ColumnSpacing = ((uint)(5));
			this.table1.BorderWidth = ((uint)(2));
			// Container child table1.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xpad = 10;
			this.label10.Xalign = 0F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Warning Level:");
			this.table1.Add (this.label10);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.label10]));
			w1.TopAttach = ((uint)(1));
			w1.BottomAttach = ((uint)(2));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.Xpad = 10;
			this.label11.Xalign = 0F;
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("Targeted glib:");
			this.table1.Add (this.label11);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.label11]));
			w2.TopAttach = ((uint)(5));
			w2.BottomAttach = ((uint)(6));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label13 = new global::Gtk.Label ();
			this.label13.Name = "label13";
			this.label13.Xpad = 10;
			this.label13.Xalign = 0F;
			this.label13.LabelProp = global::Mono.Unix.Catalog.GetString ("Use math functions:");
			this.table1.Add (this.label13);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.label13]));
			w3.TopAttach = ((uint)(6));
			w3.BottomAttach = ((uint)(7));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xpad = 10;
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Multithreading:");
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w4.TopAttach = ((uint)(4));
			w4.BottomAttach = ((uint)(5));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xpad = 10;
			this.label5.Xalign = 0F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Optimization Level:");
			this.table1.Add (this.label5);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.label5]));
			w5.TopAttach = ((uint)(2));
			w5.BottomAttach = ((uint)(3));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xpad = 10;
			this.label6.Xalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Target:");
			this.table1.Add (this.label6);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.label6]));
			w6.TopAttach = ((uint)(3));
			w6.BottomAttach = ((uint)(4));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.linkMathsLib = new global::Gtk.CheckButton ();
			this.linkMathsLib.CanFocus = true;
			this.linkMathsLib.Name = "linkMathsLib";
			this.linkMathsLib.Label = global::Mono.Unix.Catalog.GetString ("Link math library (-lm)");
			this.linkMathsLib.DrawIndicator = true;
			this.linkMathsLib.UseUnderline = true;
			this.table1.Add (this.linkMathsLib);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.linkMathsLib]));
			w7.TopAttach = ((uint)(6));
			w7.BottomAttach = ((uint)(7));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.optimizationSpinButton = new global::Gtk.SpinButton (0, 3, 1);
			this.optimizationSpinButton.CanFocus = true;
			this.optimizationSpinButton.Name = "optimizationSpinButton";
			this.optimizationSpinButton.Adjustment.PageIncrement = 10;
			this.optimizationSpinButton.ClimbRate = 1;
			this.optimizationSpinButton.Numeric = true;
			this.table1.Add (this.optimizationSpinButton);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.optimizationSpinButton]));
			w8.TopAttach = ((uint)(2));
			w8.BottomAttach = ((uint)(3));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.targetComboBox = global::Gtk.ComboBox.NewText ();
			this.targetComboBox.AppendText (global::Mono.Unix.Catalog.GetString ("Executable"));
			this.targetComboBox.AppendText (global::Mono.Unix.Catalog.GetString ("Static Library"));
			this.targetComboBox.AppendText (global::Mono.Unix.Catalog.GetString ("Shared Object"));
			this.targetComboBox.Name = "targetComboBox";
			this.table1.Add (this.targetComboBox);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.targetComboBox]));
			w9.TopAttach = ((uint)(3));
			w9.BottomAttach = ((uint)(4));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.targetGlib232 = new global::Gtk.CheckButton ();
			this.targetGlib232.CanFocus = true;
			this.targetGlib232.Name = "targetGlib232";
			this.targetGlib232.Label = global::Mono.Unix.Catalog.GetString ("target newer version of glib (2.32)");
			this.targetGlib232.DrawIndicator = true;
			this.targetGlib232.UseUnderline = true;
			this.table1.Add (this.targetGlib232);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.targetGlib232]));
			w10.TopAttach = ((uint)(5));
			w10.BottomAttach = ((uint)(6));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.threadingCheckbox = new global::Gtk.CheckButton ();
			this.threadingCheckbox.CanFocus = true;
			this.threadingCheckbox.Name = "threadingCheckbox";
			this.threadingCheckbox.Label = global::Mono.Unix.Catalog.GetString ("Enable multithreading");
			this.threadingCheckbox.DrawIndicator = true;
			this.threadingCheckbox.UseUnderline = true;
			this.table1.Add (this.threadingCheckbox);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.threadingCheckbox]));
			w11.TopAttach = ((uint)(4));
			w11.BottomAttach = ((uint)(5));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 1;
			// Container child vbox1.Gtk.Box+BoxChild
			this.noWarningRadio = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("no warnings"));
			this.noWarningRadio.CanFocus = true;
			this.noWarningRadio.Name = "noWarningRadio";
			this.noWarningRadio.DrawIndicator = true;
			this.noWarningRadio.UseUnderline = true;
			this.noWarningRadio.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.vbox1.Add (this.noWarningRadio);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.noWarningRadio]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.normalWarningRadio = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("normal"));
			this.normalWarningRadio.CanFocus = true;
			this.normalWarningRadio.Name = "normalWarningRadio";
			this.normalWarningRadio.DrawIndicator = true;
			this.normalWarningRadio.UseUnderline = true;
			this.normalWarningRadio.Group = this.noWarningRadio.Group;
			this.vbox1.Add (this.normalWarningRadio);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.normalWarningRadio]));
			w13.Position = 1;
			w13.Expand = false;
			w13.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.allWarningRadio = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("all"));
			this.allWarningRadio.CanFocus = true;
			this.allWarningRadio.Name = "allWarningRadio";
			this.allWarningRadio.DrawIndicator = true;
			this.allWarningRadio.UseUnderline = true;
			this.allWarningRadio.Group = this.noWarningRadio.Group;
			this.vbox1.Add (this.allWarningRadio);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.allWarningRadio]));
			w14.Position = 2;
			w14.Expand = false;
			w14.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.warningsAsErrorsCheckBox = new global::Gtk.CheckButton ();
			this.warningsAsErrorsCheckBox.Sensitive = false;
			this.warningsAsErrorsCheckBox.CanFocus = true;
			this.warningsAsErrorsCheckBox.Name = "warningsAsErrorsCheckBox";
			this.warningsAsErrorsCheckBox.Label = global::Mono.Unix.Catalog.GetString ("Treat warnings as errors");
			this.warningsAsErrorsCheckBox.DrawIndicator = true;
			this.warningsAsErrorsCheckBox.UseUnderline = true;
			this.vbox1.Add (this.warningsAsErrorsCheckBox);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.warningsAsErrorsCheckBox]));
			w15.Position = 3;
			w15.Expand = false;
			w15.Fill = false;
			this.table1.Add (this.vbox1);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table1 [this.vbox1]));
			w16.TopAttach = ((uint)(1));
			w16.BottomAttach = ((uint)(2));
			w16.LeftAttach = ((uint)(1));
			w16.RightAttach = ((uint)(2));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox6.Add (this.table1);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.table1]));
			w17.Position = 0;
			// Container child vbox6.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label12 = new global::Gtk.Label ();
			this.label12.Name = "label12";
			this.label12.Xpad = 13;
			this.label12.Xalign = 0F;
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString ("Define Symbols:");
			this.hbox1.Add (this.label12);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label12]));
			w18.Position = 0;
			w18.Expand = false;
			w18.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.defineSymbolsTextEntry = new global::Gtk.Entry ();
			this.defineSymbolsTextEntry.TooltipMarkup = "A space-separated list of symbols to define.";
			this.defineSymbolsTextEntry.WidthRequest = 300;
			this.defineSymbolsTextEntry.CanFocus = true;
			this.defineSymbolsTextEntry.Name = "defineSymbolsTextEntry";
			this.defineSymbolsTextEntry.IsEditable = true;
			this.defineSymbolsTextEntry.InvisibleChar = '●';
			this.hbox1.Add (this.defineSymbolsTextEntry);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.defineSymbolsTextEntry]));
			w19.Position = 1;
			w19.Expand = false;
			w19.Padding = ((uint)(14));
			this.vbox6.Add (this.hbox1);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.hbox1]));
			w20.Position = 1;
			w20.Expand = false;
			w20.Fill = false;
			// Container child vbox6.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.label14 = new global::Gtk.Label ();
			this.label14.Name = "label14";
			this.label14.Xpad = 13;
			this.label14.Xalign = 0F;
			this.label14.LabelProp = global::Mono.Unix.Catalog.GetString ("Gettext id:");
			this.hbox3.Add (this.label14);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.label14]));
			w21.Position = 0;
			w21.Expand = false;
			w21.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.gettextID = new global::Gtk.Entry ();
			this.gettextID.WidthRequest = 300;
			this.gettextID.CanFocus = true;
			this.gettextID.Name = "gettextID";
			this.gettextID.IsEditable = true;
			this.gettextID.InvisibleChar = '•';
			this.hbox3.Add (this.gettextID);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.gettextID]));
			w22.Position = 1;
			w22.Expand = false;
			w22.Padding = ((uint)(44));
			this.vbox6.Add (this.hbox3);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.hbox3]));
			w23.Position = 2;
			w23.Expand = false;
			w23.Fill = false;
			w23.Padding = ((uint)(2));
			// Container child vbox6.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			this.frame2.ShadowType = ((global::Gtk.ShadowType)(0));
			this.frame2.LabelYalign = 0F;
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.table5 = new global::Gtk.Table (((uint)(2)), ((uint)(1)), false);
			this.table5.Name = "table5";
			this.table5.RowSpacing = ((uint)(6));
			this.table5.ColumnSpacing = ((uint)(9));
			this.table5.BorderWidth = ((uint)(6));
			// Container child table5.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 0F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Extra Compiler Options");
			this.table5.Add (this.label7);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table5 [this.label7]));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table5.Gtk.Table+TableChild
			this.scrolledwindow4 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow4.CanFocus = true;
			this.scrolledwindow4.Name = "scrolledwindow4";
			this.scrolledwindow4.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child scrolledwindow4.Gtk.Container+ContainerChild
			this.extraCompilerTextView = new global::Gtk.TextView ();
			this.extraCompilerTextView.TooltipMarkup = "A newline-separated list of extra options to send to the compiler.\nOne option can be in more than one line.\nExample:\n\t`--pkg\n\tcairo`";
			this.extraCompilerTextView.CanFocus = true;
			this.extraCompilerTextView.Name = "extraCompilerTextView";
			this.scrolledwindow4.Add (this.extraCompilerTextView);
			this.table5.Add (this.scrolledwindow4);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table5 [this.scrolledwindow4]));
			w26.TopAttach = ((uint)(1));
			w26.BottomAttach = ((uint)(2));
			this.GtkAlignment.Add (this.table5);
			this.frame2.Add (this.GtkAlignment);
			this.GtkLabel12 = new global::Gtk.Label ();
			this.GtkLabel12.Name = "GtkLabel12";
			this.GtkLabel12.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Extra Options</b>");
			this.GtkLabel12.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel12;
			this.vbox6.Add (this.frame2);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.frame2]));
			w29.Position = 3;
			this.notebook1.Add (this.vbox6);
			// Notebook tab
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Code Generation");
			this.notebook1.SetTabLabel (this.vbox6, this.label1);
			this.label1.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.table2 = new global::Gtk.Table (((uint)(2)), ((uint)(3)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(10));
			this.table2.ColumnSpacing = ((uint)(10));
			this.table2.BorderWidth = ((uint)(3));
			// Container child table2.Gtk.Table+TableChild
			this.addLibButton = new global::Gtk.Button ();
			this.addLibButton.Sensitive = false;
			this.addLibButton.CanFocus = true;
			this.addLibButton.Name = "addLibButton";
			this.addLibButton.UseUnderline = true;
			this.addLibButton.Label = global::Mono.Unix.Catalog.GetString ("Add");
			this.table2.Add (this.addLibButton);
			global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.table2 [this.addLibButton]));
			w31.LeftAttach = ((uint)(2));
			w31.RightAttach = ((uint)(3));
			w31.XOptions = ((global::Gtk.AttachOptions)(4));
			w31.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Library:");
			this.table2.Add (this.label8);
			global::Gtk.Table.TableChild w32 = ((global::Gtk.Table.TableChild)(this.table2 [this.label8]));
			w32.XOptions = ((global::Gtk.AttachOptions)(4));
			w32.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.libAddEntry = new global::Gtk.Entry ();
			this.libAddEntry.CanFocus = true;
			this.libAddEntry.Name = "libAddEntry";
			this.libAddEntry.IsEditable = true;
			this.libAddEntry.InvisibleChar = '●';
			this.table2.Add (this.libAddEntry);
			global::Gtk.Table.TableChild w33 = ((global::Gtk.Table.TableChild)(this.table2 [this.libAddEntry]));
			w33.LeftAttach = ((uint)(1));
			w33.RightAttach = ((uint)(2));
			w33.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow1.CanFocus = true;
			this.scrolledwindow1.Name = "scrolledwindow1";
			this.scrolledwindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child scrolledwindow1.Gtk.Container+ContainerChild
			this.libTreeView = new global::Gtk.TreeView ();
			this.libTreeView.CanFocus = true;
			this.libTreeView.Name = "libTreeView";
			this.scrolledwindow1.Add (this.libTreeView);
			this.table2.Add (this.scrolledwindow1);
			global::Gtk.Table.TableChild w35 = ((global::Gtk.Table.TableChild)(this.table2 [this.scrolledwindow1]));
			w35.TopAttach = ((uint)(1));
			w35.BottomAttach = ((uint)(2));
			w35.LeftAttach = ((uint)(1));
			w35.RightAttach = ((uint)(2));
			// Container child table2.Gtk.Table+TableChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.browseButton = new global::Gtk.Button ();
			this.browseButton.CanFocus = true;
			this.browseButton.Name = "browseButton";
			this.browseButton.UseUnderline = true;
			this.browseButton.Label = global::Mono.Unix.Catalog.GetString ("Browse...");
			this.vbox4.Add (this.browseButton);
			global::Gtk.Box.BoxChild w36 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.browseButton]));
			w36.Position = 0;
			w36.Expand = false;
			w36.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.removeLibButton = new global::Gtk.Button ();
			this.removeLibButton.Sensitive = false;
			this.removeLibButton.CanFocus = true;
			this.removeLibButton.Name = "removeLibButton";
			this.removeLibButton.UseUnderline = true;
			this.removeLibButton.Label = global::Mono.Unix.Catalog.GetString ("Remove");
			this.vbox4.Add (this.removeLibButton);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.removeLibButton]));
			w37.Position = 1;
			w37.Expand = false;
			w37.Fill = false;
			this.table2.Add (this.vbox4);
			global::Gtk.Table.TableChild w38 = ((global::Gtk.Table.TableChild)(this.table2 [this.vbox4]));
			w38.TopAttach = ((uint)(1));
			w38.BottomAttach = ((uint)(2));
			w38.LeftAttach = ((uint)(2));
			w38.RightAttach = ((uint)(3));
			w38.XOptions = ((global::Gtk.AttachOptions)(4));
			this.notebook1.Add (this.table2);
			global::Gtk.Notebook.NotebookChild w39 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.table2]));
			w39.Position = 1;
			// Notebook tab
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Libraries");
			this.notebook1.SetTabLabel (this.table2, this.label2);
			this.label2.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.vbox7 = new global::Gtk.VBox ();
			this.vbox7.Name = "vbox7";
			this.vbox7.Spacing = 6;
			this.vbox7.BorderWidth = ((uint)(3));
			// Container child vbox7.Gtk.Box+BoxChild
			this.table3 = new global::Gtk.Table (((uint)(2)), ((uint)(3)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(10));
			this.table3.ColumnSpacing = ((uint)(10));
			// Container child table3.Gtk.Table+TableChild
			this.includePathAddButton = new global::Gtk.Button ();
			this.includePathAddButton.Sensitive = false;
			this.includePathAddButton.CanFocus = true;
			this.includePathAddButton.Name = "includePathAddButton";
			this.includePathAddButton.UseUnderline = true;
			this.includePathAddButton.Label = global::Mono.Unix.Catalog.GetString ("Add");
			this.table3.Add (this.includePathAddButton);
			global::Gtk.Table.TableChild w40 = ((global::Gtk.Table.TableChild)(this.table3 [this.includePathAddButton]));
			w40.LeftAttach = ((uint)(2));
			w40.RightAttach = ((uint)(3));
			w40.XOptions = ((global::Gtk.AttachOptions)(4));
			w40.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.includePathEntry = new global::Gtk.Entry ();
			this.includePathEntry.CanFocus = true;
			this.includePathEntry.Name = "includePathEntry";
			this.includePathEntry.IsEditable = true;
			this.includePathEntry.InvisibleChar = '●';
			this.table3.Add (this.includePathEntry);
			global::Gtk.Table.TableChild w41 = ((global::Gtk.Table.TableChild)(this.table3 [this.includePathEntry]));
			w41.LeftAttach = ((uint)(1));
			w41.RightAttach = ((uint)(2));
			w41.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Vapi Paths:");
			this.table3.Add (this.label9);
			global::Gtk.Table.TableChild w42 = ((global::Gtk.Table.TableChild)(this.table3 [this.label9]));
			w42.XOptions = ((global::Gtk.AttachOptions)(4));
			w42.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.scrolledwindow2 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow2.CanFocus = true;
			this.scrolledwindow2.Name = "scrolledwindow2";
			this.scrolledwindow2.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child scrolledwindow2.Gtk.Container+ContainerChild
			this.includePathTreeView = new global::Gtk.TreeView ();
			this.includePathTreeView.CanFocus = true;
			this.includePathTreeView.Name = "includePathTreeView";
			this.scrolledwindow2.Add (this.includePathTreeView);
			this.table3.Add (this.scrolledwindow2);
			global::Gtk.Table.TableChild w44 = ((global::Gtk.Table.TableChild)(this.table3 [this.scrolledwindow2]));
			w44.TopAttach = ((uint)(1));
			w44.BottomAttach = ((uint)(2));
			w44.LeftAttach = ((uint)(1));
			w44.RightAttach = ((uint)(2));
			// Container child table3.Gtk.Table+TableChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			// Container child vbox5.Gtk.Box+BoxChild
			this.includePathBrowseButton = new global::Gtk.Button ();
			this.includePathBrowseButton.CanFocus = true;
			this.includePathBrowseButton.Name = "includePathBrowseButton";
			this.includePathBrowseButton.UseUnderline = true;
			this.includePathBrowseButton.Label = global::Mono.Unix.Catalog.GetString ("Browse...");
			this.vbox5.Add (this.includePathBrowseButton);
			global::Gtk.Box.BoxChild w45 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.includePathBrowseButton]));
			w45.Position = 0;
			w45.Expand = false;
			w45.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.includePathRemoveButton = new global::Gtk.Button ();
			this.includePathRemoveButton.Sensitive = false;
			this.includePathRemoveButton.CanFocus = true;
			this.includePathRemoveButton.Name = "includePathRemoveButton";
			this.includePathRemoveButton.UseUnderline = true;
			this.includePathRemoveButton.Label = global::Mono.Unix.Catalog.GetString ("Remove");
			this.vbox5.Add (this.includePathRemoveButton);
			global::Gtk.Box.BoxChild w46 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.includePathRemoveButton]));
			w46.Position = 1;
			w46.Expand = false;
			w46.Fill = false;
			this.table3.Add (this.vbox5);
			global::Gtk.Table.TableChild w47 = ((global::Gtk.Table.TableChild)(this.table3 [this.vbox5]));
			w47.TopAttach = ((uint)(1));
			w47.BottomAttach = ((uint)(2));
			w47.LeftAttach = ((uint)(2));
			w47.RightAttach = ((uint)(3));
			w47.XOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox7.Add (this.table3);
			global::Gtk.Box.BoxChild w48 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.table3]));
			w48.Position = 0;
			this.notebook1.Add (this.vbox7);
			global::Gtk.Notebook.NotebookChild w49 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.vbox7]));
			w49.Position = 2;
			// Notebook tab
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Paths");
			this.notebook1.SetTabLabel (this.vbox7, this.label3);
			this.label3.ShowAll ();
			this.Add (this.notebook1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Show ();
			this.targetComboBox.Changed += new global::System.EventHandler (this.OnTargetComboBoxChanged);
			this.browseButton.Clicked += new global::System.EventHandler (this.OnBrowseButtonClick);
			this.removeLibButton.Clicked += new global::System.EventHandler (this.OnRemoveLibButtonClicked);
			this.removeLibButton.Clicked += new global::System.EventHandler (this.OnLibRemoved);
			this.libTreeView.CursorChanged += new global::System.EventHandler (this.OnLibTreeViewCursorChanged);
			this.libAddEntry.Changed += new global::System.EventHandler (this.OnLibAddEntryChanged);
			this.libAddEntry.Activated += new global::System.EventHandler (this.OnLibAddEntryActivated);
			this.addLibButton.Clicked += new global::System.EventHandler (this.OnLibAdded);
			this.includePathBrowseButton.Clicked += new global::System.EventHandler (this.OnIncludePathBrowseButtonClick);
			this.includePathRemoveButton.Clicked += new global::System.EventHandler (this.OnIncludePathRemoveButtonClicked);
			this.includePathRemoveButton.Clicked += new global::System.EventHandler (this.OnIncludePathRemoved);
			this.includePathTreeView.CursorChanged += new global::System.EventHandler (this.OnIncludePathTreeViewCursorChanged);
			this.includePathEntry.Changed += new global::System.EventHandler (this.OnIncludePathEntryChanged);
			this.includePathEntry.Activated += new global::System.EventHandler (this.OnIncludePathEntryActivated);
			this.includePathAddButton.Clicked += new global::System.EventHandler (this.OnIncludePathAdded);
		}
	}
}
