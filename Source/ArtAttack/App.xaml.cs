// <copyright file="App.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Steampunks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using Microsoft.UI.Xaml.Shapes;
    using Steampunks.DataLink;
    using Steampunks.Domain.Entities;
    using Steampunks.Services;
    using Steampunks.Services.TradeService;
    using Steampunks.ViewModels;
    using Steampunks.Views;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceCollection services;
        private Window? window;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.services = new ServiceCollection();
            this.ConfigureServices();

            // Initialize the database
            try
            {
                var dbConnector = new DatabaseConnector();
                if (!dbConnector.TestConnection())
                {
                    System.Diagnostics.Debug.WriteLine("Database not initialized, initializing now...");
                    DatabaseInitializer.InitializeDatabase();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Database already initialized, skipping initialization.");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");

                // In a real application, you might want to show this to the user
                // or handle it more gracefully
            }
        }

        /// <summary>
        /// Gets currentUser.
        /// </summary>
        public static User CurrentUser { get; private set; }

        /// <summary>
        /// Gets a type of service from the ServiceProvider.
        /// </summary>
        /// <typeparam name="T"> Service type. </typeparam>
        /// <returns> A type of service . </returns>
        /// <exception cref="InvalidOperationException">Thrown when the GetService in the serviceProvider is null. </exception>
        public static T GetService<T>()
            where T : class
        {
            if ((App.Current as App)?.services == null)
            {
                throw new InvalidOperationException("Cannot get service before app initialization");
            }

            var serviceProvider = ((App)Current).services.BuildServiceProvider();
            return serviceProvider.GetService<T>() ?? throw new InvalidOperationException($"Service {typeof(T).Name} not found");
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            this.window = new MainWindow();
            this.window.Activate();
        }

        private void ConfigureServices()
        {
            // Add navigation services
            this.services.AddTransient<INavigationViewService, NavigationViewService>();
            this.services.AddSingleton<INavigationService, NavigationService>();
            this.services.AddSingleton<IPageService, PageService>();

            // Add database services
            this.services.AddSingleton<DatabaseConnector>();

            // Add application services
            this.services.AddTransient<UserService>();
            this.services.AddTransient<GameService>();
            this.services.AddTransient<ITradeService>();

            // Add view models
            this.services.AddTransient<ITradeViewModel>();

            // Configure pages
            this.services.Configure<PageServiceOptions>(options =>
            {
                options.Configure<ITradeViewModel, TradeView>();
            });
        }
    }
}
