﻿using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GoogleMapsUnofficial.ViewModel.SettingsView;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using GoogleMapsUnofficial.Data;
using GoogleMapsUnofficial.Common;
using GoogleMapsUnofficial.View.SettingsView;
using GoogleMapsUnofficial.View;

namespace GoogleMapsUnofficial
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public static ElementTheme ActualTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    if (rootElement.RequestedTheme != ElementTheme.Default)
                    {
                        return rootElement.RequestedTheme;
                    }
                }

                return GetEnum<ElementTheme>(Current.RequestedTheme.ToString());
            }
        }

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
       
        public App()
        {

            //this.RequestedTheme = ApplicationTheme.Dark;
            InitializeTheme();
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;

        }

        //private void SaveTheme()
        //{
        //    var value = SettingsSetters.GetLocalSetting<string>("SelectedTheme", null);
        //    if (value != null)
        //    {
        //        SettingsSetters.SaveLocalSetting("SelectedTheme", value);                
        //    }
        //}
        private void InitializeTheme()
        {
            var value = SettingsSetters.GetLocalSetting<string>("SelectedTheme", null);
            if (value != null)
            {
                var theme = Enum.Parse(typeof(ApplicationTheme), value);
                RequestedTheme =(ApplicationTheme) theme;
            }
        }

        public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.");
            }
            return (TEnum)Enum.Parse(typeof(TEnum), text);
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var d = DateTime.Now;
            var st = sender.GetType();
            //var doc = await KnownFolders.PicturesLibrary.CreateFileAsync($"WingoMaps{d.Year},{d.Month},{d.Day},{d.Hour}_{d.Minute}_{d.Second}.txt", CreationCollisionOption.GenerateUniqueName);
            string CrashReport = $"Wingo MAP Crash Report {d.ToString()}.{Environment.NewLine}SenderType= {st.Name}{Environment.NewLine}ExceptionMessage={Environment.NewLine}{e.Message}{Environment.NewLine}{e.Exception.Message}Stack Trace:{e.Exception.StackTrace}{Environment.NewLine}Source={e.Exception.Source}{Environment.NewLine}HResult={e.Exception.HResult}{Environment.NewLine}HelpLink={e.Exception.HelpLink}{Environment.NewLine}";
            if (e.Exception.InnerException == null)
            {
                CrashReport += $"Inner Exception=False{Environment.NewLine}";
            }
            else
            {
                CrashReport += $"Inner Exception Present=False{Environment.NewLine}Inner Exception:{Environment.NewLine}Message:{e.Exception.InnerException.Message}{Environment.NewLine}StackTrace={e.Exception.InnerException.StackTrace}{Environment.NewLine}HResult={e.Exception.InnerException.HResult}{Environment.NewLine}HelpLink={e.Exception.InnerException.HelpLink}{Environment.NewLine}";
            }
            ApplicationData.Current.LocalSettings.Values["CrashDump"] = CrashReport;
            //await FileIO.WriteTextAsync(doc, CrashReport);
        }

        private void StartFluent()
        {
            if (ClassInfo.DeviceType() == ClassInfo.DeviceTypeEnum.Phone) return;
            if (ApiInformation.IsMethodPresent("Windows.UI.Xaml.Hosting.ElementCompositionPreview", "SetElementChildVisual"))
            {
                ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
                formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
                formattableTitleBar.ButtonForegroundColor = Colors.Black;
                formattableTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                formattableTitleBar.ButtonInactiveForegroundColor = Colors.Black;
                //formattableTitleBar.ButtonHoverForegroundColor = Colors.Transparent;

                //formattableTitleBar.InactiveForegroundColor = Colors.Black;
                //formattableTitleBar.InactiveBackgroundColor = Colors.Transparent;
                CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e != null)
            {
                if (e.PreviousExecutionState == ApplicationExecutionState.Running)
                {
                    Window.Current.Activate();
                    return;
                }
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    try
                    {
                        //await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
                if (e.Kind == ActivationKind.VoiceCommand)
                {

                }
            }
            try
            {
                StatusBar.GetForCurrentView().ForegroundColor = Colors.Transparent;
            }
            catch { }
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = Colors.DodgerBlue;
                    statusBar.ForegroundColor = Colors.White;
                }
            }
            StartFluent();
            Resources["ToggleButtonBackgroundChecked"] = Color.FromArgb(255, 96, 165, 255);
            Resources["SystemControlHighlightListAccentLowBrush"] = Color.FromArgb(255, 96, 165, 255);
            Resources["ToggleSwitchFillOn"] = Color.FromArgb(255, 96, 165, 255);
            Resources["HyperlinkButtonForeground"] = Color.FromArgb(255, 96, 165, 255);
            Resources["SystemControlBackgroundAccentBrush"] = Color.FromArgb(255, 96, 165, 255);
            SplashScreen splashScreen = e.SplashScreen;
            ExtendedSplashScreen eSplash = null;
            if (e.Arguments != "")
            {
                var sp = e.Arguments.Split(',');
                eSplash = new ExtendedSplashScreen(splashScreen, new Uri($"https://google.com/maps/@?api=1&map_action=map&center={sp[0]},{sp[1]}&zoom=17", UriKind.RelativeOrAbsolute));
            }
            else
            {
                eSplash = new ExtendedSplashScreen(splashScreen);
            }
            // Register an event handler to be executed when the splash screen has been dismissed.
            Window.Current.Content = eSplash;
            Window.Current.Activate();
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);
            #region Start app

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            #endregion
            try
            {
                StatusBar.GetForCurrentView().ForegroundColor = Colors.Black;
            }
            catch { }
            StartFluent();
            Resources["ToggleButtonBackgroundChecked"] = Color.FromArgb(255, 96, 165, 255);
            Resources["SystemControlHighlightListAccentLowBrush"] = Color.FromArgb(255, 96, 165, 255);
            Resources["ToggleSwitchFillOn"] = Color.FromArgb(255, 96, 165, 255);
            Resources["HyperlinkButtonForeground"] = Color.FromArgb(255, 96, 165, 255);
            Resources["SystemControlBackgroundAccentBrush"] = Color.FromArgb(255, 96, 165, 255);
            SplashScreen splashScreen = e.SplashScreen;
            ExtendedSplashScreen eSplash = null;

            if (e.Kind == ActivationKind.VoiceCommand)
            {
                var voiceArgs = (VoiceCommandActivatedEventArgs)e;
                var Rule = voiceArgs.Result.RulePath.FirstOrDefault();
                var input = voiceArgs.Result.SemanticInterpretation.Properties.Where(x => x.Key == "UserInput").FirstOrDefault().Value.FirstOrDefault();
                if (Rule == "DirectionsCommand")
                {
                    eSplash = new ExtendedSplashScreen(splashScreen, new Uri("https://google.com/maps/@searchplace=" + input, UriKind.RelativeOrAbsolute));
                }
                if (Rule == "FindPlace")
                {
                    eSplash = new ExtendedSplashScreen(splashScreen, new Uri("https://google.com/maps/@searchplace=" + input, UriKind.RelativeOrAbsolute));
                }
                if (Rule == "WhereAmI")
                {
                    eSplash = new ExtendedSplashScreen(splashScreen);
                }
            }
            if (e.Kind == ActivationKind.Protocol)
            {
                var protocolArgs = (ProtocolActivatedEventArgs)e;
                var x = protocolArgs.Uri.ToString();

                // Register an event handler to be executed when the splash screen has been dismissed.
                //Type deepLinkPageType = typeof(PageLogin);
                if (rootFrame.Content == null)
                {
                    try
                    {
                        switch (protocolArgs.Uri.AbsolutePath.ToLower().Replace("/maps", string.Empty).Split('/')[1].ToString())
                        {
                            case "search":
                                eSplash = new ExtendedSplashScreen(splashScreen, protocolArgs.Uri);
                                break;
                            case "dir":
                                eSplash = new ExtendedSplashScreen(splashScreen, protocolArgs.Uri);
                                break;
                            case "@":
                                eSplash = new ExtendedSplashScreen(splashScreen, protocolArgs.Uri);
                                break;
                            default:
                                eSplash = new ExtendedSplashScreen(splashScreen);
                                break;
                        }
                    }
                    catch
                    {
                        eSplash = new ExtendedSplashScreen(splashScreen, protocolArgs.Uri);
                    }
                }
            }
            Window.Current.Content = eSplash;
            Window.Current.Activate();
        }

       

        
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //SaveTheme();
            //TODO: Save application state and stop any background activity
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
