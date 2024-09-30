using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

public class TCPServer
{
	public static void Main()
	{
		TcpListener listener = new TcpListener(IPAddress.Any, 7);
		listener.Start();
		Console.WriteLine("Server started...");

		while (true)
		{
			TcpClient socket = listener.AcceptTcpClient();
			Console.WriteLine("Client connected");
			Task.Run(() => HandleClient(socket));
		}
	}

	private static void HandleClient(TcpClient socket)
	{
		NetworkStream ns = socket.GetStream();
		StreamReader reader = new StreamReader(ns);
		StreamWriter writer = new StreamWriter(ns);

		while (socket.Connected)
		{
			string message = reader.ReadLine();
			if (message == null) break;

			var request = JsonSerializer.Deserialize<Request>(message);
			string response = HandleRequest(request);
			writer.WriteLine(response);
			writer.Flush();
		}

		socket.Close();
	}

	private static string HandleRequest(Request request)
	{
		switch (request.Command.ToLower())
		{
			case "random":
				int min = request.Parameters[0];
				int max = request.Parameters[1];
				Random random = new Random();
				int randomNumber = random.Next(min, max + 1);
				return JsonSerializer.Serialize(new { result = randomNumber });

			case "add":
				int sum = request.Parameters[0] + request.Parameters[1];
				return JsonSerializer.Serialize(new { result = sum });

			case "subtract":
				int sub = request.Parameters[0] - request.Parameters[1];
				return JsonSerializer.Serialize(new { result = sub });

			default:
				return JsonSerializer.Serialize(new { error = "Unknown command" });
		}
	}
}

public class Request
{
	public string Command { get; set; }
	public int[] Parameters { get; set; }
}
