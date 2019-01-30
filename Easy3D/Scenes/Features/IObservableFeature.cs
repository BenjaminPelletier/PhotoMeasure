using Easy3D.Projection;
using Easy3D.Scenes.Observations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy3D.Scenes.Features
{
    public interface IObservableFeature
    {
        double ObservationError(LocatedCamera camera, Observation observation);
    }
}
