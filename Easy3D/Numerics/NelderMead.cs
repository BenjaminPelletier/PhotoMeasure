using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Easy3D.Numerics
{
    public class NelderMeadConfiguration
    {
        /// <summary>
        /// Reflection coefficient: determines how far across the center of the simplex the worst point is reflected to perform a new function evaluation
        /// Reasonable range: (0, Infinity)
        /// A value of 0 moves the worst point to the center of the remaining simplex every reflection step
        /// A value of less than 1 performs a contraction concurrently with every reflection step
        /// A value of 1 performs a standard reflection of the worst point to the other side of the remaining simplex
        /// A value of greater than 1 performs an expansion concurrently with every reflection step
        /// </summary>
        public double Alpha = 1;

        /// <summary>
        /// Expansion coefficient: determines how far across the center of the simplex the worst point is reflected when the standard reflection yields the lowest function value yet
        /// Reasonable range: [1, Infinity)
        /// A value of 1 performs no additional expansion beyond a simple reflection when a desirable direction is discovered
        /// </summary>
        public double Gamma = 2;

        /// <summary>
        /// Contraction coefficient: determines how far across the center of the simplex the worst point is reflected when the standard reflection does not yield the lowest function value yet
        /// Reasonable range: (-1, 0)
        /// A value of -1 leaves the worst simplex point where it is during a contraction step
        /// A value of 0 moves the worst simplex point to the center of the remaining simplex during a contraction operation
        /// </summary>
        public double Rho = -0.5;

        /// <summary>
        /// Reduction coefficient: determines how far away from the best point all other points are moved when standard reflection and contraction both fail to produce a function value lower than the worst function value
        /// Reasonable range: (0, 1)
        /// A value of 0 forces all simplex points to the best point in one reduction step
        /// A value of 1 leaves all simplex points unchanged during the reduction operation
        /// </summary>
        public double Sigma = 0.5;

        /// <summary>
        /// Approximate maximum number of times the cost function may be evaluated before the optimization will terminate
        /// </summary>
        public int MaximumFunctionEvaluations = 10000;

        /// <summary>
        /// How often the SimplexIteration event should be called.
        /// </summary>
        public TimeSpan UpdatePeriod = TimeSpan.FromSeconds(1);
    }

    public enum SimplexOperation
    {
        Reflection,
        Expansion,
        Contraction,
        Reduction,
        Complete,
    }

    /// <summary>
    /// Nelder-Mead Simplex solver based on description at http://en.wikipedia.org/wiki/Nelder-Mead_method
    /// </summary>
    /// <remarks></remarks>
    public class NelderMead<TCost> where TCost : IComparable<TCost>
    {
        public NelderMeadConfiguration Config;

        public NelderMead(NelderMeadConfiguration config = null)
        {
            this.Config = config ?? new NelderMeadConfiguration();
        }

        public class SimplexIterationEventArgs : EventArgs
        {
            public readonly SimplexOperation Operation;
            public readonly bool NewBest;
            public readonly bool NewBestSinceLastUpdate;
            public readonly BinaryHeap<SimplexPoint> x;
            public readonly int FunctionCount;
            public readonly Tuple<TCost, TCost> dF;
            public readonly TCost Cost;
            public bool Cancel = false;

            public SimplexIterationEventArgs(BinaryHeap<SimplexPoint> x, SimplexOperation operation, bool newBest, bool newBestSinceLastUpdate, int functionCount, Tuple<TCost, TCost> df, TCost cost)
            {
                this.x = x;
                this.Operation = operation;
                this.NewBest = newBest;
                this.NewBestSinceLastUpdate = newBestSinceLastUpdate;
                this.FunctionCount = functionCount;
                this.dF = df;
                this.Cost = cost;
            }
        }

        public event EventHandler<SimplexIterationEventArgs> SimplexIteration;

        public class SimplexPoint : IComparable<SimplexPoint>
        {

            public double[] x;

            public TCost fx;
            public SimplexPoint(double[] x, TCost fx)
            {
                this.x = x;
                this.fx = fx;
            }

            public int CompareTo(SimplexPoint other)
            {
                return this.fx.CompareTo(other.fx);
            }

            public override string ToString()
            {
                string s = fx + " (";
                foreach (double xi in x)
                {
                    s += xi + ", ";
                }
                return s.Substring(0, s.Length - 2) + ")";
            }
        }

        public double[] FindMinimum(Func<double[], TCost> f, double[] initialvalues, double initialsimplexsidelengths = 1)
        {
            double[] initialsimplexsides = new double[initialvalues.Length];
            for (int i = 0; i <= initialsimplexsides.Length - 1; i++)
            {
                initialsimplexsides[i] = initialsimplexsidelengths;
            }
            return FindMinimum(f, initialvalues, initialsimplexsides, CancellationToken.None);
        }

        public double[] FindMinimum(Func<double[], TCost> f, double[] initialvalues, double[] initialsimplexsides)
        {
            return FindMinimum(f, initialvalues, initialsimplexsides, CancellationToken.None);
        }

        public double[] FindMinimum(Func<double[], TCost> f, double[] initialvalues, double[] initialsimplexsides, CancellationToken token)
        {
            var x = new BinaryHeap<SimplexPoint>(); //Collection of all simplex points; point with highest function value is always quickly [O(log(n))] available
            SimplexPoint x1 = null; //Always points to member of x with lowest function value (best simplex point)
            int n = initialvalues.Length; //Number of optimization dimensions (there are n+1 simplex points)
            int fcount = 0; //Cumulative number of function evaluations performed so far
            DateTime nextupdate = DateTime.UtcNow + Config.UpdatePeriod;
            bool newBestSinceLastUpdate = false;

            //Create initial simplex
            SimplexPoint newsp = new SimplexPoint(initialvalues.Duplicate(), f(initialvalues));
            x.Push(newsp);
            x1 = newsp;
            double[] newpoint = null;
            for (int d = 0; d < n; d++)
            {
                newpoint = initialvalues.Duplicate();
                newpoint[d] += initialsimplexsides[d];
                newsp = new SimplexPoint(newpoint, f(newpoint));
                x.Push(newsp);
                if (newsp.fx.CompareTo(x1.fx) < 0)
                    x1 = newsp;
            }
            fcount = n + 1;

            //Simplex improvement loop
            SimplexPoint xr = new SimplexPoint(initialvalues.Duplicate(), default(TCost));
            SimplexPoint xe = new SimplexPoint(initialvalues.Duplicate(), default(TCost));
            SimplexPoint xc = new SimplexPoint(initialvalues.Duplicate(), default(TCost));
            SimplexPoint xi = null;
            double[] x0 = new double[initialvalues.Length];

            do
            {
                Tuple<TCost, TCost> df = null; //How much we changed the cost of the affected point during this iteration
                SimplexOperation operation; //The operation we performed on the simplex during this iteration
                bool newBest = false;

                //Grab the worst (xn1) and second worst (xn) simplex points, and remove the worst point from the simplex
                SimplexPoint xn1 = x.Pop();
                SimplexPoint xn = x.Peek();

                //Compute the center of gravity of all simplex points except xn1 (which has already been removed from x)
                //TODO: this can be improved for speed
                for (int d = 0; d <= n - 1; d++)
                {
                    x0[d] = 0;
                    for (int i = 0; i <= n - 1; i++)
                    {
                        x0[d] += x[i].x[d];
                    }
                    x0[d] /= n;
                }

                //Reflection operation
                for (int d = 0; d <= n - 1; d++)
                {
                    xr.x[d] = x0[d] + Config.Alpha * (x0[d] - xn1.x[d]);
                }
                xr.fx = f(xr.x);

                if (xr.fx.CompareTo(x1.fx) < 0)
                {
                    //The reflected point is the best so far; perform expansion
                    newBest = true;
                    for (int d = 0; d <= n - 1; d++)
                    {
                        xe.x[d] = x0[d] + Config.Gamma * (x0[d] - xn1.x[d]);
                    }
                    xe.fx = f(xe.x);
                    fcount += 1;

                    if (xe.fx.CompareTo(xr.fx) < 0)
                    {
                        x.Push(xe); //Expanded point is better than reflected point; use it to replace worst point
                        operation = SimplexOperation.Expansion;
                        x1 = xe; //Update the best simplex point
                        df = new Tuple<TCost, TCost>(xe.fx, xn1.fx); //Record how much we improved this simplex point's function value this iteration
                        xe = xn1; //Reuse existing SimplexPoint object
                    }
                    else
                    {
                        x.Push(xr); //Expanded point isn't better than reflected point; use reflected point to replace worst point
                        operation = SimplexOperation.Reflection;
                        x1 = xr; //Update the best simplex point
                        df = new Tuple<TCost, TCost>(xr.fx, xn1.fx); //Record how much we improved this simplex point's function value this iteration
                        xr = xn1; //Reuse existing SimplexPoint object
                    }
                }
                else if (xr.fx.CompareTo(xn.fx) >= 0)
                {
                    //The reflected point isn't better than the second-worst; perform contraction
                    for (int d = 0; d <= n - 1; d++)
                    {
                        xc.x[d] = x0[d] + Config.Rho * (x0[d] - xn1.x[d]);
                    }
                    xc.fx = f(xc.x);
                    fcount += 1;

                    if (xc.fx.CompareTo(xn1.fx) < 0)
                    {
                        x.Push(xc); //Contracted point is better than the worst point; replace worst point with contracted point
                        operation = SimplexOperation.Contraction;
                        if (xc.fx.CompareTo(x1.fx) < 0)
                            x1 = xc; //Update the best simplex point if appropriate
                        df = new Tuple<TCost, TCost>(xc.fx, xn1.fx); //Record how much we improved this simplex point's cost this iteration
                        xc = xn1; //Reuse existing SimplexPoint object
                    }
                    else
                    {
                        //Reduction operation; move all points toward the best point
                        operation = SimplexOperation.Reduction;
                        x.Push(xn1); //Put the worst simplex point back in the simplex
                        SimplexPoint newx1 = x1;
                        for (int i = 0; i <= n; i++)
                        {
                            xi = x[i];
                            if (!object.ReferenceEquals(xi, x1))
                            {
                                for (int d = 0; d <= n - 1; d++)
                                {
                                    xi.x[d] = x0[d] + Config.Sigma * (x0[d] - xn1.x[d]);
                                }
                                xi.fx = f(xi.x);
                                if (xi.fx.CompareTo(newx1.fx) < 0)
                                    newx1 = xi;
                            }
                        }
                        x1 = newx1;
                        fcount += n;
                    }
                }
                else
                {
                    x.Push(xr); //Simple reflection improves this point (xn1) but doesn't make it the new best point; use the reflection as the new simplex point
                    operation = SimplexOperation.Reflection;
                    xr = xn1; //Reuse existing SimplexPoint object
                }

                if (newBest)
                {
                    newBestSinceLastUpdate = true;
                }

                if (this.SimplexIteration != null)
                {
                    if (DateTime.UtcNow > nextupdate)
                    {
                        var e = new SimplexIterationEventArgs(x, operation, newBest, newBestSinceLastUpdate, fcount, df, x.Peek().fx);
                        newBestSinceLastUpdate = false;
                        this.SimplexIteration?.Invoke(this, e);
                        if (e.Cancel)
                        {
                            break;
                        }
                        if (Config.UpdatePeriod > TimeSpan.Zero)
                        {
                            nextupdate = nextupdate.AddSeconds((Math.Ceiling((DateTime.UtcNow - nextupdate).TotalSeconds / Config.UpdatePeriod.TotalSeconds)) * Config.UpdatePeriod.TotalSeconds);
                        }
                    }
                }
            } while (!(fcount >= Config.MaximumFunctionEvaluations));

            if (this.SimplexIteration != null)
            {
                var e = new SimplexIterationEventArgs(x, SimplexOperation.Complete, false, newBestSinceLastUpdate, fcount, null, x1.fx);
                this.SimplexIteration?.Invoke(this, e);
            }

            return x1.x;
        }
    }


    public class BinaryHeap<T> where T : IComparable<T>
    {
        private List<T> Items = new List<T>();

        /// <summary>
        /// Get a count of the number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the BinaryHeap,
        /// if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            Items.TrimExcess();
        }

        public T this[int i]
        {
            get { return Items[i]; }
        }

        /// <summary>
        /// Pushes an item into the heap.
        /// </summary>
        /// <param name="newItem">The item to be inserted.</param>
        public void Push(T newItem)
        {
            int i = Count;
            Items.Add(newItem);
            while (i > 0 && Items[(i - 1) / 2].CompareTo(newItem) < 0)
            {
                Items[i] = Items[(i - 1) / 2];
                i = (i - 1) / 2;
            }
            Items[i] = newItem;
        }

        /// <summary>
        /// Return the root item from the collection, without removing it.
        /// </summary>
        /// <returns>Returns the item at the root of the heap.</returns>
        public T Peek()
        {
            if (Items.Count == 0)
                throw new InvalidOperationException("The heap is empty.");
            return Items[0];
        }

        /// <summary>
        /// Removes and returns the root item from the collection.
        /// </summary>
        /// <returns>Returns the item at the root of the heap.</returns>
        public T Pop()
        {
            if (Items.Count == 0)
                throw new InvalidOperationException("The heap is empty.");

            // Get the first item
            T result = Items[0];

            // Get the last item and bubble it down.
            T oldbottom = Items[Items.Count - 1];
            Items.RemoveAt(Items.Count - 1);
            if (Items.Count > 0)
            {
                int parent = 0;
                while (parent < Items.Count / 2)
                {
                    int child = parent + parent + 1;
                    if ((child < Items.Count - 1) && (Items[child].CompareTo(Items[child + 1]) < 0))
                        child += 1;
                    if (Items[child].CompareTo(oldbottom) <= 0)
                        break;
                    Items[parent] = Items[child];
                    parent = child;
                }
                Items[parent] = oldbottom;
            }
            return result;
        }
    }


    static class NelderMeadExtensions
    {
        public static double[] Duplicate(this double[] values)
        {
            var result = new double[values.Length];
            Array.Copy(values, result, result.Length);
            return result;
        }
    }
}
