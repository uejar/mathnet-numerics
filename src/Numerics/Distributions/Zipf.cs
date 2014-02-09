﻿// <copyright file="Zipf.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2013 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Random;

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Discrete Univariate Zipf distribution.
    /// Zipf's law, an empirical law formulated using mathematical statistics, refers to the fact
    /// that many types of data studied in the physical and social sciences can be approximated with
    /// a Zipfian distribution, one of a family of related discrete power law probability distributions.
    /// For details about this distribution, see
    /// <a href="http://en.wikipedia.org/wiki/Zipf%27s_law">Wikipedia - Zipf distribution</a>.
    /// </summary>
    public class Zipf : IDiscreteDistribution
    {
        System.Random _random;

        /// <summary>
        /// The s parameter of the distribution.
        /// </summary>
        double _s;

        /// <summary>
        /// The n parameter of the distribution.
        /// </summary>
        int _n;

        /// <summary>
        /// Initializes a new instance of the <see cref="Zipf"/> class.
        /// </summary>
        /// <param name="s">The s parameter of the distribution.</param>
        /// <param name="n">The n parameter of the distribution.</param>
        public Zipf(double s, int n)
        {
            _random = SystemRandomSource.Default;
            SetParameters(s, n);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Zipf"/> class.
        /// </summary>
        /// <param name="s">The s parameter of the distribution.</param>
        /// <param name="n">The n parameter of the distribution.</param>
        /// <param name="randomSource">The random number generator which is used to draw random samples.</param>
        public Zipf(double s, int n, System.Random randomSource)
        {
            _random = randomSource ?? SystemRandomSource.Default;
            SetParameters(s, n);
        }

        /// <summary>
        /// A string representation of the distribution.
        /// </summary>
        /// <returns>a string representation of the distribution.</returns>
        public override string ToString()
        {
            return "Zipf(S = " + _s + ", N = " + _n + ")";
        }

        /// <summary>
        /// Checks whether the parameters of the distribution are valid.
        /// </summary>
        /// <param name="s">The s parameter of the distribution.</param>
        /// <param name="n">The n parameter of the distribution.</param>
        /// <returns><c>true</c> when the parameters are valid, <c>false</c> otherwise.</returns>
        static bool IsValidParameterSet(double s, int n)
        {
            return n > 0 && s > 0.0;
        }

        /// <summary>
        /// Sets the parameters of the distribution after checking their validity.
        /// </summary>
        /// <param name="s">The s parameter of the distribution.</param>
        /// <param name="n">The n parameter of the distribution.</param>
        /// <exception cref="ArgumentOutOfRangeException">When the parameters are out of range.</exception>
        void SetParameters(double s, int n)
        {
            if (Control.CheckDistributionParameters && !IsValidParameterSet(s, n))
            {
                throw new ArgumentOutOfRangeException(Resources.InvalidDistributionParameters);
            }

            _s = s;
            _n = n;
        }

        /// <summary>
        /// Gets or sets the s parameter of the distribution.
        /// </summary>
        public double S
        {
            get { return _s; }
            set { SetParameters(value, _n); }
        }

        /// <summary>
        /// Gets or sets the n parameter of the distribution.
        /// </summary>
        public int N
        {
            get { return _n; }
            set { SetParameters(_s, value); }
        }

        /// <summary>
        /// Gets or sets the random number generator which is used to draw random samples.
        /// </summary>
        public System.Random RandomSource
        {
            get { return _random; }
            set { _random = value ?? SystemRandomSource.Default; }
        }

        /// <summary>
        /// Gets the mean of the distribution.
        /// </summary>
        public double Mean
        {
            get { return SpecialFunctions.GeneralHarmonic(_n, _s - 1.0)/SpecialFunctions.GeneralHarmonic(_n, _s); }
        }

        /// <summary>
        /// Gets the variance of the distribution.
        /// </summary>
        public double Variance
        {
            get
            {
                if (_s <= 3)
                {
                    throw new NotSupportedException();
                }

                var ghns = SpecialFunctions.GeneralHarmonic(_n, _s);
                return (SpecialFunctions.GeneralHarmonic(_n, _s - 2)*SpecialFunctions.GeneralHarmonic(_n, _s))
                       - (Math.Pow(SpecialFunctions.GeneralHarmonic(_n, _s - 1), 2)/(ghns*ghns));
            }
        }

        /// <summary>
        /// Gets the standard deviation of the distribution.
        /// </summary>
        public double StdDev
        {
            get { return Math.Sqrt(Variance); }
        }

        /// <summary>
        /// Gets the entropy of the distribution.
        /// </summary>
        public double Entropy
        {
            get
            {
                double sum = 0;
                for (var i = 0; i < _n; i++)
                {
                    sum += Math.Log(i + 1)/Math.Pow(i + 1, _s);
                }

                return ((_s/SpecialFunctions.GeneralHarmonic(_n, _s))*sum) + Math.Log(SpecialFunctions.GeneralHarmonic(_n, _s));
            }
        }

        /// <summary>
        /// Gets the skewness of the distribution.
        /// </summary>
        public double Skewness
        {
            get
            {
                if (_s <= 4)
                {
                    throw new NotSupportedException();
                }

                return ((SpecialFunctions.GeneralHarmonic(_n, _s - 3)*Math.Pow(SpecialFunctions.GeneralHarmonic(_n, _s), 2)) - (SpecialFunctions.GeneralHarmonic(_n, _s - 1)*((3*SpecialFunctions.GeneralHarmonic(_n, _s - 2)*SpecialFunctions.GeneralHarmonic(_n, _s)) - Math.Pow(SpecialFunctions.GeneralHarmonic(_n, _s - 1), 2))))/Math.Pow((SpecialFunctions.GeneralHarmonic(_n, _s - 2)*SpecialFunctions.GeneralHarmonic(_n, _s)) - Math.Pow(SpecialFunctions.GeneralHarmonic(_n, _s - 1), 2), 1.5);
            }
        }

        /// <summary>
        /// Gets the mode of the distribution.
        /// </summary>
        public int Mode
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the median of the distribution.
        /// </summary>
        public int Median
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the smallest element in the domain of the distributions which can be represented by an integer.
        /// </summary>
        public int Minimum
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the largest element in the domain of the distributions which can be represented by an integer.
        /// </summary>
        public int Maximum
        {
            get { return _n; }
        }

        /// <summary>
        /// Computes the probability mass (PMF) at k, i.e. P(X = k).
        /// </summary>
        /// <param name="k">The location in the domain where we want to evaluate the probability mass function.</param>
        /// <returns>the probability mass at location <paramref name="k"/>.</returns>
        public double Probability(int k)
        {
            return (1.0/Math.Pow(k, _s))/SpecialFunctions.GeneralHarmonic(_n, _s);
        }

        /// <summary>
        /// Computes the log probability mass (lnPMF) at k, i.e. ln(P(X = k)).
        /// </summary>
        /// <param name="k">The location in the domain where we want to evaluate the log probability mass function.</param>
        /// <returns>the log probability mass at location <paramref name="k"/>.</returns>
        public double ProbabilityLn(int k)
        {
            return Math.Log(Probability(k));
        }

        /// <summary>
        /// Computes the cumulative distribution (CDF) of the distribution at x, i.e. P(X ≤ x).
        /// </summary>
        /// <param name="x">The location at which to compute the cumulative distribution function.</param>
        /// <returns>the cumulative distribution at location <paramref name="x"/>.</returns>
        public double CumulativeDistribution(double x)
        {
            if (x < 1)
            {
                return 0.0;
            }

            return SpecialFunctions.GeneralHarmonic((int) x, _s)/SpecialFunctions.GeneralHarmonic(_n, _s);
        }

        /// <summary>
        /// Generates a sample from the Zipf distribution without doing parameter checking.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="s">The s parameter of the distribution.</param>
        /// <param name="n">The n parameter of the distribution.</param>
        /// <returns>a random number from the Zipf distribution.</returns>
        static int SampleUnchecked(System.Random rnd, double s, int n)
        {
            var r = 0.0;
            while (r == 0.0)
            {
                r = rnd.NextDouble();
            }

            var p = 1.0/SpecialFunctions.GeneralHarmonic(n, s);
            int i;
            var sum = 0.0;
            for (i = 1; i <= n; i++)
            {
                sum += p/Math.Pow(i, s);
                if (sum >= r)
                {
                    break;
                }
            }

            return i;
        }

        /// <summary>
        /// Draws a random sample from the distribution.
        /// </summary>
        /// <returns>a sample from the distribution.</returns>
        public int Sample()
        {
            return SampleUnchecked(_random, _s, _n);
        }

        /// <summary>
        /// Samples an array of zipf distributed random variables.
        /// </summary>
        /// <returns>a sequence of samples from the distribution.</returns>
        public IEnumerable<int> Samples()
        {
            while (true)
            {
                yield return SampleUnchecked(_random, _s, _n);
            }
        }

        /// <summary>
        /// Samples a random variable.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="s">The s parameter of the distribution.</param>
        /// <param name="n">The n parameter of the distribution.</param>
        public static int Sample(System.Random rnd, double s, int n)
        {
            if (Control.CheckDistributionParameters && !IsValidParameterSet(s, n))
            {
                throw new ArgumentOutOfRangeException(Resources.InvalidDistributionParameters);
            }

            return SampleUnchecked(rnd, s, n);
        }

        /// <summary>
        /// Samples a sequence of this random variable.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="s">The s parameter of the distribution.</param>
        /// <param name="n">The n parameter of the distribution.</param>
        public static IEnumerable<int> Samples(System.Random rnd, double s, int n)
        {
            if (Control.CheckDistributionParameters && !IsValidParameterSet(s, n))
            {
                throw new ArgumentOutOfRangeException(Resources.InvalidDistributionParameters);
            }

            while (true)
            {
                yield return SampleUnchecked(rnd, s, n);
            }
        }
    }
}
