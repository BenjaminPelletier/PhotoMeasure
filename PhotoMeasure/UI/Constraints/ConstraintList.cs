using Easy3D.Projection;
using Easy3D.Scenes;
using Easy3D.Scenes.Constraints;
using Easy3D.Scenes.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PhotoMeasure.UI.Constraints
{
    public partial class ConstraintList : UserControl
    {
        private List<Feature> _Features = new List<Feature>();

        public class ConstraintsSelectedEventArgs : EventArgs
        {
            public readonly Constraint[] SelectedConstraints;

            public ConstraintsSelectedEventArgs(IEnumerable<Constraint> Constraints)
            {
                this.SelectedConstraints = Constraints.ToArray();
            }
        }

        private ConstraintDialog cdConstraint = new ConstraintDialog();

        public event EventHandler<ConstraintsSelectedEventArgs> ConstraintsSelected;

        public ConstraintList()
        {
            InitializeComponent();
        }

        public IEnumerable<Feature> Features
        {
            set
            {
                _Features = value.ToList();
            }
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            cdConstraint.Features = _Features;
            if (cdConstraint.ShowDialog(this) == DialogResult.OK)
            {
                AddConstraint(cdConstraint.Constraint);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Constraint> Constraints
        {
            get
            {
                var result = new List<Constraint>();
                foreach (ListViewItem item in lvConstraints.Items)
                {
                    result.Add(item.Tag as Constraint);
                }
                return result;
            }
            set
            {
                lvConstraints.Items.Clear();
                if (value != null)
                {
                    foreach (Constraint constraint in value)
                    {
                        AddConstraint(constraint);
                    }
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<Constraint> SelectedConstraints
        {
            get
            {
                var alreadyReturned = new HashSet<ListViewItem>();
                foreach (ListViewItem item in lvConstraints.Items)
                {
                    if (item.Selected && item.Focused)
                    {
                        yield return item.Tag as Constraint;
                        alreadyReturned.Add(item);
                    }
                }
                foreach (ListViewItem item in lvConstraints.Items)
                {
                    if (item.Selected && !alreadyReturned.Contains(item))
                    {
                        yield return item.Tag as Constraint;
                    }
                }
            }
            set
            {
                var selected = value.ToDictionary(f => f, f => true);
                foreach (ListViewItem item in lvConstraints.Items)
                {
                    item.Selected = selected.ContainsKey(item.Tag as Constraint);
                }
            }
        }

        private void AddConstraint(Constraint Constraint)
        {
            var item = new ListViewItem(Constraint.Name);
            item.Tag = Constraint;
            lvConstraints.Items.Add(item);
        }

        public Dictionary<string, Constraint> ConstraintsByName
        {
            get
            {
                var result = new Dictionary<string, Constraint>();
                foreach (ListViewItem item in lvConstraints.Items)
                {
                    var Constraint = item.Tag as Constraint;
                    result[Constraint.Name] = Constraint;
                }
                return result;
            }
        }

        private void lvConstraints_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            e.Item.BackColor = e.IsSelected ? System.Drawing.Color.LightGray : lvConstraints.BackColor;
            ConstraintsSelected?.Invoke(this, new ConstraintsSelectedEventArgs(this.SelectedConstraints));
        }

        private void lvConstraints_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Brush bg;
            if (e.Item.Selected)
            {
                bg = Brushes.LightBlue;
            }
            else
            {
                bg = Brushes.White;
            }

            e.Graphics.FillRectangle(bg, e.Bounds);

            if ((e.State & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                e.DrawFocusRectangle();
            }

            e.Item.ForeColor = Color.Black;

            e.DrawText();
        }
    }
}
