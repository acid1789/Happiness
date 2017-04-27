using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using System.Windows.Forms;

namespace PatchLib
{
    public class WebRequest
    {
        public enum WebRequestStatus
        {
            PingingHost,

            DownloadingData,
            DownloadingData_NoPing,

            Download_Canceled,
            Download_Failed,
            Download_Complete
        }

        public enum RequestType
        {
            String,
            Data,
            File,
            UploadString
        }
                
        // Public members
        public WebRequestStatus Status;
        public bool PingSuccess;

        // Private members
        Ping _ping;
        WebClient _wc;
        string _url;
        string _resultString;
        byte[] _resultData;
        string _outputFile;
        RequestType _requestType;
        float _percentage;

        public WebRequest(string url, RequestType type, string outputFileName = null, bool doPing = false)
        {
            _url = url;
            _wc = new WebClient();
            _wc.DownloadProgressChanged += _wc_DownloadProgressChanged;
            _requestType = type;
            switch (type)
            {
                case RequestType.String: _wc.DownloadStringCompleted += _wc_DownloadStringCompleted; break;
                case RequestType.Data: _wc.DownloadDataCompleted += _wc_DownloadDataCompleted; break;
                case RequestType.File: _wc.DownloadFileCompleted += _wc_DownloadFileCompleted; _outputFile = outputFileName; break;
                case RequestType.UploadString: _wc.UploadStringCompleted += _wc_UploadStringCompleted; _outputFile = outputFileName; break;
            }

            if (doPing)
            {
                // send ping request
                try
                {
                    _ping = new Ping();
                    _ping.PingCompleted += _ping_PingCompleted;
                    string host = url.Substring(url.IndexOf("//") + 2);
                    host = host.Substring(0, host.IndexOf('/'));
                    IPHostEntry hostEntry = Dns.GetHostEntry(host);
                    _ping.SendAsync(hostEntry.AddressList[0], 3000, null);
                    Status = WebRequestStatus.PingingHost;
                }
                catch (Exception)
                {
                    // Ping failed for whatever reason, just skip it
                    BeginDownload();                   
                }

            }
            else
                BeginDownload();
        }

        public string ResultString { get { return _resultString; } }
        public float PercentComplete { get { return _percentage; } }

        public void Cancel()
        {
            if (Status < WebRequestStatus.Download_Canceled)
            {
                _wc.CancelAsync();
                WaitForResponse();
            }
        }

        public void WaitForPingComplete()
        {
            while (Status == WebRequestStatus.PingingHost)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
            }
        }

        public void WaitForResponse()
        {
            while (Status < WebRequestStatus.Download_Canceled)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
            }
        }

        public string WaitForResponseString()
        {
            WaitForResponse();
            return _resultString;
        }

        public byte[] WaitForResponseData()
        {
            WaitForResponse();
            return _resultData;
        }

        private void _ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            _ping = null;
            if (e.Error != null || e.Cancelled)
                PingSuccess = false;
            else
                PingSuccess = true;

            BeginDownload();
        }

        void BeginDownload()
        {
            Status = PingSuccess ? WebRequestStatus.DownloadingData : WebRequestStatus.DownloadingData_NoPing;
            switch (_requestType)
            {
                case RequestType.String: _wc.DownloadStringAsync(new Uri(_url), null); break;
                case RequestType.Data: _wc.DownloadDataAsync(new Uri(_url)); break;
                case RequestType.File:
                    {
                        string directory = Path.GetDirectoryName(_outputFile);
                        if( directory != null && directory.Length > 0 )
                            Directory.CreateDirectory(directory);
                        _wc.DownloadFileAsync(new Uri(_url), _outputFile);
                        break;
                    }
                case RequestType.UploadString:
                    {
                        _wc.Headers[HttpRequestHeader.Accept] = "application/json";
                        _wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                        _wc.UploadStringAsync(new Uri(_url), _outputFile);
                        break;
                    }

            }
        }
        private void _wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _percentage = e.ProgressPercentage;
        }

        private void _wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
                Status = WebRequestStatus.Download_Canceled;
            else if (e.Error != null)
            {
                Status = WebRequestStatus.Download_Failed;
                _resultString = e.Error.Message;
            }
            else
            {
                Status = WebRequestStatus.Download_Complete;
                _resultString = e.Result;
            }
        }

        private void _wc_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Cancelled)
                Status = WebRequestStatus.Download_Canceled;
            else if (e.Error != null)
            {
                Status = WebRequestStatus.Download_Failed;
                _resultString = e.Error.Message;
            }
            else
            {
                Status = WebRequestStatus.Download_Complete;
                _resultData = e.Result;
            }            
        }

        private void _wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                Status = WebRequestStatus.Download_Canceled;
            else if (e.Error != null)
            {
                Status = WebRequestStatus.Download_Failed;
                _resultString = e.Error.Message;
            }
            else
                Status = WebRequestStatus.Download_Complete;
        }

        private void _wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
                Status = WebRequestStatus.Download_Canceled;
            else if (e.Error != null)
            {
                Status = WebRequestStatus.Download_Failed;
                _resultString = e.Error.Message;
            }
            else
            {
                Status = WebRequestStatus.Download_Complete;
                _resultString = e.Result;
            }
        }

    }
}
