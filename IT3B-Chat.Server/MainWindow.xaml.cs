using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IT3B_Chat.Server
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Messages { get; set; }
        private HttpListener httpListener;
        private WebSocket webSocket;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Messages = new ObservableCollection<string>();

            StartWebSocketServer();
        }

        private async void StartWebSocketServer()
        {
            try
            {
                httpListener = new HttpListener();
                httpListener.Prefixes.Add("http://localhost:8080/");
                httpListener.Start();

                HttpListenerContext context = await httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                    webSocket = webSocketContext.WebSocket;
                    await ReceiveMessages();
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba při spuštění WebSocket serveru: " + ex.Message);
            }
        }

        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Messages.Add(message); // Přidání přijaté zprávy do kolekce pro zobrazení
            }
        }

        private async Task SendText(string text)
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(messageTextBox.Text))
            {
                SendText(messageTextBox.Text);
                messageTextBox.Clear();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            httpListener?.Stop();
            webSocket?.Dispose();
        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            StartWebSocketServer();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (httpListener != null)
            {
                httpListener.Stop();
                webSocket?.Dispose();
                Messages.Add("Odpojeno od serveru.");
            }
        }
    }
}
