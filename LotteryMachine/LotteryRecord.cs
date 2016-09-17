using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LotteryMachine
{
    public class LotteryRecord : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _id;
        public int ID { get { return _id; } set { _id = value; OnPropertyChanged(); } }

        private int _index;
        [NotMapped]
        public int Index
        {
            get { return _index; }
            set { _index = value; OnPropertyChanged(); }
        }

        private DateTime _createTime;
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; OnPropertyChanged(); }
        }

        private string _lotteryNum;
        public string LotteryNum
        {
            get { return _lotteryNum; }
            set { _lotteryNum = value; OnPropertyChanged(); }
        }

        private LotteryLevel _level;
        public LotteryLevel Level
        {
            get { return _level; }
            set { _level = value; OnPropertyChanged(); }
        }
       
    }
    public enum LotteryLevel
    {
        一等奖 = 1,
        二等奖,
        三等奖,
        优秀奖
    }
}
