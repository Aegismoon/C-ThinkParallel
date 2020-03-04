using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lab2
{
    class PrimeNumbers
    {
        public static int nSqrt { get; set; }
        public int range { get; set; }
        public static NumHold[] numbers { get; set; }
        public static int[] basePrimes { get; set; }
        private delegate void Parallel();
        private static volatile int current_index = 0;


        public void SetInitRange(int range)
        {
            if (range <= 0)
            {
                Console.WriteLine("Отрицательный диапазон");
                return;
            }
            nSqrt = (int)Math.Sqrt(range);
            numbers = new NumHold[range];
            SetNumbers();
        }

        public void FindPrimeNumbers(int parType = 1)
        {
            SequentBasePrimes();
            Parallel Par;
            switch (parType)
            {
                case 1: Par = ParrAlgOne; break;

                case 2: Par = ParrAlgTwo; ; break;

                case 3: Par = ParrAlgThree; break;
                default:
                    Par = ParrAlgFour;
                    break;
            }
            Par();
        }



        public void ShowPrimeNumbers(int limit = 10)
        {
            var primes = numbers.Where(x => !x.isComplex).Select(x => x.number).Take(limit).ToArray();
            Console.WriteLine("primes:");
            Array.ForEach(primes, (int x) => { Console.WriteLine(x); });
        }



        private void ParrAlgOne()
        {
            var range_amount = System.Environment.ProcessorCount;
            int diapLen = (numbers.Length - nSqrt) / range_amount;
            int[] rights = new int[range_amount];
            rights[0] = nSqrt;
            for (int i = 1; i < rights.Length; i++)
            {
                rights[i] = rights[i - 1] + diapLen;
                if (rights[i] >= numbers.Length)
                {
                    rights[i] = numbers.Length;
                    break;
                }
            }
            Thread[] tar = new Thread[range_amount];
            for (int i = 0; i < tar.Length; i++)
            {
                tar[i] = new Thread(MultiRangeMultiThread);
                int left = rights[(i - 1) < 0 ? 0 : (i - 1)];
                int right = rights[i];
                tar[i].Start(new object[] { left, right });

            }
            Array.ForEach(tar, (Thread x) => { x.Join(); });
        }

        private static void MultiRangeMultiThread(object o)
        {
            int left = (int)((object[])o)[0];
            int right = (int)((object[])o)[1];
            Array.ForEach(basePrimes, (int bp) => { FullRangePrimes(bp, left, right); });

        }

        private void ParrAlgTwo()
        {
            Thread[] tar = new Thread[basePrimes.Length];
            for (int i = 0; i < tar.Length; i++)
            {
                tar[i] = new Thread(SingleBPallRange);
                tar[i].Start(basePrimes[i]);
            }
            Array.ForEach(tar, (Thread x) => { x.Join(); });

        }




        private static void SingleBPallRange(object obj)
        {

            int bP = (int)obj;
            FullRangePrimes(bP, nSqrt, numbers.Length);
        }


        private void ParrAlgThree()
        {
            ManualResetEvent[] events = new ManualResetEvent[basePrimes.Length];
            for (int i = 0; i < basePrimes.Length; i++)
            {
                events[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(SingleBPThreadPool, new object[] { basePrimes[i], events[i] });
            }

            WaitHandle.WaitAll(events);
        }
        private static void SingleBPThreadPool(object obj)
        {
            int bP = (int)((object[])obj)[0];
            ManualResetEvent ev = ((object[])obj)[1] as ManualResetEvent;
            FullRangePrimes(bP, nSqrt, numbers.Length);
            ev.Set();
        }


        private void ResetAllRange()
        {
            Array.ForEach(numbers, (NumHold x) => { x.isComplex = false; });
        }

        private static void FullRangePrimes(int bP, int left, int right)
        {

            for (int i = left; i < right; i *= bP)
            {
                if (!numbers[i].isComplex)
                {
                    numbers[i].isComplex = true;
                }
            }
        }



        private void ParrAlgFour()
        {
            Thread[] tar = new Thread[System.Environment.ProcessorCount];
            for (int i = 0; i < tar.Length; i++)
            {
                tar[i] = new Thread(MultiThreadSingleBP);
                tar[i].Start();
            }
            Array.ForEach(tar, (Thread x) => { x.Join(); });


        }


        public static void MultiThreadSingleBP()
        {
            int bp;
            int local_index;
            while (true)
            {
                lock ("bp")
                {
                    if (current_index >= basePrimes.Length) break;
                    local_index = current_index++;
                }
                bp = basePrimes[local_index];
                Console.WriteLine(Thread.CurrentThread.Name + " bp" + bp);
                FullRangePrimes(bp, nSqrt, numbers.Length);
            }
        }

        private void SetNumbers()
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i] = new NumHold { number = i + 1, isComplex = false };
            }
        }


        public void SequentBasePrimes()
        {
            int j = 1;
            List<int> tmpBase = new List<int>(nSqrt);
            do
            {
                MarkComplex(j);
                j = NextPrime(j);
            }
            while (j < nSqrt);

            for (int i = 1; i < nSqrt; i++)
            {
                if (!numbers[i].isComplex)
                    tmpBase.Add(numbers[i].number);
            }
            basePrimes = numbers.Skip(1).Take(nSqrt - 1).Where(x => !x.isComplex).Select(x => x.number).ToArray();
        }

        private void MarkComplex(int nIndex)
        {
            var tmp_dev = numbers[nIndex].number;
            for (int i = nIndex*(tmp_dev+1); i < nSqrt; i = i*(tmp_dev+1))
            {
               numbers[i].isComplex = true;
            }

        }

        private int NextPrime(int prevIndex)
        {
            for (int i = prevIndex + 1; i < nSqrt; i++)
            {
                if (!numbers[i].isComplex)
                {
                    return i;
                }
            }
            return nSqrt;
        }


        public void ShowBasePrimes(int limit = 20)
        {
            Console.WriteLine("base primes:");
            Array.ForEach(basePrimes.Take(limit).ToArray(), (int x) => { Console.WriteLine(x); });
        }
    }
}
