using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Cross 
{
    class Connection
    {
        private TcpListener _listener;
        private int _port;
        private string _ip;  // ip сервера, к которому подключаешься, когда ты клиент
        private TcpClient _client;
        private NetworkStream _stream;

        private int X;
        private int Y;
        private int W;

        public delegate void StringMessageReceiveHandler(string message);
        public delegate void TurnReceiveHandler(int x, int y);
        public delegate void GameSettingsReceiveHandler(int x, int y, int w);

        // событие о получении сообщения в чате
        public event StringMessageReceiveHandler Chatted;

        // событие о запросе на подключение
        public event GameSettingsReceiveHandler Requested;
        
        // событие о ходе противника
        public event TurnReceiveHandler Moved;

        public Connection(int port) { _port = port; }
        public Connection(int port, string ip) { _port = port; _ip = ip; }

        public void StartServer()
        {
            //_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
            _listener = new TcpListener(new IPEndPoint(IPAddress.Any, _port));
            _listener.Start();
            Console.WriteLine("Ожидание подключений...");

            _client = _listener.AcceptTcpClient();
            _stream = _client.GetStream();
            Console.WriteLine("Подключился клиент");
            _listener.Stop();

            ProcessConnection();
        }

        public void SetSettings(int x, int y, int w)
        {
            X = x;
            Y = y;
            W = w;
        }

        public void StartClient()
        {
            TcpClient _client = new TcpClient();
            _client.Connect(_ip, _port);
            Console.WriteLine("Подключился к серверу");
            _stream = _client.GetStream();

            SendRequest(X, Y, W);
            ProcessConnection();
        }

        public void SendRequest(int x, int y, int w)
        {
            byte[] data = Encoding.Unicode.GetBytes("request" + x.ToString() + "," + y.ToString() + "," + w.ToString());
            _stream.Write(data, 0, data.Length);
        }

        public void SendMove(int x, int y)
        {
            byte[] data = Encoding.Unicode.GetBytes("turn" + x.ToString() + "," + y.ToString());
            _stream.Write(data, 0, data.Length);
        }


        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes("chat" + message);
            _stream.Write(data, 0, data.Length);

        }


        private void ProcessConnection()
        {
            try
            {
                byte[] data = new byte[64]; // буфер для получаемых данных
                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = _stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (_stream.DataAvailable);

                    string message = builder.ToString();

                    if (message == "") continue;
                    if (message != "")
                    {
                        Console.WriteLine("Получил сообщение: " + message);
                    }

                    // TODO куча ифов для определения типа сообщения и создания нужных событий
                    if (message.StartsWith("chat")) Chatted(message.Substring("chat".Length));
                    if (message.StartsWith("request"))
                    {
                        
                        string request_msg = message.Substring("request".Length);
                        string[] request_mas = request_msg.Split(',');
                        int x = int.Parse(request_mas[0]);
                        int y = int.Parse(request_mas[1]);
                        int w = int.Parse(request_mas[2]);
                        Requested(x, y, w);
                    }
                    if (message.StartsWith("turn")) {
                        string turn_msg = message.Substring("turn".Length);
                        string[] mas = turn_msg.Split(',');
                        int x = int.Parse(mas[0]);
                        int y = int.Parse(mas[1]);
                        Moved(x,y);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Соединение с клиентом разорвано");
            }
            finally
            {
                if (_stream != null)
                    _stream.Close();
                if (_client != null)
                    _client.Close();
            }
        }

        public void Terminate()
        {
            if (_listener != null) _listener.Stop();
            if (_stream != null) _stream.Close();
        }
    }

}
