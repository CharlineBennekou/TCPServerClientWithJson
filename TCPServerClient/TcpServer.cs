using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpServer
{
    public static void Main(string[] args)
    {
        TcpListener server = null;

        try
        {
            // Create a TCP/IP socket and bind it to a port
            server = new TcpListener(IPAddress.Any, 12001);
            server.Start();
            Console.WriteLine("Server is up and running...");

            while (true)
            {
                // Accept client connections
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected.");
                NetworkStream stream = client.GetStream();

                // Receive data from the client
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string jsonString = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received JSON: {jsonString}");

                // Using ExtractValue method to get method and numbers from the jsonstring received
                string method = ExtractValue(jsonString, "method");
                string num1String = ExtractValue(jsonString, "number1");
                string num2String = ExtractValue(jsonString, "number2");

                // Convert the numstrings to int
                int num1 = int.Parse(num1String);
                int num2 = int.Parse(num2String);

                string response;
                // Validate method and random number range
                if (method != "random" && method != "add" && method != "subtract")
                {
                    response = "Invalid method. Please choose random, add, or subtract.";
                }
                else if (method == "random" && num1 > num2)
                {
                    response = "For the random method, the first number must be less than or equal to the second.";
                }
                else
                {
                    // Perform the operation if everything is valid
                    response = PerformOperation(method, num1, num2);
                }

                // Send the response back to the client
                byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);

                // Close the client connection
                client.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
        finally
        {
            server?.Stop();
        }
    }

    // Extracts a value from the "JSON-like" string
    private static string ExtractValue(string json, string key)
    {
        string keyPattern = $"\"{key}\":";
        int startIndex = json.IndexOf(keyPattern) + keyPattern.Length;
        int endIndex = json.IndexOf(',', startIndex);
        if (endIndex == -1)
        {
            endIndex = json.IndexOf('}', startIndex);
        }

        string value = json.Substring(startIndex, endIndex - startIndex).Trim();
        // Remove quotes from string values
        if (value.StartsWith("\"") && value.EndsWith("\""))
        {
            value = value.Substring(1, value.Length - 2);
        }
        return value;
    }

    private static string PerformOperation(string method, int num1, int num2)
    {
        switch (method)
        {
            case "random":
                Random rnd = new Random();
                int randomNum = rnd.Next(num1, num2 + 1); // Generates a random number between num1 and num2 (inclusive)
                return $"Random number between {num1} and {num2}: {randomNum}";
            case "add":
                return $"Sum of {num1} and {num2}: {num1 + num2}";
            case "subtract":
                return $"Difference between {num1} and {num2}: {num1 - num2}";
            default:
                return "Unknown operation."; //Should never happen due to validation
        }
    }
}
