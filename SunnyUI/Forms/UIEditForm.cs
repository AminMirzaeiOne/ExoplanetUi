﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Exoplanet.UI
{
    public partial class UIEditForm : UIForm
    {
        public UIEditForm()
        {
            InitializeComponent();

            btnOK.Text = UIStyles.CurrentResources.OK;
            btnCancel.Text = UIStyles.CurrentResources.Cancel;
            base.TopMost = true;
        }

        private readonly UIEditOption Option;

        private void InitEditor()
        {
            if (Option == null || Option.Infos.Count == 0) return;

            base.Text = Option.Text;
            int top = 55;

            List<Control> ctrls = new List<Control>();

            if (Option.AutoLabelWidth)
            {
                int size = 0;
                foreach (var info in Option.Infos)
                {
                    Size sf = TextRenderer.MeasureText(info.Text, Font);
                    size = Math.Max(sf.Width, size);
                }

                Option.LabelWidth = size + 1 + 50;
            }

            Width = Option.LabelWidth + Option.ValueWidth + 28;

            foreach (var info in Option.Infos)
            {
                UILabel label = new UILabel();
                label.Text = info.Text;
                label.AutoSize = false;
                label.Left = 5;
                label.Width = Option.LabelWidth - 25;
                label.Height = 29;
                label.Top = top;
                label.TextAlign = ContentAlignment.MiddleRight;
                label.Parent = this;

                Control ctrl = null;

                if (info.EditType == EditType.Text)
                {
                    ctrl = new UITextBox();
                    var edit = (UITextBox)ctrl;
                    edit.Text = info.Value?.ToString();
                    edit.EnterAsTab = true;
                }

                if (info.EditType == EditType.FileSelect)
                {
                    ctrl = new UITextBox();
                    var edit = (UITextBox)ctrl;
                    edit.ShowButton = true;
                    edit.ButtonSymbol = 261788;
                    edit.ButtonSymbolSize = 22;
                    edit.ButtonSymbolOffset = new Point(1, 1);
                    edit.Text = info.Value?.ToString();
                    edit.EnterAsTab = true;
                    edit.ButtonClick += Edit_FileButtonClick;
                }

                if (info.EditType == EditType.DirSelect)
                {
                    ctrl = new UITextBox();
                    var edit = (UITextBox)ctrl;
                    edit.ShowButton = true;
                    edit.ButtonSymbol = 61717;
                    edit.ButtonSymbolOffset = new Point(2, 0);
                    edit.Text = info.Value?.ToString();
                    edit.EnterAsTab = true;
                    edit.ButtonClick += Dir_FileButtonClick;
                }

                if (info.EditType == EditType.Password)
                {
                    ctrl = new UITextBox();
                    var edit = (UITextBox)ctrl;
                    edit.Text = info.Value?.ToString();
                    edit.PasswordChar = '*';
                    edit.EnterAsTab = true;
                }

                if (info.EditType == EditType.Integer)
                {
                    ctrl = new UITextBox();
                    var edit = (UITextBox)ctrl;
                    edit.Type = UITextBox.UIEditType.Integer;
                    edit.IntValue = info.Value.ToString().ToInt();
                    edit.EnterAsTab = true;
                }

                if (info.EditType == EditType.Double)
                {
                    ctrl = new UITextBox();
                    var edit = (UITextBox)ctrl;
                    edit.DecimalPlaces = info.DecimalPlaces;
                    edit.Type = UITextBox.UIEditType.Double;
                    edit.DoubleValue = info.Value.ToString().ToDouble();
                    edit.EnterAsTab = true;
                }

                if (info.EditType == EditType.Date)
                {
                    ctrl = new UIDatePicker();
                    var edit = (UIDatePicker)ctrl;
                    edit.Value = (DateTime)info.Value;
                }

                if (info.EditType == EditType.DateTime)
                {
                    ctrl = new UIDatetimePicker();
                    var edit = (UIDatetimePicker)ctrl;
                    edit.Value = (DateTime)info.Value;
                }

                if (info.EditType == EditType.Switch)
                {
                    ctrl = new UISwitch();
                    var edit = (UISwitch)ctrl;
                    edit.SwitchShape = UISwitch.UISwitchShape.Square;
                    edit.Height = 29;
                    edit.Active = (bool)info.Value;

                    if (info.DataSource != null)
                    {
                        string[] items = (string[])info.DataSource;
                        edit.ActiveText = items[0];
                        edit.InActiveText = items[1];
                        Size sf1 = TextRenderer.MeasureText(items[0], edit.Font);
                        Size sf2 = TextRenderer.MeasureText(items[0], edit.Font);
                        edit.Width = Math.Max(sf1.Width, sf2.Width) + edit.Height + 16;
                    }
                }

                if (info.EditType == EditType.Combobox)
                {
                    ctrl = new UIComboBox();
                    var edit = (UIComboBox)ctrl;
                    edit.DropDownStyle = UIDropDownStyle.DropDownList;

                    if (info.DisplayMember.IsNullOrEmpty())
                    {
                        object[] items = (object[])info.DataSource;
                        if (items != null)
                        {
                            edit.Items.AddRange(items);
                            int index = info.Value.ToString().ToInt();
                            if (index < items.Length)
                                edit.SelectedIndex = index;
                        }
                    }
                    else
                    {
                        edit.DisplayMember = info.DisplayMember;
                        edit.ValueMember = info.ValueMember;
                        edit.DataSource = info.DataSource;
                        edit.SelectedValue = info.Value;
                    }
                }

                if (info.EditType == EditType.ComboTreeView)
                {
                    ctrl = new UIComboTreeView();
                    var edit = (UIComboTreeView)ctrl;
                    edit.CanSelectRootNode = true;
                    edit.ShowLines = true;
                    edit.DropDownStyle = UIDropDownStyle.DropDownList;
                    edit.TreeView.Nodes.Clear();
                    edit.TreeView.Nodes.AddRange((TreeNode[])info.DataSource);
                    if (info.Value != null)
                    {
                        edit.TreeView.SelectedNode = (TreeNode)info.Value;
                        edit.Text = edit.TreeView.SelectedNode.Text;
                    }
                }

                if (info.EditType == EditType.ComboCheckedListBox)
                {
                    ctrl = new UIComboTreeView();
                    var edit = (UIComboTreeView)ctrl;
                    edit.CanSelectRootNode = true;
                    edit.CheckBoxes = true;
                    edit.DropDownStyle = UIDropDownStyle.DropDownList;
                    edit.TreeView.Nodes.Clear();
                    edit.Text = info.Value?.ToString();
                    var obj = (ComboCheckedListBoxItem[])info.DataSource;
                    foreach (var item in obj)
                    {
                        TreeNode node = edit.TreeView.Nodes.Add(item.Text);
                        node.Tag = item;
                        node.Checked = item.Checked;
                    }
                }

                if (info.EditType == EditType.ComboDataGridView)
                {
                    ctrl = new UIComboDataGridView();
                    var edit = (UIComboDataGridView)ctrl;
                    edit.DataGridView.Init();
                    edit.DataGridView.AutoGenerateColumns = true;
                    var obj = (DataTable)info.DataSource;
                    edit.DataGridView.DataSource = obj;
                    edit.DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    foreach (DataGridViewColumn item in edit.DataGridView.Columns)
                    {
                        item.ReadOnly = true;
                    }

                    edit.SelectIndexChange += Edit_SelectIndexChange;
                    int index = (int)info.Value;
                    if (index >= 0)
                    {
                        edit.DataGridView.SelectedIndex = index;
                        edit.Text = obj.Rows[index][info.DisplayMember].ToString();
                    }
                }

                if (ctrl != null)
                {
                    ctrl.Left = Option.LabelWidth;
                    if (info.EditType != EditType.Switch)
                        ctrl.Width = info.HalfWidth ? Option.ValueWidth / 2 : Option.ValueWidth;
                    ctrl.Top = top;
                    ctrl.Parent = this;
                    ctrl.Name = "Edit_" + info.DataPropertyName;
                    ctrl.Enabled = info.Enabled;
                    ctrls.Add(ctrl);
                }

                top += 29 + 10;
            }

            pnlBtm.BringToFront();
            Height = top + 10 + 55;

            int tabIndex = 0;
            foreach (var ctrl in ctrls)
            {
                ctrl.TabIndex = tabIndex;
                tabIndex++;
            }

            pnlBtm.TabIndex = tabIndex;
            tabIndex++;
            btnOK.TabIndex = tabIndex;
            tabIndex++;
            btnCancel.TabIndex = tabIndex;
            btnOK.ShowFocusLine = btnCancel.ShowFocusLine = true;
        }

        private void Edit_FileButtonClick(object sender, EventArgs e)
        {
            UITextBox edit = (UITextBox)sender;
            string filename = edit.Text;
            var info = Option.Dictionary[edit.Name.Replace("Edit_", "")];
            if (Dialogs.OpenFileDialog(ref filename, info.DisplayMember, info.ValueMember))
            {
                edit.Text = filename;
            }
        }

        private void Dir_FileButtonClick(object sender, EventArgs e)
        {
            UITextBox edit = (UITextBox)sender;
            string dirname = edit.Text;
            var info = Option.Dictionary[edit.Name.Replace("Edit_", "")];
            if (Dialogs.SelectDirEx(info.DisplayMember, ref dirname))
            {
                edit.Text = dirname;
            }
        }

        private void Edit_SelectIndexChange(object sender, int index)
        {
            UIComboDataGridView edit = (UIComboDataGridView)sender;
            var info = Option.Dictionary[edit.Name.Replace("Edit_", "")];
            var obj = (DataTable)info.DataSource;
            edit.Text = obj.Rows[index][info.DisplayMember].ToString();
        }

        public UIEditForm(UIEditOption option)
        {
            InitializeComponent();

            btnOK.Text = UIStyles.CurrentResources.OK;
            btnCancel.Text = UIStyles.CurrentResources.Cancel;
            base.TopMost = true;

            Option = option;
            InitEditor();
        }

        public bool ExistsDataPropertyName(string dataPropertyName) => Option != null && Option.ExistsDataPropertyName(dataPropertyName);

        public object this[string dataPropertyName]
        {
            get
            {
                if (Option == null)
                {
                    throw new ArgumentNullException();
                }

                if (!Option.Dictionary.ContainsKey(dataPropertyName))
                {
                    throw new ArgumentOutOfRangeException();
                }

                return Option.Dictionary[dataPropertyName].Value;
            }
        }

        public bool IsOK { get; protected set; }

        [Category("ExoplanetUI"), Description("OK button click event")]
        public event EventHandler ButtonOkClick;

        [Category("ExoplanetUI"), Description("Cancel button click event")]
        public event EventHandler ButtonCancelClick;

        [Description("OK button enabled state"), Category("ExoplanetUI")]
        [DefaultValue(true)]
        public bool ButtonOKEnabled
        {
            get => btnOK.Enabled;
            set => btnOK.Enabled = value;
        }

        [Description("Cancel button enabled state"), Category("ExoplanetUI")]
        [DefaultValue(true)]

        public bool ButtonCancelEnabled
        {
            get => btnCancel.Enabled;
            set => btnCancel.Enabled = value;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckData())
            {
                return;
            }
            else
            {
                DialogResult = DialogResult.OK;
                IsOK = true;
            }

            if (CheckedData != null)
            {
                if (!CheckedData.Invoke(this, new EditFormEventArgs(this)))
                {
                    DialogResult = DialogResult.None;
                    IsOK = false;
                    return;
                }
                else
                {
                    DialogResult = DialogResult.OK;
                    IsOK = true;
                }
            }

            if (ButtonOkClick != null)
            {
                DialogResult = DialogResult.None;
                IsOK = false;
                ButtonOkClick.Invoke(sender, e);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (ButtonCancelClick != null)
            {
                ButtonCancelClick.Invoke(sender, e);
            }
            else
            {
                DialogResult = DialogResult.Cancel;
                IsOK = false;
                Close();
            }
        }

        public void SetEditorFocus(string dataPropertyName)
        {
            Control editor = this.GetControl<UITextBox>("Edit_" + dataPropertyName);
            if (editor != null)
                editor.Focus();
        }

        protected virtual bool CheckData()
        {
            if (Option != null)
            {
                foreach (var info in Option.Infos)
                {
                    if (info.EditType == EditType.Text || info.EditType == EditType.Password ||
                        info.EditType == EditType.FileSelect || info.EditType == EditType.DirSelect)
                    {
                        UITextBox edit = this.GetControl<UITextBox>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;

                        if (info.CheckEmpty && edit.Text.IsNullOrEmpty())
                        {
                            this.ShowWarningTip(edit, info.Text + Environment.NewLine + UIStyles.CurrentResources.EditorCantEmpty);
                            edit.Focus();
                            return false;
                        }

                        info.Value = edit.Text;
                    }

                    if (info.EditType == EditType.Integer)
                    {
                        UITextBox edit = this.GetControl<UITextBox>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;
                        info.Value = edit.IntValue;
                    }

                    if (info.EditType == EditType.Double)
                    {
                        UITextBox edit = this.GetControl<UITextBox>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;
                        info.Value = edit.DoubleValue;
                    }

                    if (info.EditType == EditType.Date)
                    {
                        UIDatePicker edit = this.GetControl<UIDatePicker>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;
                        info.Value = edit.Value.Date;
                    }

                    if (info.EditType == EditType.DateTime)
                    {
                        UIDatetimePicker edit = this.GetControl<UIDatetimePicker>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;
                        info.Value = edit.Value;
                    }

                    if (info.EditType == EditType.Combobox)
                    {
                        UIComboBox edit = this.GetControl<UIComboBox>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;
                        info.Value = edit.ValueMember.IsValid() ? edit.SelectedValue : edit.SelectedIndex;
                    }

                    if (info.EditType == EditType.Switch)
                    {
                        UISwitch edit = this.GetControl<UISwitch>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;
                        info.Value = edit.Active;
                    }

                    if (info.EditType == EditType.ComboTreeView)
                    {
                        UIComboTreeView edit = this.GetControl<UIComboTreeView>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;
                        info.Value = edit.TreeView.SelectedNode;
                    }

                    if (info.EditType == EditType.ComboCheckedListBox)
                    {
                        UIComboTreeView edit = this.GetControl<UIComboTreeView>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;
                        List<ComboCheckedListBoxItem> result = new List<ComboCheckedListBoxItem>();
                        foreach (TreeNode item in edit.Nodes)
                        {
                            ComboCheckedListBoxItem obj = (ComboCheckedListBoxItem)item.Tag;
                            obj.Checked = item.Checked;
                            if (obj.Checked) result.Add(obj);
                        }

                        info.Value = result.ToArray();
                    }

                    if (info.EditType == EditType.ComboDataGridView)
                    {
                        UIComboDataGridView edit = this.GetControl<UIComboDataGridView>("Edit_" + info.DataPropertyName);
                        if (edit == null) continue;
                        info.Value = edit.DataGridView.SelectedIndex;
                    }
                }
            }

            return true;
        }

        public delegate bool OnCheckedData(object sender, EditFormEventArgs e);

        public event OnCheckedData CheckedData;

        public class EditFormEventArgs : EventArgs
        {
            public EditFormEventArgs()
            {

            }

            public EditFormEventArgs(UIEditForm editor)
            {
                Form = editor;
            }

            public UIEditForm Form { get; set; }
        }

        /// <summary>
        /// Override control size change
        /// </summary>
        /// <param name="e">Parameter</param>

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (btnOK != null && btnCancel != null)
            {
                btnCancel.Left = Width - 130;
                btnOK.Left = Width - 245;
            }
        }

        protected bool CheckEmpty(UITextBox edit, string desc)
        {
            bool result = edit.Text.IsValid();
            if (!result)
            {
                this.ShowWarningDialog(desc);
                edit.Focus();
            }

            return result;
        }

        protected bool CheckRange(UITextBox edit, int min, int max, string desc)
        {
            bool result = edit.IntValue >= min && edit.IntValue <= max;
            if (!result)
            {
                this.ShowWarningDialog(desc);
                edit.Focus();
            }

            return result;
        }

        protected bool CheckRange(UITextBox edit, double min, double max, string desc)
        {
            bool result = edit.DoubleValue >= min && edit.DoubleValue <= max;
            if (!result)
            {
                this.ShowWarningDialog(desc);
                edit.Focus();
            }

            return result;
        }

        protected bool CheckEmpty(UIComboBox edit, string desc)
        {
            bool result = edit.Text.IsValid();
            if (!result)
            {
                this.ShowWarningDialog(desc);
                edit.Focus();
            }

            return result;
        }

        protected bool CheckEmpty(UIDatePicker edit, string desc)
        {
            bool result = edit.Text.IsValid();
            if (!result)
            {
                this.ShowWarningDialog(desc);
                edit.Focus();
            }

            return result;
        }
    }
}