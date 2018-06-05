using System;
using System.Runtime;
using System.Windows.Threading;

namespace ImageClassifierApp.Helpers
{
    /// <summary>
    /// Garbage collector helper class this helper is thread safe
    /// </summary>
    public static class GcHelper
    {
        private static DispatcherTimer _timer = null;
        private static int _scheduleRegistrations = 0;
        private static readonly object Sync = new object();
        private static bool _collecting = false;

        #region Scheduled collection

        /// <summary>
        /// registration for scheduled collection in 10 seconds interval
        /// </summary>
        public static void RegisterForScheduledCollection()
        {
            lock (Sync)
            {
                const int secondsBetweenCollecting = 10;
                if (_scheduleRegistrations++ == 0)
                {
                    _timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(secondsBetweenCollecting)
                    };
                    _timer.Tick += TimerOnScheduledCollection;
                    _timer.Start();
                }
            }
        }

        private static void TimerOnScheduledCollection(object sender, EventArgs eventArgs)
        {
            ForceCollect();
        }

        /// <summary>
        /// Unregistration from scheduled collection in 10 seconds interval
        /// </summary>
        public static void UnRegisterScheduledCollection()
        {
            lock (Sync)
            {
                if (--_scheduleRegistrations == 0)
                {
                    _timer.Stop();
                    _timer.Tick -= TimerOnScheduledCollection;
                    _timer = null;
                }
            }
        }

        #endregion

        #region Forced collection 

        /// <summary>
        /// If is not used NoGC region anywhere in code method will force GC into collection
        /// Mostly it works for Generations 0 - 1 
        /// </summary>
        public static void ForceCollect()
        {
            if (_collecting == false && GCSettings.LatencyMode != GCLatencyMode.NoGCRegion)
            {
                lock (Sync)
                {
                    if (GCSettings.LatencyMode != GCLatencyMode.NoGCRegion)
                    {
                        _collecting = true;
                        GC.WaitForPendingFinalizers();
                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                        _collecting = false;
                    }
                }
            }
        }

        #endregion

        #region Heap memory info

        /// <summary>
        /// Method provides info about heap allocation in MB
        /// </summary>
        /// <returns></returns>
        public static int GetHeapAllocatedMemory()
        {
            lock (Sync)
            {
                return (int)(GC.GetTotalMemory(false) / 1024 / 1024);
            }
        }

        #endregion

        #region No garbage collections IDisposable region

        /// <summary>
        /// No GcRegion ussage for IDisposable pattern for faster data processing
        /// NO GC region is working until 100MB allocation is allocated after that is NO GC automatically turned off
        /// </summary>
        public static IDisposable NoGcRegionDisposable
        {
            get
            {
                lock (Sync)
                {
                    const int allowedMemoryIncreaseInMegaBytes = 100 * 1024 * 1024;
                    var region = new NoGcRegion();
                    region.Disposed += RegionOnDisposed;
                    if (GCSettings.LatencyMode != GCLatencyMode.NoGCRegion)
                    {
                        if (!GC.TryStartNoGCRegion(allowedMemoryIncreaseInMegaBytes))
                        {
                            region.Disposed -= RegionOnDisposed;
                        }
                    }
                    return region;
                }
            }
        }

        private static void RegionOnDisposed(object sender, EventArgs e)
        {
            lock (Sync)
            {
                var region = (NoGcRegion)sender;
                region.Disposed -= RegionOnDisposed;
                if (GCSettings.LatencyMode == GCLatencyMode.NoGCRegion)
                {
                    GC.EndNoGCRegion();
                }
            }
        }

        private class NoGcRegion : IDisposable
        {
            public event EventHandler Disposed;
            private bool _disposed;

            public NoGcRegion()
            {
                _disposed = false;
            }

            public void Dispose()
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(NoGcRegion));
                _disposed = true;
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
