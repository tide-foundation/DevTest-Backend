using System;
using System.Diagnostics;
using System.IO;

namespace IntegrationTest.Tools
{
    public class Openssl
    {
        private ProcessStartInfo _procInfo;

        public Openssl() {
            _procInfo = new ProcessStartInfo("openssl");
            _procInfo.RedirectStandardOutput = false;
            _procInfo.RedirectStandardInput = false;
            _procInfo.UseShellExecute = false;
            _procInfo.CreateNoWindow = true;
            _procInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _procInfo.WorkingDirectory = Path.GetTempPath();
        }

        public (string PrvKey, string PubKey) GenKey() {
            var prvKey = GetString("ecparam -name prime256v1 -genkey -noout");
            var pubKey = GetString(prvKey, "ec -pubout");
            
            return (prvKey, pubKey);
        }

        public string SignString(byte[] message, string prvKey)
            => Convert.ToBase64String(Sign(message, prvKey));

        public byte[] Sign(byte[] message, string prvKey) {
            var keyPath = Path.GetTempFileName();
            File.WriteAllText(keyPath, prvKey);
            return GetBytes(message, $"dgst -sha3-256 -sign {keyPath}");
        }

        public bool Verify(byte[] message, byte[] signature, string pubKey)
        {
            var keyPath = Path.GetTempFileName();
            File.WriteAllText(keyPath, pubKey);

            var signaturePath = Path.GetTempFileName();
            File.WriteAllBytes(signaturePath, signature);

            return 0 == GetExitCode(message, $"dgst -sha3-256 -verify {keyPath} -signature {signaturePath}");
        }

        private int GetExitCode(byte[] input, string args = "") =>
            GetExitCode(wrt => wrt.BaseStream.Write(input, 0, input.Length), args);

        private string GetString(string args = "") => GetOutput(rdr => rdr.ReadToEnd(), args);

        private byte[] GetBytes(string args = "") =>
            GetOutput(rdr => ReadAllBytes(rdr.BaseStream), args);

        private string GetString(string input, string args = "") =>
            GetOutput(wrt => wrt.Write(input), rdr => rdr.ReadToEnd(), args);

        private byte[] GetBytes(string input, string args = "") =>
            GetOutput(wrt => wrt.Write(input), rdr => ReadAllBytes(rdr.BaseStream), args);

        private string GetString(byte[] input, string args = "") =>
            GetOutput(wrt => wrt.BaseStream.Write(input, 0, input.Length), rdr => rdr.ReadToEnd(), args);

        private byte[] GetBytes(byte[] input, string args = "") =>
            GetOutput(wrt => wrt.BaseStream.Write(input, 0, input.Length), rdr => ReadAllBytes(rdr.BaseStream), args);

        private T GetOutput<T>(Action<StreamWriter> input, Func<StreamReader, T> output, string args = "") {
            _procInfo.RedirectStandardOutput = true;
            _procInfo.RedirectStandardInput = true;
            _procInfo.Arguments = args;
            using (var process = Process.Start(_procInfo))
            {
                using (process.StandardInput)
                {
                    input(process.StandardInput);
                }

                using (process.StandardOutput)
                {
                    return output(process.StandardOutput);
                }
            }
        }

        private T GetOutput<T>(Func<StreamReader, T> output, string args = "") {
            _procInfo.RedirectStandardOutput = true;
            _procInfo.RedirectStandardInput = false;
            _procInfo.Arguments = args;
            using (var process = Process.Start(_procInfo))
            {
                using (process.StandardOutput)
                {
                    return output(process.StandardOutput);
                }
            }
        }

        private int GetExitCode(Action<StreamWriter> input, string args = "") {
            _procInfo.RedirectStandardOutput = false;
            _procInfo.RedirectStandardInput = true;
            _procInfo.Arguments = args;
            using (var process = Process.Start(_procInfo))
            {
                using (process.StandardInput)
                {
                    input(process.StandardInput);
                }
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        private static byte[] ReadAllBytes(Stream stream) {
            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }
   }
}