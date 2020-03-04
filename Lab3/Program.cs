using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lab3
{
    class Program
    {
        //    static string buffer;

        static void double_lock() {
           
            bool bEmpty = true;
            bool finish = false;

            List<string> written_buff = new List<string>();
            List<string> readed_buff = new List<string>();
           
            string buffer = "";

            int w_count = 3;
            int r_count = 5;

            Thread[] writers = new Thread[w_count];
            Thread[] readers = new Thread[r_count];


            for (int i = 0; i < writers.Length; i++)
            {
                int y = i;
                writers[i] = new Thread(() =>
                {
                    int w_number = y + 1;
                    List<string> myMessages = new List<string>();

                    for (int j = 0; j < w_number; j++)
                    {
                        StringBuilder str = new StringBuilder("WWWW");
                        str.Append(j);
                        str.Append("T");
                        str.Append(Thread.CurrentThread.Name);
                        myMessages.Add(str.ToString());
                        lock ("w_b")
                        {
                            written_buff.Add(str.ToString());
                        }
                    }
                    int it = 0;

                    while (it < myMessages.Count)
                    {

                        if (bEmpty)
                        {
                            lock ("w")
                            {
                                if (bEmpty)
                                {
                                    buffer = myMessages[it++];
                                    Console.WriteLine($"{Thread.CurrentThread.Name} >> {buffer}");
                                    bEmpty = false;
                                }
                            }
                        }
                    }

                });
                writers[i].Name = "Writer" + i;
            }

            for (int i = 0; i < readers.Length; i++)
            {
                readers[i] = new Thread(() =>
                {

                    List<string> myMessages = new List<string>();

                    while (!finish)
                    {

                        if (!bEmpty)
                        {

                            lock ("rd")
                            {
                                if (!bEmpty)
                                {
                                    myMessages.Add(buffer);
                                    Console.WriteLine($"{Thread.CurrentThread.Name} << {buffer}");
                                    bEmpty = true;

                                }
                            }
                        }
                    }
                    // когда закончили читать, тогда и добавили
                    lock ("b_buff")
                    {
                        foreach (var read_mess in myMessages)
                        {
                            readed_buff.Add(read_mess);
                        }
                    }
                });
                readers[i].Name = "Reader" + i;
                readers[i].Start();
            }



            for (int i = 0; i < writers.Length; i++)
            {
                writers[i].Start();
            }


            // Ожидание завершения работы читателей
            for (int i = 0; i < writers.Length; i++)
                writers[i].Join();
            finish = true;
            // Ожидаем завершения работы читателей
            for (int i = 0; i < readers.Length; i++)
                readers[i].Join();

            Console.WriteLine("Сколько дублировано:");
          //  written_buff.ForEach(Console.WriteLine);

            var dubs = written_buff.GroupBy(x => x).Where(d => d.Count() > 1).Count();
            Console.WriteLine(dubs);
            Console.WriteLine("Сколько потеряно");
            //            readed_buff.ForEach(Console.WriteLine);
            int lost = written_buff.Count() - readed_buff.Count();
            Console.WriteLine(lost);
            //readed_buff; - КОЛИЧЕСТВО ПО ПЕРЕСЕЧЕНИЮ
        }

        static void event_lock() {

            bool bEmpty = true;
            bool finish = false;

            List<string> written_buff = new List<string>();
            List<string> readed_buff = new List<string>();
            string buffer = "";

            int w_count = 3;
            int r_count = 5;


            Thread[] writers = new Thread[w_count];
            Thread[] readers = new Thread[r_count];





            for (int i = 0; i < writers.Length; i++)
            {
                int y = i;
                writers[i] = new Thread(() =>
                {
                    int w_number = y + 1;
                    List<string> myMessages = new List<string>();


                    // 

                    for (int j = 0; j < w_number; j++)
                    {
                        StringBuilder str = new StringBuilder("AAAA");
                        str.Append(j);
                        myMessages.Add(str.ToString());
                        lock ("w_b")
                        {
                            written_buff.Add(str.ToString());
                        }
                    }
                    int it = 0;
                    while (it < myMessages.Count)
                    {

                        if (bEmpty)
                        {
                            lock ("w")
                            {
                                if (bEmpty)
                                {
                                    buffer = myMessages[it++];
                                    Console.WriteLine($"{Thread.CurrentThread.Name} >> {buffer}");
                                    bEmpty = false;
                                }
                            }
                        }
                    }

                });
                writers[i].Name = "Writer" + i;


            }

            for (int i = 0; i < readers.Length; i++)
            {
                readers[i] = new Thread(() =>
                {

                    List<string> myMessages = new List<string>();

                    while (!finish)
                    {

                        if (!bEmpty)
                        {

                            lock ("rd")
                            {
                                if (!bEmpty)
                                {
                                    myMessages.Add(buffer);
                                    Console.WriteLine($"{Thread.CurrentThread.Name} << {buffer}");
                                    bEmpty = true;

                                }
                            }
                        }
                    }
                    // когда закончили читать, тогда и добавили
                    lock ("b_buff")
                    {
                        foreach (var read_mess in myMessages)
                        {
                            readed_buff.Add(read_mess);
                        }
                    }
                });
                readers[i].Name = "Reader" + i;
                readers[i].Start();
            }



            for (int i = 0; i < writers.Length; i++)
            {
                writers[i].Start();
            }


            // Ожидание завершения работы читателей
            for (int i = 0; i < writers.Length; i++)
                writers[i].Join();



            finish = true;



            // Ожидаем завершения работы читателей
            for (int i = 0; i < readers.Length; i++)
                readers[i].Join();

            Console.WriteLine("Сколько дублировано:");
            //  written_buff.ForEach(Console.WriteLine);

            var dubs = written_buff.GroupBy(x => x).Where(d => d.Count() > 1).Count();
            Console.WriteLine(dubs);
            Console.WriteLine("Сколько потеряно");
            //            readed_buff.ForEach(Console.WriteLine);
            int lost = written_buff.Count() - readed_buff.Count();
            Console.WriteLine(lost);
            //readed_buff; - КОЛИЧЕСТВО ПО ПЕРЕСЕЧЕНИЮ

        }




        static void Main(string[] args)
        {
            Program.double_lock();

        }


    }

}
