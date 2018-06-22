using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace mFramework
{
    /// <summary>
    /// Static class to receive the time from a NTP server.
    /// </summary>
    public class NtpClient
    {
        /// <summary>
        /// Gets the current DateTime from time-a.nist.gov.
        /// </summary>
        /// <returns>A DateTime containing the current time.</returns>
        public static DateTime GetNetworkTime()
        {
            return GetNetworkTime("time.windows.com"); // time-a.nist.gov
        }

        /// <summary>
        /// Gets the current DateTime from <paramref name="ntpServer"/>.
        /// </summary>
        /// <param name="ntpServer">The hostname of the NTP server.</param>
        /// <returns>A DateTime containing the current time.</returns>
        public static DateTime GetNetworkTime(string ntpServer)
        {
            var address = Dns.GetHostEntry(ntpServer).AddressList;

            if (address == null || address.Length == 0)
                throw new ArgumentException("Could not resolve ip address from '" + ntpServer + "'.",
                    nameof(ntpServer));

            var endPoint = new IPEndPoint(address[0], 123);
            return GetNetworkTime(endPoint);
        }

        /// <summary>
        /// Gets the current DateTime form <paramref name="ep"/> IPEndPoint.
        /// </summary>
        /// <param name="ep">The IPEndPoint to connect to.</param>
        /// <returns>A DateTime containing the current time.</returns>
        public static DateTime GetNetworkTime(IPEndPoint ep)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = 1000
            };

            socket.Connect(ep);

            var ntpData = new byte[48]; // RFC 2030 
            ntpData[0] = 0x1B;
            for (var i = 1; i < 48; i++)
                ntpData[i] = 0;

            socket.Send(ntpData);
            socket.Receive(ntpData);

            const byte offsetTransmitTime = 40;
            ulong intpart = 0;
            ulong fractpart = 0;

            for (var i = 0; i <= 3; i++)
                intpart = 256 * intpart + ntpData[offsetTransmitTime + i];

            for (var i = 4; i <= 7; i++)
                fractpart = 256 * fractpart + ntpData[offsetTransmitTime + i];

            var milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);
            socket.Close();

            var timeSpan = TimeSpan.FromTicks((long) milliseconds * TimeSpan.TicksPerMillisecond);
            var dateTime = new DateTime(1900, 1, 1);

            dateTime += timeSpan;

            var offsetAmount = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
            var networkDateTime = (dateTime + offsetAmount);
            return networkDateTime;
        }

        /// <summary>
        /// Gets async the current DateTime from time-a.nist.gov.
        /// </summary>
        /// <returns>A DateTime containing the current time.</returns>
        public static async Task<DateTime> GetNetworkTimeAsync()
        {
            return await GetNetworkTimeAsync("time.windows.com"); // time-a.nist.gov
        }

        /// <summary>
        /// Gets async the current DateTime from <paramref name="ntpServer"/>.
        /// </summary>
        /// <param name="ntpServer">The hostname of the NTP server.</param>
        /// <returns>A DateTime containing the current time.</returns>
        public static async Task<DateTime> GetNetworkTimeAsync(string ntpServer)
        {
            var address = (await Dns.GetHostEntryAsync(ntpServer)).AddressList;
            if (address == null || address.Length == 0)
                throw new ArgumentException("Could not resolve ip address from '" + ntpServer + "'.",
                    nameof(ntpServer));

            var endPoint = new IPEndPoint(address[0], 123);
            return await GetNetworkTimeAsync(endPoint);
        }

        /// <summary>
        /// Gets async the current DateTime form <paramref name="endPoint"/> IPEndPoint.
        /// </summary>
        /// <param name="endPoint">The IPEndPoint to connect to.</param>
        /// <returns>A DateTime containing the current time.</returns>
        public static async Task<DateTime> GetNetworkTimeAsync(IPEndPoint endPoint)
        {
            using (var udpClient = new UdpClient(AddressFamily.InterNetwork))
            {
                await Task.Factory.FromAsync(udpClient.Client.BeginConnect, udpClient.Client.EndConnect,
                    endPoint, null);

                if (!udpClient.Client.Connected)
                    throw new Exception("GetNetworkTimeAsync socket not connected");

                var data = new byte[48]; // RFC 2030 
                data[0] = 0x1B;
                for (var i = 1; i < 48; i++)
                    data[i] = 0;

                await udpClient.SendAsync(data, data.Length);
                var result = await udpClient.ReceiveAsync();

                if (result.Buffer.Length < data.Length)
                    throw new Exception("GetNetworkTimeAsync receive error");

                const byte offsetTransmitTime = 40;
                ulong intpart = 0;
                ulong fractpart = 0;

                for (var i = 0; i <= 3; i++)
                    intpart = 256 * intpart + result.Buffer[offsetTransmitTime + i];

                for (var i = 4; i <= 7; i++)
                    fractpart = 256 * fractpart + result.Buffer[offsetTransmitTime + i];

                var milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);

                var timeSpan = TimeSpan.FromTicks((long) milliseconds * TimeSpan.TicksPerMillisecond);
                var dateTime = new DateTime(1900, 1, 1);

                dateTime += timeSpan;

                var offsetAmount = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
                var networkDateTime = (dateTime + offsetAmount);
                return networkDateTime;
            }
        }
    }
}