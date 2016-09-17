using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Reflection;
using Windows.UI.Xaml.Media.Animation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace LotteryMachine
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ConfigurationPage : Page
    {
        public LotteryConfigration Configration { get; set; } = new LotteryConfigration();
        public ConfigurationPage()
        {
            this.InitializeComponent();
            this.Loaded += ConfigurationPage_Loaded;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            EdgeUIThemeTransition paneTransition = new EdgeUIThemeTransition();
            paneTransition.Edge = EdgeTransitionLocation.Bottom;
            this.Transitions = new TransitionCollection();
            this.Transitions.Add(paneTransition);
            base.OnNavigatedTo(e);
        }
        private async void ConfigurationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var re=await LotteryConfigration.Load();

            if (re != null)
            {
                var properties = re.GetType().GetProperties();
                foreach (var item in properties)
                {
                    var value = item.GetValue(re);
                    item.SetValue(Configration, value);
                }
            }
            else
            {

            }
            tbLogPath.Text = Logger.LogFilePath;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Configration.Save();
            this.Frame.GoBack();
        }
    }
}
