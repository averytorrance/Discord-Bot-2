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

        public bool NullData { get; private set; } = false;

        public Statistics(List<double> values)
        {
            if(values == null || values.Count == 0)
            {
                NullData = true;
                return;
            }
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
            if (NullData) 
            {
                return "Empty Data set. Unable to calculate statistics.";
            }

            return $"Average: {Mean}\nStandard Deviation: {StandardDeviation}\nMinimum: {Minimum}\nMaximum: {Maximum}";
        }

        /// <summary>
        /// To String
        /// </summary>
        /// <returns></returns>
        public string BasicString()
        {
            if (NullData)
            {
                return "Empty Data set. Unable to calculate statistics.";
            }

            return $"Average: {Mean}\nStandard Deviation: {StandardDeviation}";
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
            return values.Max();
        }

        /// <summary>
        /// Gets the minimum value
        /// </summary>
        /// <returns></returns>
        public static double Min(List<double> values)
        {
            return values.Min();
        }

        /// <summary>
        /// Sets the maximum value. May be needed if the max should be pulled from a slightly modified dataset. 
        /// </summary>
        /// <param name="min"></param>
        public void SetMax(double max)
        {
            Maximum = max;
        }

        /// <summary>
        /// Sets the minimum value. May be needed if the min should be pulled from a slightly modified dataset. 
        /// </summary>
        /// <param name="min"></param>
        public void SetMin(double min)
        {
            Minimum = min;
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
