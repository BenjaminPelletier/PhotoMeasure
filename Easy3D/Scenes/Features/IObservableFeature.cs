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
        /// <summary>
        /// Compute the error between the expected projected location of Feature in a LocatedCamera and an actual Observation of that feature in that camera.
        /// </summary>
        /// <param name="camera">The camera through which the Feature is being observed.</param>
        /// <param name="observation">Where the Feature was actually observed using that camera.</param>
        /// <returns>Pixel distance between expected and actual Feature location in the provided camera.</returns>
        double ObservationError(LocatedCamera camera, Observation observation);
    }
}
