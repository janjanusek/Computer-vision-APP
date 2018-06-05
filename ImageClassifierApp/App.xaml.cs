using System;
using System.Diagnostics;
using System.Windows;
using AiSdk.NeuralNet.Gpu;
using ImageClassifierApp.Helpers;

namespace ImageClassifierApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var message =
                $"Something unexpected happend in AI-TOOL. For more info read exception: {unhandledExceptionEventArgs.ExceptionObject}";
            const string title = "Unexpected error";
            try
            {
                NotepadHelper.ShowMessage(message, title);
            }
            catch (Exception)
            {
                MessageBox.Show(message, title);
            }
        }

        /// <summary>
        /// Becuse GPU cannot be used more than once, application will not allow user to use more instances of AI TOOL
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Get Reference to the current Process
            var thisProc = Process.GetCurrentProcess();
            // Check how many total processes have the same name as the current one
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                try
                {
                    var window = new Window()
                    {
                        Visibility = Visibility.Hidden,
                        AllowsTransparency = true,
                        Background = System.Windows.Media.Brushes.Transparent,
                        WindowStyle = WindowStyle.None,
                        ShowInTaskbar = false
                    };

                    window.Show();
                    MessageBox.Show(window, "Application AI TOOL is already running. Click OK to close this window.", "App is already running.");
                    window.Close();
                    Win32Helper.Activate("AI TOOL");
                }
                catch
                {
                    // ignored
                }
                Application.Current.Shutdown();
                return;
            }
            base.OnStartup(e);
        }

        /// <summary>
        /// Clean all GPU resources before shut down
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            GpuCardManager.Instance.Shutdown();
            base.OnExit(e);
        }

        /// <summary>
        /// Helper method for running action in UI thread
        /// </summary>
        /// <param name="paAction"></param>
        public static void RunInUiThread(Action paAction)
        {
            Application.Current?.Dispatcher.Invoke(paAction);
        }

        /// <summary>
        /// Helper method for running function in UI thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paFunc"></param>
        /// <returns></returns>
        public static T RunInUiThread<T>(Func<T> paFunc)
        {
            T result = default(T);
            Application.Current?.Dispatcher.Invoke(() =>
            {
                result = paFunc.Invoke();
            });
            return result;
        }
    }
}
