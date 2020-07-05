using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SAM.Models
{
    public class Account : INotifyPropertyChanged
    {
        private string _timeoutTimeLeft;

        public string Name { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }
        public string SharedSecret { get; set; }
        public string ProfUrl { get; set; }
        public string AviUrl { get; set; }
        public string SteamId { get; set; }
        public DateTime? Timeout { get; set; }

        public string TimeoutTimeLeft { 
            get => _timeoutTimeLeft;
            set
            {
                _timeoutTimeLeft = value;
                OnPropertyChanged();
            }
        }

        public string Description { get; set; }
        public bool CommunityBanned { get; set; }
        public bool IsVacBanned { get; set; }
        public int NumberOfVacBans { get; set; }
        public int DaysSinceLastBan { get; set; }

        public int NumberOfGameBans { get; set; }

        public string EconomyBan { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
