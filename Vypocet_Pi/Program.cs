using System.Diagnostics;
class Program
{
    private static int NumberOfCirclePoints = 0;
    private static Random RandomObject = new Random();

    static decimal PI;
    static decimal radius;
    static string center;
    static long numberOfPoints;
    static int numberOfIterations;
    static decimal scx, scy, cx, cy;


    private static decimal GenerateRandomDecimal(decimal min, decimal max)
    {
        decimal d = Convert.ToDecimal(RandomObject.NextDouble()) * (max - min) + min;
        return d;
    }

    static void Main(string[] args)
    {
        Stopwatch W = new Stopwatch();

        Console.WriteLine("----Sériově----");

        W.Start();
        GeneratePI("seriove");
        W.Stop();
        Console.WriteLine(W.ElapsedMilliseconds.ToString() + "ms");

        Console.WriteLine("\n----Paralelně----");
        W.Restart();

        W.Start();
        GeneratePI("paralelne");
        W.Stop();
        Console.WriteLine(W.ElapsedMilliseconds.ToString() + "ms");

    }

    static void GeneratePI(string metodaVlaken)
    {
        radius = 100;
        center = "0,0";
        numberOfPoints = 100000000;
        numberOfIterations = 0;

        string[] centerCoordinates = center.Split(',');

        cx = Convert.ToDecimal(centerCoordinates[0].Trim());
        cy = Convert.ToDecimal(centerCoordinates[1].Trim());

        scx = scy = cx + radius;

        if (metodaVlaken == "seriove")
        {
            Parallel.For(0, numberOfPoints, i =>
            {
                numberOfIterations++;
                decimal cxDouble = GenerateRandomDecimal(cx, scx);
                decimal cyDouble = GenerateRandomDecimal(cy, scy);

                // calculate the distance between the circle center and the given point.
                decimal jedna = (cxDouble - cx) * (cxDouble - cx);
                decimal dva = (cyDouble - cy) * (cyDouble - cy);

                decimal vysl = (decimal)Math.Sqrt((double)(jedna + dva));

                if (vysl <= radius)
                {
                    Interlocked.Increment(ref NumberOfCirclePoints);
                }
            });
            Fin(NumberOfCirclePoints, numberOfPoints, numberOfIterations);
        }
        else if (metodaVlaken == "paralelne")
        {
            Task<int>[] tasks = new Task<int>[Environment.ProcessorCount];

            for (int i = 0; i < tasks.Length; i++)
            {
                //Vytvoří nový úkol, který poběží na jiném vláknu
                tasks[i] = Task.Run(() =>
                {
                    int numberOfCirclePointsInTask = 0;

                    for (int j = 0; j < numberOfPoints / tasks.Length; j++)
                    {
                        numberOfIterations++;

                        decimal cxDouble = GenerateRandomDecimal(cx, scx);
                        decimal cyDouble = GenerateRandomDecimal(cy, scy);

                        decimal jedna = (cxDouble - cx) * (cxDouble - cx);
                        decimal dva = (cyDouble - cy) * (cyDouble - cy);

                        decimal vysl = (decimal)Math.Sqrt((double)(jedna + dva));

                        if (vysl <= radius)
                        {
                            numberOfCirclePointsInTask++;
                        }
                    }

                    return numberOfCirclePointsInTask;
                });
            }

            //Spojí do sebe výsledky
            Task.WaitAll(tasks);
            for (int i = 0; i < tasks.Length; i++)
            {
                NumberOfCirclePoints += tasks[i].Result;
            }
            Fin(NumberOfCirclePoints / 2, numberOfPoints, numberOfIterations);
        }
        else
        {
            Console.WriteLine("Něco se solidně dosralo kamaráde (✖╭╮✖)");
        }
    }
    static void Vypis(Process currentProcess, int numberOfIterations)
    {
        Console.WriteLine($"Výpočet Pí: {PI}");
        Console.WriteLine($"Požito vláken: {currentProcess.Threads.Count}");
        Console.WriteLine($"Počet iterací: {numberOfIterations}");
    }
    static void Fin(int NumberOfCirclePoints, long numberOfPoints, int numberOfIterations)
    {
        PI = 4 * (Convert.ToDecimal(NumberOfCirclePoints) / Convert.ToDecimal(numberOfPoints));
        PI = decimal.Parse(PI.ToString("F8"));

        Process currentProcess = Process.GetCurrentProcess();
        Vypis(currentProcess, numberOfIterations);
    }
}