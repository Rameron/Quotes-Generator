using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Quotes_Generator
{
    public class ValuesGenerator
    {
        private const int RECONNECT_DELAY_MS = 5000;
        private const int ROUND_DIGITS = 4;

        private readonly GenerationConfig _generationConfig;
        private readonly Random _randomGenerator;
        private UdpClient _udpClient;
        public long SendPackets { get; private set; }

        public ValuesGenerator(GenerationConfig generationConfig)
        {
            _generationConfig = generationConfig;
            _randomGenerator = new Random();
        }

        ~ValuesGenerator()
        {
            StopGeneration();
        }


        public void StartGeneration()
        {
            InitUdpClient();

            var ipEndPoint = new IPEndPoint(_generationConfig.MulticastGroup, _generationConfig.MulticastPort);
            while (true)
            {
                var packetsCounterBytes = BitConverter.GetBytes(SendPackets);
                var sendingValueBytes = BitConverter.GetBytes(Math.Round(GenerateValue(), ROUND_DIGITS));
                var sendingDataBytes = packetsCounterBytes.Concat(sendingValueBytes).ToArray();
                SendValue(sendingDataBytes, ipEndPoint);
                SendPackets++;
            }
        }

        private void StopGeneration()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient.Dispose();
            }
        }

        private void InitUdpClient()
        {
            try
            {
                _udpClient = new UdpClient(AddressFamily.InterNetwork);
                _udpClient.JoinMulticastGroup(_generationConfig.MulticastGroup);
            }
            catch (SocketException e)
            {
                Console.WriteLine(
                    $"Impossible to initialize the UDP client: {e.Message}.\nOperation will repeat in {RECONNECT_DELAY_MS / 1000} seconds...");
                Thread.Sleep(RECONNECT_DELAY_MS);
                InitUdpClient();
            }
        }

        private double GenerateValue()
        {
            return _generationConfig.MinValue + _randomGenerator.NextDouble() * (_generationConfig.MaxValue - _generationConfig.MinValue);
        }

        private void SendValue(byte[] sendBytes, IPEndPoint ipEndPoint)
        {
            try
            {
                _udpClient.Send(sendBytes, sendBytes.Length, ipEndPoint);
            }
            catch (SocketException e)
            {
                Console.WriteLine(
                    $"Failed to send data through UDP client: {e.Message}.\nOperation will repeat in {RECONNECT_DELAY_MS / 1000} seconds...");
                Thread.Sleep(RECONNECT_DELAY_MS);
                SendValue(sendBytes, ipEndPoint);
            }
        }
    }
}