using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace IT3B_Chat.Server
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Messages { get; set; }
        public ObservableCollection<string> ConnectionEvents { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Messages = new ObservableCollection<string>();
            ConnectionEvents = new ObservableCollection<string>();

            // Příklad přidání několika zpráv a událostí připojení/odpojení klientů (pro účely demonstrace)
            Messages.Add("Nová zpráva od klienta A: Dobrý den!");
            Messages.Add("Nová zpráva od klienta B: Ahoj, jak se máte?");
            ConnectionEvents.Add("Nový klient připojen: 192.168.0.1");
            ConnectionEvents.Add("Klient odpojen: 192.168.0.2");
        }
    }
}
