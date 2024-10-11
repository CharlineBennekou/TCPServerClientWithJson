using System;
using System.Net.Sockets;
using System.Text;

class TcpClientProgram
{
    static void Main(string[] args)
    {
        try
        {
            // Connect to the server
            TcpClient client = new TcpClient("127.0.0.1", 12001);
            NetworkStream stream = client.GetStream();

            string method = string.Empty;
            int number1 = 0;
            int number2 = 0;
            bool validMethod = false;

            // Loop until valid method is provided
            while (!validMethod)
            {
                Console.WriteLine("Enter the method (random, add, subtract):");
                method = Console.ReadLine().Trim().ToLower();

                // Check if the method is valid
                if (method == "random" || method == "add" || method == "subtract")
                {
                    validMethod = true;
                }
                else
                {
                    Console.WriteLine("Invalid method. Please choose random, add, or subtract.");
                }
            }

            // Reset flag for number validation
            bool validNumbers = false;

            // Loop until valid numbers are provided
            while (!validNumbers)
            {
                try
                {
                    Console.WriteLine("Enter the first number:");
                    number1 = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter the second number:");
                    number2 = int.Parse(Console.ReadLine());

                    // Check if numbers are valid for the random method
                    if (method == "random" && number1 > number2)
                    {
                        Console.WriteLine("For the random method, the first number must be less than or equal to the second. Please try again.");
                    }
                    else
                    {
                        validNumbers = true; // Numbers are valid
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid input. Please enter valid integers.");
                }
            }

            // Manually create a JSON-like string
            string jsonString = $"{{\"method\":\"{method}\",\"number1\":{number1},\"number2\":{number2}}}";

            // Send the JSON-like string to the server
            byte[] data = Encoding.ASCII.GetBytes(jsonString);
            stream.Write(data, 0, data.Length);

            // Receive the response from the server
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Server response: {response}");

            // Close the connection
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
}
