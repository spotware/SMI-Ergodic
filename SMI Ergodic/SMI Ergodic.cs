using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SMIErgodic : Indicator
    {
        private IndicatorDataSeries _sourceChange, _sourceChangeAbsolute;

        private IndicatorDataSeries _tsi;

        private ExponentialMovingAverage _longEma, _shortEma, _longAbsoluteEma, _shortAbsoluteEma, _signalEma;

        [Parameter("Long Periods", DefaultValue = 20, MinValue = 1)]
        public int LongPeriods { get; set; }

        [Parameter("Short Periods", DefaultValue = 5, MinValue = 1)]
        public int ShortPeriods { get; set; }

        [Parameter("Signal Line Periods", DefaultValue = 5, MinValue = 1)]
        public int SignalLinePeriods { get; set; }

        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Output("SMI", LineColor = "Blue")]
        public IndicatorDataSeries Smi { get; set; }

        [Output("Signal", LineColor = "Red")]
        public IndicatorDataSeries Signal { get; set; }

        [Output("Oscillator", LineColor = "Yellow", PlotType = PlotType.Histogram, Thickness = 4)]
        public IndicatorDataSeries Oscillator { get; set; }

        protected override void Initialize()
        {
            _sourceChange = CreateDataSeries();
            _sourceChangeAbsolute = CreateDataSeries();
            _tsi = CreateDataSeries();

            _longEma = Indicators.ExponentialMovingAverage(_sourceChange, LongPeriods);
            _shortEma = Indicators.ExponentialMovingAverage(_longEma.Result, ShortPeriods);

            _longAbsoluteEma = Indicators.ExponentialMovingAverage(_sourceChangeAbsolute, LongPeriods);
            _shortAbsoluteEma = Indicators.ExponentialMovingAverage(_longAbsoluteEma.Result, ShortPeriods);

            _signalEma = Indicators.ExponentialMovingAverage(_tsi, SignalLinePeriods);
        }

        public override void Calculate(int index)
        {
            _sourceChange[index] = index == 0 ? double.NaN : Source[index] - Source[index - 1];
            _sourceChangeAbsolute[index] = Math.Abs(_sourceChange[index]);

            _tsi[index] = _shortEma.Result[index] / _shortAbsoluteEma.Result[index];

            Smi[index] = _tsi[index];
            Signal[index] = _signalEma.Result[index];

            Oscillator[index] = Smi[index] - Signal[index];
        }
    }
}