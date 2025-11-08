// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;

namespace Core2D.Procedural.WaveFunctionCollapse;

/// <summary>
/// Port of Maxim Gumin's WaveFunctionCollapse model, trimmed to the features Core2D needs.
/// </summary>
internal abstract class WaveFunctionModel
{
    protected bool[][] wave;
    protected int[][][] propagator;
    private int[][][] _compatible;
    protected int[] observed;

    private (int, int)[] _stack;
    private int _stackSize;
    private int _observedSoFar;

    protected readonly int MX;
    protected readonly int MY;
    protected readonly int N;
    protected readonly bool periodic;
    protected bool ground;

    protected double[] weights;
    private double[] _weightLogWeights;
    private double[] _distribution;

    private int[] _sumsOfOnes;
    private double[] _sumsOfWeights;
    private double[] _sumsOfWeightLogWeights;
    private double[] _entropies;
    private double _sumOfWeights;
    private double _sumOfWeightLogWeights;
    private double _startingEntropy;

    protected WaveFunctionHeuristicType HeuristicMode { get; }

    protected WaveFunctionModel(int width, int height, int n, bool periodic, WaveFunctionHeuristicType heuristic)
    {
        MX = width;
        MY = height;
        N = n;
        this.periodic = periodic;
        HeuristicMode = heuristic;
    }

    private void Init()
    {
        wave = new bool[MX * MY][];
        _compatible = new int[wave.Length][][];
        for (var i = 0; i < wave.Length; i++)
        {
            wave[i] = new bool[T];
            _compatible[i] = new int[T][];
            for (var t = 0; t < T; t++)
            {
                _compatible[i][t] = new int[4];
            }
        }

        _distribution = new double[T];
        observed = new int[MX * MY];

        _weightLogWeights = new double[T];
        _sumOfWeights = 0;
        _sumOfWeightLogWeights = 0;

        for (var t = 0; t < T; t++)
        {
            _weightLogWeights[t] = weights[t] * Math.Log(weights[t]);
            _sumOfWeights += weights[t];
            _sumOfWeightLogWeights += _weightLogWeights[t];
        }

        _startingEntropy = Math.Log(_sumOfWeights) - _sumOfWeightLogWeights / _sumOfWeights;

        _sumsOfOnes = new int[MX * MY];
        _sumsOfWeights = new double[MX * MY];
        _sumsOfWeightLogWeights = new double[MX * MY];
        _entropies = new double[MX * MY];

        _stack = new (int, int)[wave.Length * T];
        _stackSize = 0;
    }

    protected int T { get; set; }

    public bool Run(int seed, int limit)
    {
        if (wave == null)
        {
            Init();
        }

        Clear();
        var random = new Random(seed);

        for (var l = 0; l < limit || limit < 0; l++)
        {
            var node = NextUnobservedNode(random);
            if (node >= 0)
            {
                Observe(node, random);
                var success = Propagate();
                if (!success)
                {
                    return false;
                }
            }
            else
            {
                for (var i = 0; i < wave.Length; i++)
                {
                    for (var t = 0; t < T; t++)
                    {
                        if (wave[i][t])
                        {
                            observed[i] = t;
                            break;
                        }
                    }
                }

                return true;
            }
        }

        return true;
    }

    private int NextUnobservedNode(Random random)
    {
        if (HeuristicMode == WaveFunctionHeuristicType.Scanline)
        {
            for (var i = _observedSoFar; i < wave.Length; i++)
            {
                if (!periodic && (i % MX + N > MX || i / MX + N > MY))
                {
                    continue;
                }

                if (_sumsOfOnes[i] > 1)
                {
                    _observedSoFar = i + 1;
                    return i;
                }
            }

            return -1;
        }

        var min = double.MaxValue;
        var argmin = -1;

        for (var i = 0; i < wave.Length; i++)
        {
            if (!periodic && (i % MX + N > MX || i / MX + N > MY))
            {
                continue;
            }

            var remaining = _sumsOfOnes[i];
            var entropy = HeuristicMode == WaveFunctionHeuristicType.Entropy ? _entropies[i] : remaining;

            if (remaining > 1 && entropy <= min)
            {
                var noise = 1E-6 * random.NextDouble();
                if (entropy + noise < min)
                {
                    min = entropy + noise;
                    argmin = i;
                }
            }
        }

        return argmin;
    }

