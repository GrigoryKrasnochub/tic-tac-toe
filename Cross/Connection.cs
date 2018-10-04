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

        public delegate void MessageReceiveHandler(string message);
        // событие о получении сообщения в чате
        public event MessageReceiveHandler Chatted;
        // событие о ходе противника
        public event MessageReceiveHandler Moved;

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

        public void StartClient()
        {
            TcpClient _client = new TcpClient();
            _client.Connect(_ip, _port);
            Console.WriteLine("Подключился к серверу");
            _stream = _client.GetStream();

            byte[] data = Encoding.Unicode.GetBytes("Подключаюсь");
            _stream.Write(data, 0, data.Length);

            ProcessConnection();
        }


        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
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

                    // TODO куча ифов для определения типа сообщения и создания нужных событий
                    Chatted(message);
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
