using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace LotteryMachine
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public LotteryConfigration configration { get; set; }
        public Lottery Lottery { get; set; } = new Lottery();
        public ObservableCollection<LotteryRecord> LotteryNumbers { get; set; } = new ObservableCollection<LotteryRecord>();

        private MediaPlayer _player  = BackgroundMediaPlayer.Current;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.pivotList.SelectionChanged += PivotList_SelectionChanged;
            this.imgRunning.Loaded += ImgRunning_Loaded;
            this.KeyUp += MainPage_KeyUp;
           
        }

        private void MainPage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key== Windows.System.VirtualKey.Enter)
            {
                if (Lottery.IsRunning == true)
                {
                    gridRunning_Tapped(null,null);
                }
                else
                {
                    btnStart_Click(null,null);
                }
            }
        }

        private void ImgRunning_Loaded(object sender, RoutedEventArgs e)
        {
            imgRunningStoryBoard.Begin();
        }

        private void PivotList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (1== pivotList.SelectedIndex )
            {
                var list=Lottery.LoadHistory();
                numberListHistory.ItemsSource = list;
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var re=await LotteryConfigration.Load();
            configration = re;

            _player.AutoPlay = false;
            MediaSource source=MediaSource.CreateFromUri((new Uri("ms-appx:///musics/鼓乐3.mp3")));
            _player.Source = source;
            _player.IsLoopingEnabled = true;
            _player.AudioBalance = 0.5;
            
        }

        private CancellationTokenSource _cts;
        private async  void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("开始UI线程：" + Task.CurrentId);
            if (configration == null)
            {
                await new MessageDialog("抽奖配置信息无效").ShowAsync();
                return;
            }

            StartStoryboard.Begin();

            _player.Play();

            Lottery.IsRunning = true;
            btnStart.Visibility = Visibility.Collapsed;
            _cts = new CancellationTokenSource();
            var num=await Lottery.RandNums(configration, _cts.Token);

            //记录日志
            Logger.Log(num.ToString());
            //添加到信息列表
            LotteryRecord record = new LotteryRecord();
            record.CreateTime = DateTime.Now;
            record.Level =  LotteryLevel.一等奖;
            record.LotteryNum = num;
            Lottery.SaveRecord(record);

            record.Index = LotteryNumbers.Count+1;

            LotteryNumbers.Insert(0, record);

            btnStart.Visibility = Visibility.Visible;
            Lottery.IsRunning = false;

            StopStoryboard.Begin();

            Debug.WriteLine("结束UI线程：" + Task.CurrentId);
        }

        private void btnConfigure_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ConfigurationPage));
        }

        private void btnChnageList_Click(object sender, RoutedEventArgs e)
        {
            if (pivotList.SelectedIndex == 0)
            {
                pivotList.SelectedIndex = 1;
            }
            else
            {
                pivotList.SelectedIndex = 0;
            }
        }

        private void btnItemDelete_Click(object sender, RoutedEventArgs e)
        {
           var grid= VisualTreeHelper.GetParent(sender as FrameworkElement);
            ContentPresenter presenter = null;
            for (int i=0;i<VisualTreeHelper.GetChildrenCount(grid);i++)
            {
                var child = VisualTreeHelper.GetChild(grid, i);
                if (child is ContentPresenter)
                {
                    presenter = child as ContentPresenter;
                    break;
                }
                    
            }
            var lotteryRecord = presenter.Content as LotteryRecord;
            var item=LotteryNumbers.Where(m => m.LotteryNum == lotteryRecord.LotteryNum).FirstOrDefault();
            if (item != null)
            {
                var list=LotteryNumbers.Where(m => m.Index > item.Index).ToList();
                foreach (var record in list)
                {
                    record.Index--;
                }
                LotteryNumbers.Remove(item);
            }
               
        }

        private void btnClearList_Click(object sender, RoutedEventArgs e)
        {
            LotteryNumbers.Clear();
        }

        private void gridRunning_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _cts.Cancel();
            _player.Pause();
        }

        #region 抽奖详细页面
        private void btnShowList_Click(object sender, RoutedEventArgs e)
        {
            
            gridShow.Visibility = Visibility.Visible;
            if (string.IsNullOrWhiteSpace(configration.ShowTitle) != true)
            {
                tbShowTitle.Text = configration.ShowTitle;
            }
        }

        private void btnShowBack_Click(object sender, RoutedEventArgs e)
        {
            gridShow.Visibility = Visibility.Collapsed;
        }
        #endregion
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool status = (bool)value;
            if (true == status)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
