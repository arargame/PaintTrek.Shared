using System;
using System.Diagnostics;

namespace PaintTrek.Diagnostics
{
    public static class ColdStartMetrics
    {
        private static Stopwatch _stopwatch;
        public static long OnCreateToGame1Ms { get; private set; }
        public static long AppToFirstFrameMs { get; private set; }
        public static long Game1ToMainMenuMs { get; private set; }
        public static long TotalColdStartMs { get; private set; }

        public static void Start()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            System.Diagnostics.Debug.WriteLine("[ColdStart] Metrics started.");
        }

        public static void MarkGame1Initialized()
        {
            if (_stopwatch != null && OnCreateToGame1Ms == 0)
            {
                OnCreateToGame1Ms = _stopwatch.ElapsedMilliseconds;
                System.Diagnostics.Debug.WriteLine($"[ColdStart] MainActivity.OnCreate -> Game1.Initialize: {OnCreateToGame1Ms} ms");
            }
        }

        public static void MarkFirstFrameRendered()
        {
            if (_stopwatch != null && AppToFirstFrameMs == 0)
            {
                AppToFirstFrameMs = _stopwatch.ElapsedMilliseconds;
                System.Diagnostics.Debug.WriteLine($"[ColdStart] FIRST FRAME RENDERED! AppToFirstFrame: {AppToFirstFrameMs} ms");
            }
        }

        public static void MarkMainMenuReached()
        {
            if (_stopwatch != null && Game1ToMainMenuMs == 0)
            {
                TotalColdStartMs = _stopwatch.ElapsedMilliseconds;
                Game1ToMainMenuMs = TotalColdStartMs - OnCreateToGame1Ms;
                _stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine($"[ColdStart] Game1.Initialize -> MainMenuScreen: {Game1ToMainMenuMs} ms");
                System.Diagnostics.Debug.WriteLine($"[ColdStart] TOTAL APP -> MAIN MENU TIME: {TotalColdStartMs} ms");
            }
        }
    }
}
