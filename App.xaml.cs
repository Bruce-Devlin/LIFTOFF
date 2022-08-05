using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LIFTOFF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Exception Handlers
        private bool ExThrown = false;
        [STAThread]
        private async Task ShowUnhandledException(Exception e, bool critical = false)
        {
            if (!ExThrown)
            {
                ExThrown = true;
                Functions.Core.Log("Exception Handled! (CRITICAL = " + critical + ")", true);
                Functions.Core.Log("Exception = Message: " + e.ToString() + " StackTrace:" + e.StackTrace.ToString(), true);

                if (critical)
                {
                    MessageBox.Show("Error! \r\nIt Looks like LIFTOFF ran into an issue and couldn't continue :/ \r\nPlease report this to us! (The error has been copied to your clipboard)", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.ReadLine();
                }

                try
                {
                    Clipboard.SetText(e.Message.ToString());
                }
                catch (Exception ex)
                {
                    Functions.Core.Log("Error copying to clipboard!");
                    Console.WriteLine(ex.ToString());
                    Functions.Core.Log("THE APPLICATION IS UNABLE TO CONTINUE.");
                    Console.ReadLine();
                }
            }
        }

        private async void DispatcherUnhandledEx(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            if (!args.Handled) await ShowUnhandledException(args.Exception, true);
        }

        private async void UnHandledEx(object sender, UnhandledExceptionEventArgs args)
        {
            await ShowUnhandledException((Exception)args.ExceptionObject, true);
        }

        private async void AppDomainUnhandledEx(object sender, UnhandledExceptionEventArgs args)
        {
            await ShowUnhandledException((Exception)args.ExceptionObject, true);
        }

        private async void UnobservedTaskEx(object sender, UnobservedTaskExceptionEventArgs args)
        {
            if (!args.Observed) await ShowUnhandledException(args.Exception, true);
        }

        private async void FirstChanceEx(object sender, FirstChanceExceptionEventArgs args)
        {
            await ShowUnhandledException(args.Exception, false);
        }
        #endregion

        public static string[] StartupArgs;
        void StartUp(object sender, StartupEventArgs e)
        {
            StartupArgs = e.Args;

            Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(DispatcherUnhandledEx);
            AppDomain.CurrentDomain.FirstChanceException += FirstChanceEx;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnHandledEx);
            TaskScheduler.UnobservedTaskException += new EventHandler<UnobservedTaskExceptionEventArgs>(UnobservedTaskEx);

            
            if (Debugger.IsAttached)
            {
                Array.Resize(ref StartupArgs, StartupArgs.Length + 1);
                StartupArgs[StartupArgs.Length - 1] = "-debug true";

                Variables.HomeDir = Directory.GetCurrentDirectory();
            }
            
        }
    }
}
