using Bev.Instruments.ArraySpectrometer.Abstractions;
using System.Linq;

namespace Bev.Instruments.ArraySpectrometer.Domain
{
    public static class NormalizedIntensity
    {
        public static double[] GetNormalizedIntensityData(this IArraySpectrometer spectrometer)
        {
            double saturationLevel = spectrometer.SaturationLevel; // Getting SaturationLevel might be computationally expensive -> cache the value
            double[] intensityData = spectrometer.GetIntensityData();
            return intensityData.Select(intensity => intensity / saturationLevel).ToArray();
        }
    }
}