using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;

[DisassemblyDiagnoser(maxDepth: 0)]
public class Program
{
    private int _wartosc = 12345;
    private byte[] _bufer = new byte[100];

    public static void Main(string[] args) =>
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, DefaultConfig.Instance
            //.WithSummaryStyle(new SummaryStyle(CultureInfo.InvariantCulture, printUnitsInHeader: false, SizeUnit.B, TimeUnit.Microsecond))
            );


    [Benchmark]
    public int ObliczZeWstawianiem() => ObliczWartoscZeWstawianiem(123) * 11;

    private int ObliczWartoscZeWstawianiem(int length) => length * 7;

    [MethodImpl(MethodImplOptions.NoInlining)]
    [Benchmark]
    public int ObliczBezWstawiania() => ObliczWartoscBezWstawiania(123) * 11;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private int ObliczWartoscBezWstawiania(int length) => length * 7;

    [Benchmark]
    public bool FormatujUTF8() => Utf8Formatter.TryFormat(
        _wartosc,
        _bufer,
        out _,
        new StandardFormat('D', 3));
}
