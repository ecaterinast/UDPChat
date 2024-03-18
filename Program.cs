using UdpChat;

public class Program
{
    private static void Main(string[] args)
    {
        fullUDP myClient = new fullUDP(9001);

        myClient.RecieveMessages();

        //myClient.SendBroadcast("Cristi ti-a trimis un mesaj, verifica te rog.");

        while (true)
        {
            Console.Write("Enter message to broadcast (or 'exit' to close): ");
            string message = Console.ReadLine();

            if (message.ToLower() == "exit")
                break;

            myClient.SendBroadcast(message);
        }


    }
}
