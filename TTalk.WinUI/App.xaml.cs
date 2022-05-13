﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using TTalk.Library.Packets;
using TTalk.WinUI.Activation;
using TTalk.WinUI.Contracts.Services;
using TTalk.WinUI.Core.Contracts.Services;
using TTalk.WinUI.Core.Services;
using TTalk.WinUI.Helpers;
using TTalk.WinUI.Models;
using TTalk.WinUI.Services;
using TTalk.WinUI.ViewModels;
using TTalk.WinUI.Views;

// To learn more about WinUI3, see: https://docs.microsoft.com/windows/apps/winui/winui3/.
namespace TTalk.WinUI
{
    public partial class App : Application
    {
        // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Other Activation Handlers

                // Services
                services.AddSingleton<ILocalSettingsService, LocalSettingsServiceUnpackaged>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();

                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // Core Services
                services.AddSingleton<IFileService, FileService>();

                // Views and ViewModels
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<MainViewModel>((_) =>
                {
                    if (_mainViewModel == null)
                        ResetMainViewModel();
                    return _mainViewModel;
                });
                services.AddTransient<MainPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();

                // Configuration
                services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            })
            .Build();

        public static T GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T;

        public static Window MainWindow { get; set; } = new Window() { Title = "AppDisplayName".GetLocalized() };

        private static MainViewModel _mainViewModel;

        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
            PacketReader.Init();
        }

        public static void ResetMainViewModel()
        {
            _mainViewModel = new(GetService<ILocalSettingsService>());
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO: Log and handle exceptions as appropriate.
            // For more details, see https://docs.microsoft.com/windows/winui/api/microsoft.ui.xaml.unhandledexceptioneventargs.
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            var activationService = App.GetService<IActivationService>();
            await activationService.ActivateAsync(args);
        }
    }
}