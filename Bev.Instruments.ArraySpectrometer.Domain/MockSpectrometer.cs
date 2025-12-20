using Bev.Instruments.ArraySpectrometer.Abstractions;
using System;
using System.Threading;

namespace Bev.Instruments.ArraySpectrometer.Domain
{
    public class MockSpectrometer : IArraySpectrometer
    {
        public MockSpectrometer()
        {
            _wavelengthsCache = CreateEquidistantWavelengths(400, 800, 1);
            _integrationTimeCache = MinimumIntegrationTime;
        }

        public string InstrumentManufacturer => "MockManufacturer";

        public string InstrumentType => "MockSpectrometer";

        public string InstrumentSerialNumber => "000001";

        public string InstrumentFirmwareVersion => "1.0.0";

        public double[] Wavelengths => _wavelengthsCache;

        public double MinimumWavelength => _wavelengthsCache[0];

        public double MaximumWavelength => _wavelengthsCache[_wavelengthsCache.Length - 1];

        public double SaturationLevel => 1.0;

        public double MinimumIntegrationTime => 0.001;

        public double MaximumIntegrationTime => 10.0;

        public double GetIntegrationTime() => _integrationTimeCache;

        public double[] GetIntensityData()
        {
            Thread.Sleep((int)(GetIntegrationTime()*1000)); // Simulate some delay
            double[] intensity = new double[_wavelengthsCache.Length]; // Mock data
            for (int i = 0; i < intensity.Length; i++)
            {
                intensity[i] = SaturationLevel / (i + 1); // Just a simple mock
            }
            return intensity;   
        }

        public void SetIntegrationTime(double seconds)
        {
            if (seconds < MinimumIntegrationTime) seconds = MinimumIntegrationTime;
            if (seconds > MaximumIntegrationTime) seconds = MaximumIntegrationTime;
            _integrationTimeCache = seconds;
        }

        private double[] _wavelengthsCache;
        private double _integrationTimeCache;

        private static double[] CreateEquidistantWavelengths(double startWavelength, double endWavelength, double step)
        {
            if (step <= 0)
            {
                throw new ArgumentException("Step must be a positive value.");
            }
            int numPoints = (int)Math.Floor((endWavelength - startWavelength) / step) + 1;
            double[] wavelengths = new double[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                wavelengths[i] = startWavelength + i * step;
            }
            return wavelengths;
        }
    }
}