    private void Observe(int node, Random random)
    {
        var w = wave[node];
        for (var t = 0; t < T; t++)
        {
            _distribution[t] = w[t] ? weights[t] : 0.0;
        }

        var r = RandomFromDistribution(_distribution, random.NextDouble());
        for (var t = 0; t < T; t++)
        {
            if (w[t] != (t == r))
            {
                Ban(node, t);
            }
        }
    }

    private bool Propagate()
    {
        while (_stackSize > 0)
        {
            var (i1, t1) = _stack[_stackSize - 1];
            _stackSize--;

            var x1 = i1 % MX;
            var y1 = i1 / MX;

            for (var d = 0; d < 4; d++)
            {
                var x2 = x1 + dx[d];
                var y2 = y1 + dy[d];

                if (!periodic && (x2 < 0 || y2 < 0 || x2 + N > MX || y2 + N > MY))
                {
                    continue;
                }

                if (x2 < 0)
                {
                    x2 += MX;
                }
                else if (x2 >= MX)
                {
                    x2 -= MX;
                }

                if (y2 < 0)
                {
                    y2 += MY;
                }
                else if (y2 >= MY)
                {
                    y2 -= MY;
                }

                var i2 = x2 + y2 * MX;
                var allowed = propagator[d][t1];
                var compat = _compatible[i2];

                for (var l = 0; l < allowed.Length; l++)
                {
                    var t2 = allowed[l];
                    var entry = compat[t2];

                    entry[d]--;
                    if (entry[d] == 0)
                    {
                        Ban(i2, t2);
                    }
                }
            }
        }

        return _sumsOfOnes[0] > 0;
    }

    private void Ban(int index, int tile)
    {
        if (!wave[index][tile])
        {
            return;
        }

        wave[index][tile] = false;

        var comp = _compatible[index][tile];
        for (var d = 0; d < 4; d++)
        {
            comp[d] = 0;
        }

        _stack[_stackSize] = (index, tile);
        _stackSize++;

        _sumsOfOnes[index] -= 1;
        _sumsOfWeights[index] -= weights[tile];
        _sumsOfWeightLogWeights[index] -= _weightLogWeights[tile];

        var sum = _sumsOfWeights[index];
        if (sum <= 0)
        {
            _entropies[index] = 0;
        }
        else
        {
            _entropies[index] = Math.Log(sum) - _sumsOfWeightLogWeights[index] / sum;
        }
    }

    private void Clear()
    {
        for (var i = 0; i < wave.Length; i++)
        {
            for (var t = 0; t < T; t++)
            {
                wave[i][t] = true;
                for (var d = 0; d < 4; d++)
                {
                    _compatible[i][t][d] = propagator[opposite[d]][t].Length;
                }
            }

            _sumsOfOnes[i] = weights.Length;
            _sumsOfWeights[i] = _sumOfWeights;
            _sumsOfWeightLogWeights[i] = _sumOfWeightLogWeights;
            _entropies[i] = _startingEntropy;
            observed[i] = -1;
        }

        _observedSoFar = 0;

        if (ground)
        {
            for (var x = 0; x < MX; x++)
            {
                for (var t = 0; t < T - 1; t++)
                {
                    Ban(x + (MY - 1) * MX, t);
                }

                for (var y = 0; y < MY - 1; y++)
                {
                    Ban(x + y * MX, T - 1);
                }
            }

            Propagate();
        }
    }

    public ReadOnlySpan<int> Observed => observed;

    public int GetObservedValue(int index) => observed[index];

    public int GetObservedValue(int x, int y) => observed[x + y * MX];

    private static int RandomFromDistribution(double[] values, double roll)
    {
        var total = 0.0;
        for (var i = 0; i < values.Length; i++)
        {
            total += values[i];
        }

        if (total <= 0)
        {
            return 0;
        }

        var target = roll * total;
        var cumulative = 0.0;
        for (var i = 0; i < values.Length; i++)
        {
            cumulative += values[i];
            if (cumulative >= target)
            {
                return i;
            }
        }

        return values.Length - 1;
    }

    protected static readonly int[] dx = { -1, 0, 1, 0 };
    protected static readonly int[] dy = { 0, 1, 0, -1 };
    protected static readonly int[] opposite = { 2, 3, 0, 1 };
}

public enum WaveFunctionHeuristicType
{
    Entropy,
    MostConstrained,
    Scanline
}
