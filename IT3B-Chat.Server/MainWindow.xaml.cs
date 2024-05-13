using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace IT3B_Chat.Server
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Messages { get; set; }
        public ObservableCollection<string> ConnectionEvents { get; set; }
        private BackgroundWorker messageReceiver;
        private TcpClient client;
        private NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Messages = new ObservableCollection<string>();
            ConnectionEvents = new ObservableCollection<string>();

            messageReceiver = new BackgroundWorker();
            messageReceiver.DoWork += MessageReceiver_DoWork;
            messageReceiver.RunWorkerAsync();
        }

        private void MessageReceiver_DoWork(object sender, DoWorkEventArgs e)
        {
            // Simulace příjmu zpráv ze serveru
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new TcpClient(serverAddressTextBox.Text, 1234);
                stream = client.GetStream();
                MessageBox.Show("Připojení k serveru bylo úspěšné.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při připojování k serveru: " + ex.Message);
            }
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.Close();
                MessageBox.Show("Odpojení od serveru.");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    byte[] data = Encoding.UTF8.GetBytes(messageTextBox.Text);
                    stream.Write(data, 0, data.Length);
                    Messages.Add("Odesláno: " + messageTextBox.Text); // Přidání odeslané zprávy do kolekce pro zobrazení
                    messageTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Není připojeno k serveru.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při odesílání zprávy: " + ex.Message);
            }
        }
    }
}
