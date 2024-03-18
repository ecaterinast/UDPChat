using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UdpChat
{
    public class fullUDP
    {
        private int _port;
        private Socket _socket;
        private IPAddress _addressBroad;

        public fullUDP(int port)
        {
            _port = port;
            _addressBroad = UdpService.broadcastIp();
            var hostIP = new IPEndPoint(IPAddress.Any, _port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.EnableBroadcast = true;
            _socket.Bind(hostIP);

        }

        public void RecieveMessages()
        {
            Thread recieveThread = new Thread(Recieve);
            recieveThread.Start();
        }

        private void Recieve()
        {
            while (true)
            {
                byte[] data = new byte[1024];
                EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                int bytesRead = _socket.ReceiveFrom(data, ref remoteIp);
                string message = Encoding.UTF8.GetString(data, 0, bytesRead);
                Console.WriteLine($"Message from {remoteIp}: {message}");
            }
        }

        public void SendBroadcast(string message)
        {
            Console.WriteLine(_addressBroad);
            byte[] data = Encoding.UTF8.GetBytes(message);

            Thread broadcastThread = new Thread(() =>
            {
                //IPEndPoint remoteIP = new IPEndPoint(IPAddress.Parse("192.168.8.156"), _port);
                EndPoint remoteIP = new IPEndPoint(_addressBroad, _port);

                _socket.SendTo(data, remoteIP);
                Console.WriteLine($"Sent broadcast: {message} to {remoteIP}");
            });

            broadcastThread.Start();
        }
    }
}
