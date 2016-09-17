using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
namespace LotteryMachine
{
    public class Lottery:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int[] _columnNums;
        public int[] ColumnNums
        {
            get { return _columnNums; }
            set { _columnNums = value; }
        }
        private string _lotteryNum;
        public string LotteryNum
        {
            get { return _lotteryNum; }
            set { _lotteryNum = value; OnPropertyChanged(); }
        }

        private bool _isRunning = false;

        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value;OnPropertyChanged(); }
        }

        public ObservableCollection<LotteryRecord> LotteryRecords { get; set; }
        public Lottery()
        {
            LotteryRecords = new ObservableCollection<LotteryRecord>();
        }
        private Random _rand = new Random();

        private CancellationTokenSource _cts;
        public CancellationTokenSource CTS
        {
            get { return _cts; }
            set { _cts = value;OnPropertyChanged(); }
        }
        public async Task<string> RandNums(LotteryConfigration configration,CancellationToken token)
        {

            Debug.WriteLine("随机开始线程：" + Task.CurrentId);

            ColumnNums = new int[configration.Bits];

            int count = configration.AllTime / configration.Period;
            while (true)
            {
                var num = _rand.Next(configration.StartNum, configration.EndNum);
                for (int i = _columnNums.Length - 1; i >= 0; i--)
                {
                    ColumnNums[i] = num % 10;
                    num = num / 10;
                }
                string tempNum = string.Empty;
                foreach (var item in ColumnNums)
                {
                    tempNum += item;
                }
                LotteryNum = tempNum;

                if (token.IsCancellationRequested == true)
                {
                    return LotteryNum;
                }
                await Task.Delay(configration.Period);
            }
        }


        private static LotteryDbContext dbcontext = new LotteryDbContext();
        public  static IList<LotteryRecord> LoadHistory()
        {
            var list= dbcontext.LotteryRecords.ToList();
            return list;
        }
        public  static void SaveRecord(LotteryRecord record)
        {
            dbcontext.LotteryRecords.Add(record);
            dbcontext.SaveChanges();
        }
        public  static void RemoveRecord(LotteryRecord record)
        {
            var item=dbcontext.LotteryRecords.Where(m=>m.ID== record.ID).FirstOrDefault();
            if (item != null)
            {
                dbcontext.LotteryRecords.Remove(item);
            }
            dbcontext.SaveChanges();
        }

    }

    [DataContract]
    public class LotteryConfigration: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        private int _startNum;
        [DataMember]
        public int StartNum
        {
            get { return _startNum; }
            set { _startNum = value; OnPropertyChanged(); }
        }

        private int _endNum;
        [DataMember]
        public int EndNum
        {
            get { return _endNum; }
            set { _endNum = value; OnPropertyChanged(); }
        }

        private int _bits;
        [DataMember]
        public int Bits
        {
            get { return _bits; }
            set { _bits = value; OnPropertyChanged(); }
        }

        private int _period;
        [DataMember]
        public int Period
        {
            get { return _period; }
            set { _period = value; OnPropertyChanged(); }
        }

        private int _allTime;
        [DataMember]
        public int AllTime
        {
            get { return _allTime; }
            set { _allTime = value; OnPropertyChanged(); }
        }

        private string  _showTitle;
        [DataMember]
        public string ShowTitle
        {
            get { return _showTitle; }
            set { _showTitle = value; OnPropertyChanged(); }
        }

        public async void Save()
        {
            var file=await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(typeof(LotteryConfigration).Name, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LotteryConfigration));
                ser.WriteObject(stream.AsStreamForWrite(), this);
            }
        }
        public static async Task<LotteryConfigration> Load()
        {
            StorageFile file = null;
            try
            {
                file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(typeof(LotteryConfigration).Name);
            }
            catch
            {
                return null;
            }
            try
            {
                using (var stream = await file.OpenReadAsync())
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LotteryConfigration));
                    var configration = ser.ReadObject(stream.AsStreamForWrite()) as LotteryConfigration;
                    return configration;
                }
            }
            catch
            {
                return new LotteryConfigration();
            }
            

        }
    }
}
