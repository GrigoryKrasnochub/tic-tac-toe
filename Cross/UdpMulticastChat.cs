using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Cross
{
    class UdpMulticastChat
    {
        UdpClient client;
        readonly int LOCALPORT; // порт для приема сообщений
        readonly int REMOTEPORT; // порт для отправки сообщений
        const int TTL = 20;
        const string HOST = "235.5.5.1"; // хост для групповой рассылки
        IPAddress groupAddress; // адрес для групповой рассылки

        public delegate void StringMessageReceiveHandler(string ip, string message);
        // событие о получении сообщения в чате
        public event StringMessageReceiveHandler Chatted;

        public UdpMulticastChat(int port)
        {
            LOCALPORT = port;
            REMOTEPORT = port;
            groupAddress = IPAddress.Parse(HOST);

        }

        // метод приема сообщений
        public void ReceiveMessages()
        {
            try
            {
                client = new UdpClient(new IPEndPoint(IPAddress.Any, LOCALPORT));
                // присоединяемся к групповой рассылке
                client.JoinMulticastGroup(groupAddress, TTL);

                string myIP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
                SendMessage("присоединился к чату");

                while (true)
                {
                    IPEndPoint remoteIp = null;
                    byte[] data = client.Receive(ref remoteIp);
                    if (data == null) continue;
                    string message = Encoding.Unicode.GetString(data);
                    if (message == "") continue;

                    string clientIP = remoteIp.Address.ToString();

                    if (myIP == clientIP) continue;
                    Chatted(clientIP, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                client.Send(data, data.Length, HOST, REMOTEPORT);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void Terminate()
        {
            if (client != null) client.Close();
        }
    }
}
