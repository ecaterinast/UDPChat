using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UdpChat
{
    public class UdpService
    {
        public Socket _socket;
        public int _port;
        public IPAddress _addressBroadcast;

        public List<string> connectedIPs;

        public bool shouldStop = false;
        public UdpService(int port)
        {
            _port = port;
            var hostIP = new IPEndPoint(IPAddress.Any, port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.EnableBroadcast = true;
            _socket.Bind(hostIP);
            _addressBroadcast = broadcastIp();

            connectedIPs = new List<string>();
        }
        public void AddConnectedIP(string ip)
        {
            if (!connectedIPs.Contains(ip))
                connectedIPs.Add(ip);
        }
        public List<string> GetConnectedIPs()
        {
            return connectedIPs;
        }
        public void RemoveConnectedIP(string ip)
        {
            if (connectedIPs.Contains(ip))
            {
                connectedIPs.Remove(ip);
                Broadcast($"User {ip} has disconnected.");
            }
        }
        public void StartReceiveLoop()
        {
            Thread recieve = new Thread(Recieve);
            recieve.Start();
        }
        public void StopReceiveLoop()
        {
            shouldStop = true;
        }
        public void Recieve()
        {
            while (!shouldStop)
            {
                byte[] data = new byte[1024];
                EndPoint remoteClient = new IPEndPoint(IPAddress.Any, 0);

                int bytesRead = _socket.ReceiveFrom(data, ref remoteClient);
                string text = Encoding.UTF8.GetString(data, 0, bytesRead);
                Console.WriteLine($"\nReceived from {remoteClient}: {text}\n");

                if (text == "disconnect")
                {
                    if (remoteClient is IPEndPoint ipEndPoint)
                    {
                        string clientIP = ipEndPoint.Address.ToString();
                        RemoveConnectedIP(clientIP);
                        //StopReceiveLoop(); 
                    }
                    continue;
                }
                else if (remoteClient is IPEndPoint ipEndPoint)
                {
                    string clientIP = ipEndPoint.Address.ToString();
                    AddConnectedIP(clientIP);
                }
            }
        }
        public void Unicast(string text, string IpAddress)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                Thread unicast = new Thread(() =>
                {
                    EndPoint remoteClient = new IPEndPoint(IPAddress.Parse(IpAddress), _port);
                    _socket.SendTo(bytes, remoteClient);
                    Console.WriteLine($"\nSend unicast: {text} to {remoteClient}\n");
                });
                unicast.Start();
                unicast.Join();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void Broadcast(string text)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                Thread broadcast = new Thread(() =>
                {
                    EndPoint remoteClient = new IPEndPoint(_addressBroadcast, _port);
                    _socket.SendTo(bytes, remoteClient);
                    Console.WriteLine($"Send broadcast: {text} to {remoteClient}");
                });
                broadcast.Start();
                broadcast.Join();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public IPAddress broadcastIp()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.Name.StartsWith("Wi-Fi") || ni.Name.Equals("Wi-Fi"))
                {
                    Console.WriteLine(ni.Name);

                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses) //se obtine i lista de informatii deswpre adresa ip unicast
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork) //verificare daca este ipv4
                        {
                            Console.WriteLine("IP Address: " + ip.Address.ToString());

                            IPAddress subnetMask = ip.IPv4Mask;
                            byte[] ipAddressBytes = ip.Address.GetAddressBytes();
                            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

                            byte[] broadcastBytes = new byte[ipAddressBytes.Length];
                            for (int i = 0; i < ipAddressBytes.Length; i++)
                            {
                                broadcastBytes[i] = (byte)(ipAddressBytes[i] | ~(subnetMaskBytes[i]));
                            }

                            IPAddress broadcastAddress = new IPAddress(broadcastBytes);
                            return broadcastAddress;
                        }
                    }

                }
            }
            return null;
        }
    }
}
