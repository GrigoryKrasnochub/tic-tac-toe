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
        const int LOCALPORT = 33388; // порт для приема сообщений
        const int REMOTEPORT = 33388; // порт для отправки сообщений
        const int TTL = 20;
        const string HOST = "235.5.5.1"; // хост для групповой рассылки
        IPAddress groupAddress; // адрес для групповой рассылки

        public delegate void StringMessageReceiveHandler(string ip, string message);
        // событие о получении сообщения в чате
        public event StringMessageReceiveHandler Chatted;

        public UdpMulticastChat()
        {
            groupAddress = IPAddress.Parse(HOST);

        }

        // метод приема сообщений
        public void ReceiveMessages()
        {
            try
            {
                client = new UdpClient(LOCALPORT);
                // присоединяемся к групповой рассылке
                client.JoinMulticastGroup(groupAddress, TTL);

                while (true)
                {
                    IPEndPoint remoteIp = null;
                    byte[] data = client.Receive(ref remoteIp);
                    if (data == null) continue;
                    string message = Encoding.Unicode.GetString(data);
                    if (message == "") continue;
                    //IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                    //string ip = endPoint.Address.ToString();
                    string ip = "multicast";
                    // добавляем полученное сообщение в текстовое поле
                    Chatted(ip, message);

                    //this.Invoke(new MethodInvoker(() =>
                    //{
                    //    string time = DateTime.Now.ToShortTimeString();
                    //    chatTextBox.Text = time + " " + message + "\r\n" + chatTextBox.Text;
                    //}));
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
    }
}
