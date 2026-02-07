using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DS4WinWPF.Infraestructura
{
    public class FtpManager
    {
        private readonly string _ftpServer;
        private readonly string _username;
        private readonly string _password;
        private readonly AESCrypto _crypto;

        public FtpManager(string ftpServer, string username, string password)
        {
            _ftpServer = ftpServer;
            _username = username;
            _password = password;
            _crypto = new AESCrypto();
        }

        public bool UploadProfileContent(string content, string fileName)
        {
            try
            {
                string remoteFileName = fileName.EndsWith(".xml") ? fileName.Replace(".xml", ".cif") : fileName;
                string ftpUrl = $"{_ftpServer}/{remoteFileName}";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;
                request.UseBinary = true;

                string encryptedContent = _crypto.Encrypt(content);
                byte[] encryptedData = System.Text.Encoding.UTF8.GetBytes(encryptedContent);
                request.ContentLength = encryptedData.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(encryptedData, 0, encryptedData.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == FtpStatusCode.ClosingData;
                }
            }
            catch
            {
                return false;
            }
        }

        public string DownloadProfileContent(string fileName)
        {
            try
            {
                string remoteFileName = fileName.EndsWith(".xml") ? fileName.Replace(".xml", ".cif") : fileName;
                string ftpUrl = $"{_ftpServer}/{remoteFileName}";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;
                request.UseBinary = true;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                {
                    string encryptedContent = reader.ReadToEnd();
                    return _crypto.Decrypt(encryptedContent);
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UploadProfileAsync(string localFilePath, string remoteFileName = null)
        {
            try
            {
                if (!File.Exists(localFilePath))
                    return false;

                string fileName = remoteFileName ?? Path.GetFileName(localFilePath);
                string finalFileName = fileName.EndsWith(".xml") ? fileName.Replace(".xml", ".cif") : fileName;
                string ftpUrl = $"{_ftpServer}/{finalFileName}";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;
                request.UseBinary = true;

                string originalContent = await File.ReadAllTextAsync(localFilePath);
                string encryptedContent = _crypto.Encrypt(originalContent);
                byte[] encryptedData = System.Text.Encoding.UTF8.GetBytes(encryptedContent);
                request.ContentLength = encryptedData.Length;

                using (Stream requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(encryptedData, 0, encryptedData.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    return response.StatusCode == FtpStatusCode.ClosingData;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool UploadProfile(string localFilePath, string remoteFileName = null)
        {
            try
            {
                if (!File.Exists(localFilePath))
                    return false;

                string fileName = remoteFileName ?? Path.GetFileName(localFilePath);
                string finalFileName = fileName.EndsWith(".xml") ? fileName.Replace(".xml", ".cif") : fileName;
                string ftpUrl = $"{_ftpServer}/{finalFileName}";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;
                request.UseBinary = true;

                string originalContent = File.ReadAllText(localFilePath);
                string encryptedContent = _crypto.Encrypt(originalContent);
                byte[] encryptedData = System.Text.Encoding.UTF8.GetBytes(encryptedContent);
                request.ContentLength = encryptedData.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(encryptedData, 0, encryptedData.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == FtpStatusCode.ClosingData;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DownloadProfile(string remoteFileName, string localFilePath)
        {
            try
            {
                string finalRemoteFileName = remoteFileName.EndsWith(".xml") ? remoteFileName.Replace(".xml", ".cif") : remoteFileName;
                string ftpUrl = $"{_ftpServer}/{finalRemoteFileName}";

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;
                request.UseBinary = true;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                {
                    string encryptedContent = reader.ReadToEnd();
                    string decryptedContent = _crypto.Decrypt(encryptedContent);
                    File.WriteAllText(localFilePath, decryptedContent);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<string> ListProfileFiles()
        {
            var files = new List<string>();
            try
            {
                string ftpUrl = $"{_ftpServer}/Profiles/";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(_username, _password);
                request.UsePassive = true;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        files.Add(line);
                    }
                }
            }
            catch
            {
                // Return empty list on error
            }
            return files;
        }
    }
}