# Bev.Instruments.ArraySpectrometer.Domain

A .NET Framework 4.7.2 library for working with array spectrometers. It provides abstractions and domain utilities to determine optimal exposure/integration time based on live intensity data.

## Features

- Abstractions for `IArraySpectrometer` devices
- Utility methods to estimate optimal exposure/integration time with a safety factor
- Simple debugging output for integration-time sweeps

## Requirements

- .NET Framework 4.7.2
- C# 7.3
- Visual Studio 2022


## Usage

The `Exposure` static class provides extension methods for `IArraySpectrometer` instances to compute and set an optimal integration time.

### API Overview

- `double GetOptimalExposureTime(this IArraySpectrometer spectrometer)`
- `double GetOptimalExposureTime(this IArraySpectrometer spectrometer, bool debug)`
- `double GetOptimalExposureTime(this IArraySpectrometer spectrometer, double targetSignal, bool debug)`

These methods perform a doubling sweep from the device's minimum integration time and estimate the optimal time using linear extrapolation once the measured signal reaches ~49% of the target. The chosen time is rounded to two significant digits and capped to the device maximum.

## Debugging

Enable `debug = true` to print integration-time steps and the resulting signal to the console. In Visual Studio, see the output in the __Debug > Windows > Output__ pane during a run.

## Development

- Code style adheres to repository standards managed via `.editorconfig` and `CONTRIBUTING.md`.
- Use __Build > Rebuild Solution__ to verify changes.
- Run unit tests via __Test > Run All Tests__ if tests are available.
