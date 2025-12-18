using Bev.Instruments.ArraySpectrometer.Abstractions;
using System;

namespace Bev.Instruments.ArraySpectrometer.Domain
{
    public static class Exposure
    { 
        private static readonly double SafetyFactor = 0.95;

        public static double GetOptimalExposureTime(this IArraySpectrometer spectrometer) => spectrometer.GetOptimalExposureTime(SafetyFactor * spectrometer.SaturationLevel, false);

        public static double GetOptimalExposureTime(this IArraySpectrometer spectrometer, bool debug) => spectrometer.GetOptimalExposureTime(SafetyFactor * spectrometer.SaturationLevel, debug);

        public static double GetOptimalExposureTime(this IArraySpectrometer spectrometer, double targetSignal, bool debug)
        {
            double maxIntegrationTime = spectrometer.MaximumIntegrationTime;
            double minIntegrationTime = spectrometer.MinimumIntegrationTime;

            double optimalIntegrationTime = 0;
            double integrationTime = minIntegrationTime;
            spectrometer.SetIntegrationTime(integrationTime);

            while (integrationTime < maxIntegrationTime)
            {
                spectrometer.SetIntegrationTime(integrationTime);
                double maxSignal = spectrometer.GetIntensityData().GetMaxIntensity();

                if (debug)
                {
                    Console.WriteLine($">>> debug {spectrometer.GetIntegrationTime():F5} s -> {maxSignal}");
                }
                if (maxSignal >= 0.49 * targetSignal)
                {
                    // Estimate optimal integration time by linear extrapolation
                    optimalIntegrationTime = spectrometer.GetIntegrationTime() * (targetSignal / maxSignal);
                    break;
                }
                integrationTime *= 2;
            }
            var finalIntegrationTime = RoundToSignificantDigits(optimalIntegrationTime, 2);
            if (finalIntegrationTime > maxIntegrationTime)
            {
                finalIntegrationTime = maxIntegrationTime;
            }
            spectrometer.SetIntegrationTime(finalIntegrationTime);
            if (debug)
            {
                double maxSignal = spectrometer.GetIntensityData().GetMaxIntensity();
                Console.WriteLine($">>> debug final {spectrometer.GetIntegrationTime():F5} s -> {maxSignal}");
            }
            return finalIntegrationTime;
        }

        private static double GetMaxIntensity(this double[] signal)
        {
            double maxSignal = double.MinValue;
            foreach (var value in signal)
            {
                if (value > maxSignal)
                {
                    maxSignal = value;
                }
            }
            return maxSignal;
        }

        private static double RoundToSignificantDigits(double number, int digits)
        {
            int sign = Math.Sign(number);
            if (sign < 0) number *= -1;
            if (number == 0) return 0;
            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(number))) + 1);
            return sign * scale * Math.Round(number / scale, digits);
        }
    }
}

