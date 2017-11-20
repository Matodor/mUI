using System;
using System.Net;
using System.Net.Sockets;

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
                throw new ArgumentException("Could not resolve ip address from '" + ntpServer + "'.", nameof(ntpServer));

            var ep = new IPEndPoint(address[0], 123);

            return GetNetworkTime(ep);
        }

        /// <summary>
        /// Gets the current DateTime form <paramref name="ep"/> IPEndPoint.
        /// </summary>
        /// <param name="ep">The IPEndPoint to connect to.</param>
        /// <returns>A DateTime containing the current time.</returns>
        public static DateTime GetNetworkTime(IPEndPoint ep)
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                ReceiveTimeout = 1000
            };
            s.Connect(ep);

            byte[] ntpData = new byte[48]; // RFC 2030 
            ntpData[0] = 0x1B;
            for (int i = 1; i < 48; i++)
                ntpData[i] = 0;

            s.Send(ntpData);
            s.Receive(ntpData);

            byte offsetTransmitTime = 40;
            ulong intpart = 0;
            ulong fractpart = 0;

            for (int i = 0; i <= 3; i++)
                intpart = 256 * intpart + ntpData[offsetTransmitTime + i];

            for (int i = 4; i <= 7; i++)
                fractpart = 256 * fractpart + ntpData[offsetTransmitTime + i];

            var milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);
            s.Close();

            var timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
            var dateTime = new DateTime(1900, 1, 1);

            dateTime += timeSpan;

            var offsetAmount = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
            var networkDateTime = (dateTime + offsetAmount);
            return networkDateTime;
        }
    }
}