﻿using MMRando.Extensions;
using MMRando.Utils;
using MMRando.GameObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MMRando.Models.Settings;

namespace MMRando.Forms
{
    public partial class JunkLocationEditForm : Form
    {
        private readonly List<Item> _junkLocations;
        private readonly SettingsObject _settings;
        private bool updating = false;
        private const int ItemGroupCount = 13;

        public string ExternalLabel { get; private set; }

        public JunkLocationEditForm(SettingsObject settings)
        {
            InitializeComponent();

            _settings = settings;
            _junkLocations = ItemUtils.AllLocations().ToList();

            foreach (var item in _junkLocations)
            {
                lJunkLocations.Items.Add(item.Location());
            }

            if (_settings.CustomJunkLocations != null)
            {
                UpdateString(_settings.CustomJunkLocations);
                ExternalLabel = $"{_settings.CustomJunkLocations.Count}/{_junkLocations.Count} items selected";
            }
            else
            {
                tJunkLocationsString.Text = "--";
            }
        }

        private void JunkLocationEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void UpdateString(List<Item> selections)
        {
            int[] n = new int[ItemGroupCount];
            string[] ns = new string[ItemGroupCount];
            foreach (var item in selections)
            {
                var i = _junkLocations.IndexOf(item);
                int j = i / 32;
                int k = i % 32;
                n[j] |= (int)(1 << k);
                ns[j] = Convert.ToString(n[j], 16);
            }

            tJunkLocationsString.Text = string.Join("-", ns.Reverse());
            _settings.CustomJunkLocationsString = tJunkLocationsString.Text;
        }

        public void UpdateChecks(string c)
        {
            updating = true;
            try
            {
                tJunkLocationsString.Text = c;
                _settings.CustomJunkLocationsString = c;
                _settings.CustomJunkLocations.Clear();
                string[] v = c.Split('-');
                int[] vi = new int[ItemGroupCount];
                if (v.Length != vi.Length)
                {
                    ExternalLabel = "Invalid junk locations string";
                    return;
                }
                for (int i = 0; i < ItemGroupCount; i++)
                {
                    if (v[ItemGroupCount - 1 - i] != "")
                    {
                        vi[i] = Convert.ToInt32(v[ItemGroupCount - 1 - i], 16);
                    }
                }
                for (int i = 0; i < 32 * ItemGroupCount; i++)
                {
                    int j = i / 32;
                    int k = i % 32;
                    if (((vi[j] >> k) & 1) > 0)
                    {
                        if (i >= ItemUtils.AllLocations().Count())
                        {
                            throw new IndexOutOfRangeException();
                        }
                        _settings.CustomJunkLocations.Add(_junkLocations[i]);
                    }
                }
                foreach (ListViewItem l in lJunkLocations.Items)
                {
                    if (_settings.CustomJunkLocations.Contains(_junkLocations[l.Index]))
                    {
                        l.Checked = true;
                    }
                    else
                    {
                        l.Checked = false;
                    }
                }
                ExternalLabel = $"{_settings.CustomJunkLocations.Count}/{_junkLocations.Count} locations selected";
            }
            catch
            {
                _settings.CustomJunkLocations.Clear();
                ExternalLabel = "Invalid junk locations string";
            }
            finally
            {
                updating = false;
            }
        }

        private void tJunkLocationsString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateChecks(tJunkLocationsString.Text);
            }
        }

        private void lJunkLocations_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (updating)
            {
                return;
            }
            updating = true;
            if (e.Item.Checked)
            {
                _settings.CustomJunkLocations.Add(_junkLocations[e.Item.Index]);
            }
            else
            {
                _settings.CustomJunkLocations.Remove(_junkLocations[e.Item.Index]);
            }
            UpdateString(_settings.CustomJunkLocations);
            updating = false;
        }
    }
}
