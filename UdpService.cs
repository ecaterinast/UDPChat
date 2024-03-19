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
        public static IPAddress broadcastIp()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.Name.Equals("WiFi"))
                {
                    Console.WriteLine(ni.Name);

                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            Console.WriteLine("IP Address: " + ip.Address.ToString());

                            // Get the subnet mask to calculate the broadcast address
                            IPAddress subnetMask = ip.IPv4Mask;
                            byte[] ipAddressBytes = ip.Address.GetAddressBytes();
                            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

                            // Calculate the broadcast address
                            byte[] broadcastBytes = new byte[ipAddressBytes.Length];
                            for (int i = 0; i < ipAddressBytes.Length; i++)
                            {
                                broadcastBytes[i] = (byte)(ipAddressBytes[i] | ~subnetMaskBytes[i]);
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
