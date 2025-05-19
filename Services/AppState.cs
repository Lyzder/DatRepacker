using DatRepacker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatRepacker.Services
{
    public class AppState
    {
        private static AppState _instance;
        private static readonly object _lock = new();

        public ObservableCollection<ModContainer> modContainers = new ObservableCollection<ModContainer>();

        private AppState() { }

        public static AppState Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new AppState();
                }
            }
        }
    }
}
