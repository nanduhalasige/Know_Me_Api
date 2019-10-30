using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;

namespace Know_Me_Api.Utility
{
    public class AccurateClock : Google.Apis.Util.IClock
    {
        const int UpdateIntervalMinutes = 60;
        const string NntpServer = "time.nist.gov";

        private TimeSpan _timeOffset;
        private DateTime _lastChecked;

        public AccurateClock()
        {
            _timeOffset = TimeSpan.FromSeconds(0);
            _lastChecked = DateTime.MinValue;
        }

        private DateTime GetTime()
        {
            try
            {
                if (DateTime.Now.Subtract(_lastChecked).TotalMinutes >= UpdateIntervalMinutes)
                {
                    // Update offset 
                    var client = new TcpClient(NntpServer, 13);
                    DateTime serverTime;
                    using (var streamReader = new StreamReader(client.GetStream()))
                    {
                        var response = streamReader.ReadToEnd();
                        var utcDateTimeString = response.Substring(7, 17);
                        serverTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    }
                    _timeOffset = DateTime.UtcNow.Subtract(serverTime);
                    _lastChecked = DateTime.Now;
                }
                var accurateTime = DateTime.UtcNow.Subtract(_timeOffset);
                return accurateTime;
            }
            catch (Exception ex)
            {
                return DateTime.UtcNow;
            }

        }

        public DateTime Now
        {
            get
            {
                return GetTime().ToLocalTime();
            }
        }

        public DateTime UtcNow
        {
            get
            {

                return GetTime();
            }
        }
    }
}
