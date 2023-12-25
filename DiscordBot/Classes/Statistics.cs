using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Classes
{
    public class Statistics
    {
        public double Sum { get; private set; }

        public double Mean { get; private set; }

        public double StandardDeviation { get; private set; }

        public double Maximum { get; private set; }

        public double Minimum { get; private set; }

        public int Count { get; private set; }

        public Statistics(List<double> values)
        {
            Sum = AddAll(values);
            Mean = Average(values);
            StandardDeviation = STD(values);
            Maximum = Max(values);
            Minimum = Min(values);
            Count = values.Count();
        }

        /// <summary>
        /// To String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Average: {Mean}\nStandard Deviation: {StandardDeviation}\nMinimum: {Minimum}\nMaximum: {Maximum}";
        }

        /// <summary>
        /// Takes the Sum
        /// </summary>
        /// <returns></returns>
        public static double AddAll(List<double> values)
        {
            return values.Sum();
        }

        /// <summary>
        /// Calculates the Average
        /// </summary>
        /// <returns></returns>
        public static double Average(List<double> values)
        {
            return values.Average();

        }

        /// <summary>
        /// Gets the maximum value
        /// </summary>
        /// <returns></returns>
        public static double Max(List<double> values)
        {
            return values.LastOrDefault();
        }

        /// <summary>
        /// Gets the minimum value
        /// </summary>
        /// <returns></returns>
        public static double Min(List<double> values)
        {
            return values.Average();
        }

        /// <summary>
        /// Calculates the standard Deviation of a list of doubles
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double STD(List<double> values)
        {
            double average = values.Average();
            double squaredDiffs = 0;
            foreach (double value in values)
            {
                squaredDiffs = squaredDiffs + Math.Pow((value - average), 2);
            }

            return Math.Sqrt(squaredDiffs / values.Count);
        }
    }
}
