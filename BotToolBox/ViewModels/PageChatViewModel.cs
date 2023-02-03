using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using BotToolBox.Models.Chat;
using BotToolBox.Util;
using Fleck;

namespace BotToolBox.ViewModels
{
    public partial class PageChatViewModel : ObservableObject
    {
        public ObservableCollection<Message> Messages { get; set; }

        private WebSocketServer _webSocketServer;

        [ObservableProperty] private int _wsServerPort = 11451;
        [ObservableProperty] private bool _isConnected = false;

        private List<IWebSocketConnection> _socketConnections;


        public PageChatViewModel()
        {
            Messages = new();
            _socketConnections = new();
        }


        private void SendMessage(Message message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(message);
                foreach (IWebSocketConnection socketConnection in _socketConnections)
                {
                    socketConnection.Send(JsonSerializeUtil.Serialize(message));
                }
            });
        }

        [RelayCommand]
        public void MessageSend(string text)
        {
            SendMessage(new() { User = "ToolBox", Text = text });
        }

        [RelayCommand]
        public void StartWs()
        {
            _webSocketServer = new($"ws://0.0.0.0:{_wsServerPort}");
            _webSocketServer.Start(socket =>
            {
                socket.OnOpen = () => { _socketConnections.Add(socket); };
                socket.OnClose = () => { _socketConnections.Remove(socket); };
                socket.OnMessage = (content) =>
                {
                    try
                    {
                        var message = JsonSerializeUtil.Deserialize<Message>(content);
                        if (message != null) SendMessage(message);
                    }
                    catch (Exception ex)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            var dialog = new Wpf.Ui.Controls.Dialog() { Message = ex.Message };
                            dialog.Show();
                        });
                    }
                };
            });
            if (_webSocketServer.ListenerSocket.Connected)
            {
                IsConnected = true;
            }
        }

        [RelayCommand]
        public void StopWs()
        {
            this._webSocketServer.ListenerSocket.Close();
            this._webSocketServer.Dispose();
            IsConnected = false;
        }
    }
}