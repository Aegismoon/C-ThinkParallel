using Lab2;
using System;
namespace Parralell
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt1, dt2;

            PrimeNumbers Prime = new PrimeNumbers();
            Prime.SetInitRange(100);
            dt1 = DateTime.Now;
            Prime.FindPrimeNumbers(1);
            dt2 = DateTime.Now;
            TimeSpan ts = dt2 - dt1;
            Console.WriteLine("Total time:{0} ms", ts.TotalMilliseconds);
            Prime.ShowBasePrimes();
            Prime.ShowPrimeNumbers();


        }
    }
}
