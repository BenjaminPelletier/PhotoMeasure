using Easy3D.Scenes;
using Easy3D.Scenes.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoMeasure.UI.Constraints
{
    public class ConstraintValidityEventArgs : EventArgs
    {
        public readonly bool ConstraintValid;

        public ConstraintValidityEventArgs(bool constraintValid)
        {
            this.ConstraintValid = constraintValid;
        }
    }

    interface IConstraintProvider
    {
        Constraint GetConstraint(string name);
        event EventHandler<ConstraintValidityEventArgs> ConstraintValidityChanged;
    }
}
