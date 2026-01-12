using Bev.Instruments.ArraySpectrometer.Abstractions;
using System;

namespace Bev.Instruments.ArraySpectrometer.Domain
{
    public static class Exposure
    {
        // value according to "Photometrieseminar 2024 - Praktikum Spektroradiometrie", slide 7
        private static readonly double _saturationSafetyFactor = 0.85;

        public static double GetOptimalExposureTime(this IArraySpectrometer spectrometer) => spectrometer.GetOptimalExposureTime(_saturationSafetyFactor * spectrometer.SaturationLevel);

        public static double GetOptimalExposureTime(this IArraySpectrometer spectrometer, double targetSignal)
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

