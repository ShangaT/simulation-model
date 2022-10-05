using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    class Channel
    {
        public bool free = true;
        public int halp = 0;
        public double timeService = 0;
        public double timeRelease = 0;
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            double lambda = 0.037;
            double mu = 0.005;
            double nu = 0.015;
            double eta = 0.007;
            int n = 3, l = 3, m = 1;

            double sumCh = 0;
            double Next = 0;
            double timeMod = 0;
                        
            bool QueueFree = true;
            double timeQueue = 0;
            int scoreQueue = 0;
            
            Channel[] ArrayChannels = new Channel[n]; //массив для каналов
            for (int i = 0; i < n; i++)
            {
                ArrayChannels[i] = new Channel();
            }

            int countFreeChannels() //функция для нахождения числа свободных каналов
            {
                int Count = 0;
                for (int i = 0; i < n; i++)
                {
                    if (ArrayChannels[i].free == true) Count++;
                }
                return Count;
            }

            while (Next < 1000000)
            {
                Random RND = new Random();
                double timeNext = -(1 / lambda) * Math.Log(RND.NextDouble()); //время до появление следующей заявки                

                if (countFreeChannels() != 0) //проверяем свободные каналы
                {
                    if (countFreeChannels() >= l) //проверяем возможность взаимопомощи l
                    {
                        double timeService = -(1 / (l * mu + eta)) * Math.Log(RND.NextDouble());
                        int k = 0; //служебная перемення
                        while (k < l) //занимаем l каналов
                        {
                            for (int i = 0; i < n; i++)
                            {
                                if (ArrayChannels[i].free == true)
                                {
                                    ArrayChannels[i].free = false;
                                    ArrayChannels[i].halp = l;
                                    ArrayChannels[i].timeService = timeService; //время обслуживания заявки
                                    ArrayChannels[i].timeRelease = timeService + timeMod;
                                    break;
                                }
                            }
                            k++;
                        }
                    }
                    if (countFreeChannels() == l - 1) // проверяем взаимопомощь l-1
                    {
                        double timeService = -(1 / ((l - 1) * mu + eta)) * Math.Log(RND.NextDouble());
                        int k = 0; //служебная перемення
                        while (k < l - 1) //занимаем l-1 каналов
                        {
                            for (int i = 0; i < n; i++)
                            {
                                if (ArrayChannels[i].free == true)
                                {
                                    ArrayChannels[i].free = false;
                                    ArrayChannels[i].halp = l - 1;
                                    ArrayChannels[i].timeService = timeService; //время обслуживания заявки                     
                                    ArrayChannels[i].timeRelease = timeService + timeMod;
                                    break;
                                }
                            }
                            k++;
                        }
                    }
                    if (countFreeChannels() == 1)
                    {
                        double timeService = -(1 / (mu + eta)) * Math.Log(RND.NextDouble());
                        for (int i = 0; i < n; i++)
                        {
                            if (ArrayChannels[i].free == true)
                            {
                                ArrayChannels[i].free = false;
                                ArrayChannels[i].halp = 1;
                                ArrayChannels[i].timeService = timeService; //время обслуживания заявки
                                ArrayChannels[i].timeRelease = timeService + timeMod;
                                break;
                            }
                        }
                    }
                }
                else //если свободных каналов нет
                {
                    for (int j = 1; j < n; j++) //сортируем
                    {
                        for (int i = 0; i < n - 1; i++)
                        {
                            if (ArrayChannels[i].timeService > ArrayChannels[i + 1].timeService)
                            {
                                double x = ArrayChannels[i].timeService;
                                ArrayChannels[i].timeService = ArrayChannels[i + 1].timeService;
                                ArrayChannels[i + 1].timeService = x;
                            }
                        }
                    }

                    for (int i = 0; i < n - 2; i++) //ищем l каналов обслуживающие одну заявку
                    {
                        if (ArrayChannels[i].timeService == ArrayChannels[i + 1].timeService && ArrayChannels[i].timeService == ArrayChannels[i + 2].timeService)
                        {
                            ArrayChannels[i].timeService = -(1 / (mu + eta)) * Math.Log(RND.NextDouble()); // отрываем один канал                            
                            ArrayChannels[i].timeRelease = timeMod + ArrayChannels[i].timeService;
                            ArrayChannels[i].halp = 1;
                                                        
                            double timeSrvice =  (-(1 / ((l - 1) * mu + eta)) * Math.Log(RND.NextDouble()));

                            ArrayChannels[i + 1].timeService = timeSrvice;
                            ArrayChannels[i + 1].timeRelease = timeMod + timeSrvice;
                            ArrayChannels[i + 1].halp = l - 1;

                            ArrayChannels[i + 2].timeService = timeSrvice;
                            ArrayChannels[i + 2].timeRelease = timeMod + timeSrvice;
                            ArrayChannels[i + 2].halp = l - 1;
                            break;
                        }
                        if (ArrayChannels[i].timeService == ArrayChannels[i + 1].timeService && ArrayChannels[i].timeService != ArrayChannels[i + 2].timeService)
                        { //ищем l-1 каналов обслуживающие одну заявку
                            ArrayChannels[i].timeService = -(1 / (mu + eta)) * Math.Log(RND.NextDouble()); // отрываем один канал                            
                            ArrayChannels[i].timeRelease = timeMod + ArrayChannels[i].timeService;
                            ArrayChannels[i].halp = 1;
                                                       
                            double timeSrvice =  (-(1 / ((l - 1) * mu + eta)) * Math.Log(RND.NextDouble()));

                            ArrayChannels[i + 1].timeService = timeSrvice;
                            ArrayChannels[i + 1].timeRelease = timeMod + timeSrvice;
                            ArrayChannels[i + 1].halp = 1;
                            break;
                        }
                        else //если все каналы работают без взаимопомощи
                        {
                            if (QueueFree == true)//проверяем места в очереди 
                            {
                                QueueFree = false; //ставим заявку в очередь                        
                                scoreQueue++;
                                timeQueue = timeMod;
                            }
                        }
                    }
                }

                timeMod += timeNext; //приращение времени моделирования
                Next++;

                if (timeMod > timeQueue + (1 / nu)) //освобождение места в очереди, если время ожидания истекло
                {
                    QueueFree = true;
                    timeQueue = 0;
                }

                for (int i = 0; i < n; i++) //если канал освободился и есть заявка в очереди берем заявку из очереди на обслуживания
                {
                    if (timeMod >= ArrayChannels[i].timeRelease)
                    {
                        if (QueueFree == false)
                        {                           
                            QueueFree = true; //осовбождаем место в очереди 
                            timeQueue = 0;

                            ArrayChannels[i].halp = 1;
                            ArrayChannels[i].timeService = -(1 / (mu + eta)) * Math.Log(RND.NextDouble());
                            ArrayChannels[i].timeRelease += ArrayChannels[i].timeService;
                            break;
                        }
                    }
                }

                for (int i = 0; i < n; i++) // освобождение каналов
                {
                    if (timeMod >= ArrayChannels[i].timeRelease || ArrayChannels[i].timeService > 1 / eta)
                    {                        
                        ArrayChannels[i].free = true;
                        ArrayChannels[i].halp = 0;
                        ArrayChannels[i].timeService = 0;
                        ArrayChannels[i].timeRelease = 0;
                    }
                }

                sumCh += n - countFreeChannels();
            } //конец основного цикла       
                        
            double p = sumCh / Next / n;
            double r = scoreQueue / Next; //средняя длинна очереди
            double t = r / lambda;

            Console.WriteLine($"Вероятность занятости канала = {p}");
            Console.WriteLine($"Среднее число заявок в очереди = {r}");
            Console.WriteLine($"Среднее время нахождения заявки в очереди = {t}");
        }
    }

}
