﻿using System;
using System.Collections.Generic;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace Benchmark
{
    public class FFT
    {
        readonly Dictionary<int, Complex[]> _data = new Dictionary<int, Complex[]>();

        [Params(64, 65, 4096, 4097, 65536, 65537, 1048576, 1048577)]
        public int N { get; set; }

        [Setup]
        public void Setup()
        {
            var realSinusoidal = Generate.Sinusoidal(1048577, 32, -2.0, 2.0);
            var imagSawtooth = Generate.Sawtooth(1048577, 32, -20.0, 20.0);
            var signal = Generate.Map2(realSinusoidal, imagSawtooth, (r, i) => new Complex(r, i));
            foreach (var n in new[] { 64, 65, 4096, 4097, 65536, 65537, 1048576, 1048577 })
            {
                var s = new Complex[n];
                Array.Copy(signal, 0, s, 0, n);
                _data[n] = s;
            }

            Control.NativeProviderPath = @"C:\Triage\NATIVE-Win\";
            Control.UseNativeMKL();
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = 2)]
        public void Managed()
        {
            Fourier.BluesteinForward(_data[N], FourierOptions.NoScaling);
            Fourier.BluesteinInverse(_data[N], FourierOptions.NoScaling);
        }

        [Benchmark(OperationsPerInvoke = 2)]
        public void NativeMKL()
        {
            Fourier.Forward(_data[N], FourierOptions.NoScaling);
            Fourier.Inverse(_data[N], FourierOptions.NoScaling);
        }
    }
}
