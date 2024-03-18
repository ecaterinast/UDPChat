using UdpChat;

public class Program
{
    static UdpService myClient;

    static void Main(string[] args)
    {
        myClient = new UdpService(9001);
        Console.WriteLine("\n");
        myClient.Broadcast("M-am conectat");
        Console.WriteLine("\n");
        myClient.StartReceiveLoop();

        while (true)
        {
            MainMenu();
        }
    }

    public static void MainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine("###########----MENU----##############");
        Console.Write("1.Broadcast \n2.Unicast \n3.Connected IPs\n4.Disconnect\n");
        string choice = Console.ReadLine();
        if (choice.Equals("1"))
        {
            Console.Write("Enter message: ");
            string text = Console.ReadLine();
            myClient.Broadcast(text);
        }
        else if (choice.Equals("2"))
        {
            Console.Write("Enter IP:\n");
            string ip = Console.ReadLine();

            Console.Write("Enter message : \n");
            string text = Console.ReadLine();

            myClient.Unicast(text, ip);
        }
        else if (choice.Equals("3"))
        {
            Console.WriteLine("Connected IPs:");
            foreach (string ip in myClient.GetConnectedIPs())
            {
                Console.WriteLine(ip);
            }
        }
        else if (choice.Equals("4"))
        {
            Console.WriteLine("Disconnect");
            Console.Write("Are you sure you want to disconnect? (y/n): ");
            string confirm = Console.ReadLine();
            if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                myClient.Broadcast("disconnect");
                Environment.Exit(0);
            }
        }
        else
        {
            Console.WriteLine("Wrong choice!");
        }
    }


}
