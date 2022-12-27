using InvoiceAssistant.Logic.Model;
using InvoiceAssistant.Logic.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InvoiceAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Services = ConfigureServices();

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            // 设置单例程序
            Process currentProcess = Process.GetCurrentProcess();
            Process? process = Process.GetProcessesByName(currentProcess.ProcessName)
                .FirstOrDefault(x => x.Id != currentProcess.Id);
            if (process != null)
            {
                AppHelpers.HandleRunningInstance(process);
                currentProcess.Kill();

            }

            // 创建数据库
            using var context = new ProductContext();
            context.Database.EnsureCreated();

            base.OnStartup(e);
        }

        /// <summary>
        /// 获取 <see cref="IServiceProvider"/> 实例去解析应用程序服务.
        /// </summary>
        public IServiceProvider Services { get; private set; }
        /// <summary>
        /// 配置应用程序的服务。
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            ServiceCollection services = new();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ImportTaxCodeViewModel>();
            return services.BuildServiceProvider();
        }
        /// <summary>
        /// 获取当前使用的 <see cref="App"/> 实例
        /// </summary>
        public new static App Current => (App)Application.Current;

       
    }
}

