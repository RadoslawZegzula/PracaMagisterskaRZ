using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Reports;
using System.Globalization;
using BenchmarkDotNet.Columns;
using Perfolizer.Horology;
using BenchmarkDotNet.Exporters;

[DisassemblyDiagnoser(maxDepth: 1)]
[HideColumns(Column.Job, Column.Ratio, Column.RatioSD, Column.StdDev, Column.Median)]
[RPlotExporter]
public class Program
{
    private int _wartosc = 12345;
    private byte[] _bufer = new byte[1000];
    private int wartosc7 = 7;
    private int wartosc8 = 8;
    private int wartosc123 = 123;
    private char formatD = 'D';
    private byte wartosc3 = 33;

    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, DefaultConfig.Instance
            .WithSummaryStyle(
            new SummaryStyle(CultureInfo.CurrentCulture,
            printUnitsInHeader: false,
            SizeUnit.B,
            TimeUnit.Microsecond,
            printUnitsInContent: true))
            );
    }


    [Benchmark]
    public int ObliczZeWstawianiem()
    {
        return ObliczWartoscZeWstawianiem(wartosc123) * wartosc7;
    }

    private int ObliczWartoscZeWstawianiem(
        int dlugosc)
    {
        return dlugosc 
            * wartosc8;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    [Benchmark]
    public int ObliczBezWstawiania()
    {
        return ObliczWartoscBezWstawiania(wartosc123) * wartosc7;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private int ObliczWartoscBezWstawiania(
        int dlugosc)
    {
        return dlugosc 
            * wartosc8;
    }

    [Benchmark]
    public bool FormatujUTF8() => Utf8Formatter.TryFormat(
        _wartosc,
        _bufer,
        out _,
        new StandardFormat(
            formatD,
            wartosc3));

    private int[] _values = Enumerable.Range(
        0,
        100_000)
        .ToArray();
    
    [Benchmark]
    public int ZnajdzElement() => Znajdz(
        _values,
        99_000);


    private static int Znajdz<T>(
        T[] array,
        T item)
    {
        for (int i = 0; i < array.Length; i++)
            if (EqualityComparer<T>.Default.Equals(array[i], item))
                return i;

        return -1;
    }

    private (int, long, int, long) wartosci1 
        = (
        3,
        11, 
        14, 
        19
        );

    private (int, long, int, long) wartosci2 
        = (
        3,
        11,
        14, 
        19
        );

    [Benchmark]
    public int Porównaj() => 
        wartosci1.CompareTo(wartosci2);


    [Benchmark]
    public int Pobieraniedługości() 
        => ((ITuple)(6, 7, 8)).Length;


    [Benchmark]
    public int Pobieraniedługości2()
    {
        ITuple t = (6, 7, 8);
        Ignore(t);
        return t.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Ignore(
        object o) { }

    private IEnumerator<int> zrodlo = 
        Enumerable.Range(
            0, 
            int.MaxValue)
        .GetEnumerator();

/// <summary>
/// Część druga
/// </summary>
    private FileStream? potokPliku;
    private byte[] wielkosBufora = new byte[1024];
 
    [Params(false, true)]
    public bool CzyAsynchroniczna { get; set; }   
    [Params(1, 4096)]
    public int rozmiarbuforu { get; set; }

    [GlobalSetup]
    public void Ustawiator()
    {
        byte[] danebitowe = new byte[10_000_000];
        new Random(42).NextBytes(danebitowe);

        string sciezka = Path.GetTempFileName();
        File.WriteAllBytes(
            sciezka,
            danebitowe);

        potokPliku = new FileStream(
            sciezka, 
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            rozmiarbuforu,
            CzyAsynchroniczna);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        potokPliku.Dispose();
        File.Delete(potokPliku.Name);
    }

    [Benchmark]
    public void Read()
    {
        potokPliku.Position = 0;
        while (potokPliku.Read(wielkosBufora
#if !NETCOREAPP2_1_OR_GREATER
                , 0, wielkosBufora.Length
#endif
                ) != 0) ;
    }

    [Benchmark]
    public async Task ReadAsync()
    {
        potokPliku.Position = 0;
        while (await potokPliku.ReadAsync(wielkosBufora
#if !NETCOREAPP2_1_OR_GREATER
                , 0, wielkosBufora.Length
#endif
                ) != 0) ;
    }


 
   private static string zawartosci = string.Concat(
       Enumerable.Range(0, 100_000)
       .Select(i => (char)('a' + (i % 26))));
   private static string sciezka = Path.GetRandomFileName();

    [Benchmark]
    public void Zapisuj() => File.WriteAllText(
        sciezka,
        zawartosci);
    
    private char[] tablica = new char[1280];
    private char znak_C = 'c';

    [Benchmark(Baseline = true)]
    public void CzyszczenieTablicy() => Array.Clear(
        tablica, 
        0, 
        tablica.Length);

    [Benchmark]
    public void WypełnijKolekcje() => tablica
        .AsSpan()
        .Fill(
        znak_C);

    //[Benchmark]
    //public void PiszDoPlikuAsynchornicznie() => File.WriteAllTextAsync(
    //sciezka,
    //zawartosci);
    //[Benchmark]
    //public void WypełnijKolekcje2() => Array.Fill(
    //tablica,
    //znak_C);
}
