﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TogglDesktop
{
    public class AutoCompleteTextBox : TextBox
    {
        public ListBox autoCompleteListBox;
        private bool _isAdded;
        private String _formerValue = String.Empty;
        private SizeF currentFactor;
        private int defaultItemHeight;

        public AutoCompleteTextBox()
        {
            InitializeComponent();
            defaultItemHeight = autoCompleteListBox.ItemHeight;
        }

        private void InitializeComponent()
        {
            autoCompleteListBox = new ListBox();
            autoCompleteListBox.DrawMode = DrawMode.OwnerDrawFixed;
            autoCompleteListBox.DrawItem += autoCompleteListBox_DrawItem;
            autoCompleteListBox.MouseEnter += autoCompleteListBox_MouseEnter;
            autoCompleteListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom )));
        }

        void autoCompleteListBox_MouseEnter(object sender, EventArgs e)
        {
            autoCompleteListBox.Focus();
        }

        public void scaleList(SizeF factor)
        {
            currentFactor = factor;
        }

        private void autoCompleteListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            autoCompleteListBox.ItemHeight = (int)(defaultItemHeight * currentFactor.Height);
            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.DrawString(
                 autoCompleteListBox.Items[e.Index].ToString(),
                 e.Font,
                 new SolidBrush(e.ForeColor),
                 e.Bounds);
        }

        public void InitListBox()
        {
            if (!_isAdded)
            {
                Parent.Parent.Parent.Parent.Controls.Add(autoCompleteListBox);
                autoCompleteListBox.Left = Left;
                autoCompleteListBox.Top = Top + Height;
                _isAdded = true;
            }
        }

        public void ShowListBox()
        {
            autoCompleteListBox.MaximumSize = new Size(Width + 145, Parent.Parent.Parent.Parent.Height - 40);
            autoCompleteListBox.Visible = true;
            autoCompleteListBox.BringToFront();
        }

        public void ResetListBox()
        {
            autoCompleteListBox.Visible = false;
        }

        public void UpdateListBox(List<Toggl.AutocompleteItem> autoCompleteList)
        {
            if (Text == _formerValue) return;
            _formerValue = Text;
            String word = Text;
            if (autoCompleteList != null && word.Length > 1)
            {
                ResetListBox();
                autoCompleteListBox.Items.Clear();
                foreach (Toggl.AutocompleteItem item in autoCompleteList)
                {
                    if (item.ToString().IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        autoCompleteListBox.Items.Add(item);
                    }
                }
                if (autoCompleteListBox.Items.Count > 0)
                {
                    InitListBox();
                    autoCompleteListBox.SelectedIndex = 0;
                    autoCompleteListBox.Height = 0;
                    autoCompleteListBox.Width = 0;
                    Focus();
                    using (Graphics graphics = autoCompleteListBox.CreateGraphics())
                    {
                        for (int i = 0; i < autoCompleteListBox.Items.Count; i++)
                        {
                            autoCompleteListBox.Height += autoCompleteListBox.GetItemHeight(i);
                            // it item width is larger than the current one
                            // set it to the new max item width
                            // GetItemRectangle does not work for me
                            // we add a little extra space by using '_'
                            int itemWidth = (int)graphics.MeasureString((autoCompleteListBox.Items[i].ToString()) + "_", autoCompleteListBox.Font).Width;
                            autoCompleteListBox.Width = (autoCompleteListBox.Width < itemWidth) ? itemWidth : autoCompleteListBox.Width;
                        }
                    }
                    ShowListBox();
                }
                else
                {
                    ResetListBox();
                }
            }
            else
            {
                ResetListBox();
            }
        }

        public Boolean parseKeyDown(PreviewKeyDownEventArgs e, List<Toggl.AutocompleteItem> autoCompleteList) 
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    {
                        if (autoCompleteList == null)
                        {
                            return false;
                        }
                        if (autoCompleteListBox.Visible)
                        {
                            ResetListBox();
                            _formerValue = Text;
                            return true;
                        }

                        break;
                    }
                case Keys.Escape:
                    {
                        ResetListBox();
                        break;
                    }
                case Keys.Tab:
                    {
                        if (autoCompleteListBox.Visible)
                        {
                            ResetListBox();
                            _formerValue = Text;
                            return true;
                        }
                        break;
                    }
                case Keys.Down:
                    {
                        if ((autoCompleteListBox.Visible) && (autoCompleteListBox.SelectedIndex < autoCompleteListBox.Items.Count - 1))
                            autoCompleteListBox.SelectedIndex++;

                        break;
                    }
                case Keys.Up:
                    {
                        if ((autoCompleteListBox.Visible) && (autoCompleteListBox.SelectedIndex > 0))
                            autoCompleteListBox.SelectedIndex--;

                        break;
                    }
            }
            return false;
        }
    }
}
