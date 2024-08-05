using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace MapsWinForms
{
    public partial class Form1 : Form
    {
        private readonly SemaphoreSlim _updateLocationSemaphore = new SemaphoreSlim(1, 1);
        private readonly System.Timers.Timer _debounceTimer; // Use System.Timers.Timer

        private static string FolderPath => Path.Combine(Directory.GetCurrentDirectory(), "VPN");

        public Form1()
        {
            InitializeComponent();
            webView21.EnsureCoreWebView2Async(null);
            _debounceTimer = new System.Timers.Timer(500); // Debounce for 500ms
            _debounceTimer.Elapsed += DebounceUpdateLocation;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var vpnHosts = new[]
            {
                "CA149.vpnbook.com", "DE20.vpnbook.com", "FR200.vpnbook.com",
                "PL134.vpnbook.com", "UK205.vpnbook.com", "US1.vpnbook.com",
                "US2.vpnbook.com", "CA196.vpnbook.com", "DE220.vpnbook.com",
                "FR231.vpnbook.com", "PL140.vpnbook.com", "UK68.vpnbook.com"
            };

            comboBox1.Items.AddRange(vpnHosts);

            var geoData = await GetGeolocationDataAsync();
            await webView21.EnsureCoreWebView2Async(null);

            LoadMap(geoData);
        }

        private void LoadMap(dynamic geoData)
        {
            string htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Interactive Map</title>
                    <meta charset='utf-8' />
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
                    <style>#map {{ height: 100vh; }}</style>
                </head>
                <body>
                    <div id='map'></div>
                    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
                    <script>
                        var map = L.map('map').setView([{geoData.lat}, {geoData.lon}], 17.5);
                        var marker = L.marker([{geoData.lat}, {geoData.lon}]).addTo(map)
                            .bindPopup('You are in {geoData.city}')
                            .openPopup();
                        
                        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                            attribution: '&copy; OpenStreetMap contributors'
                        }}).addTo(map);

                        function panMap(dx, dy) {{
                            map.panBy([dx, dy], {{ animate: true }});
                        }}

                        function zoomMap(zoomFactor) {{
                            map.setZoom(map.getZoom() + zoomFactor);
                        }}

                        function updateMarkerPosition(lat, lng, city) {{
                            marker.setLatLng([lat, lng]);
                            marker.setPopupContent('You are in ' + city);
                            map.setView([lat, lng], 17.5);
                        }}
                    </script>
                </body>
                </html>";

            webView21.CoreWebView2.NavigateToString(htmlContent);
        }

        private async Task<dynamic> GetGeolocationDataAsync()
        {
            using var client = new HttpClient();
            var ipResponse = await client.GetStringAsync("https://api.ipify.org?format=json");
            var ipAddress = JObject.Parse(ipResponse)["ip"].ToString();

            var geoResponse = await client.GetStringAsync($"http://ip-api.com/json/{ipAddress}");
            return JObject.Parse(geoResponse);
        }

        private async Task<string> GetIPAddressAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync("https://api.ipify.org?format=json");
            return JObject.Parse(response)["ip"].ToString();
        }

        private async Task PanMap(int dx, int dy)
        {
            await webView21.CoreWebView2.ExecuteScriptAsync($"panMap({dx}, {dy})");
        }

        private async Task ZoomMap(int zoomFactor)
        {
            await webView21.CoreWebView2.ExecuteScriptAsync($"zoomMap({zoomFactor})");
        }

        private async void PanLeftButton_Click(object sender, EventArgs e)
        {
            await PanMap(-50, 0);
        }

        private async void PanRightButton_Click(object sender, EventArgs e)
        {
            await PanMap(50, 0);
        }

        private async void ZoomInButton_Click(object sender, EventArgs e)
        {
            await ZoomMap(1);
        }

        private async void ZoomOutButton_Click(object sender, EventArgs e)
        {
            await ZoomMap(-1);
        }

        private async void btnConnect_Click_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a VPN host.");
                return;
            }

            string host = comboBox1.SelectedItem.ToString();
            string username = txtUsrname.Text;
            string password = txtPassword.Text;

            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            var sb = new StringBuilder()
                .AppendLine("[VPN]")
                .AppendLine("MEDIA=rastapi")
                .AppendLine("Port=VPN2-0")
                .AppendLine("Device=WAN Miniport (IKEv2)")
                .AppendLine("DEVICE=vpn")
                .AppendLine("PhoneNumber=" + host);

            File.WriteAllText(Path.Combine(FolderPath, "VpnConnection.pbk"), sb.ToString());

            sb = new StringBuilder()
                .AppendLine($"rasdial \"VPN\" {username} {password} /phonebook:\"{Path.Combine(FolderPath, "VpnConnection.pbk")}\"");

            File.WriteAllText(Path.Combine(FolderPath, "VpnConnection.bat"), sb.ToString());

            var newProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(FolderPath, "VpnConnection.bat"),
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };

            newProcess.Start();
            newProcess.WaitForExit();

            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;

            await UpdateLocationAsync();
        }

        private async Task UpdateLocationAsync()
        {
            if (!_updateLocationSemaphore.Wait(0))
            {
                return; // Already updating, skip this request
            }

            try
            {
                string ipAddress = await GetIPAddressAsync();
                if (string.IsNullOrEmpty(ipAddress))
                {
                    MessageBox.Show("Failed to retrieve IP address.");
                    return;
                }

                var (lat, lng) = await GeocodeIPAsync(ipAddress);
                if (lat == 0 && lng == 0)
                {
                    MessageBox.Show("Failed to retrieve geolocation data.");
                    return;
                }

                var geoData = await GetGeolocationDataAsync();
                await UpdateMarkerPosition(lat, lng, geoData.city);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update the location: " + ex.Message);
            }
            finally
            {
                _updateLocationSemaphore.Release();
            }
        }

        private async Task UpdateMarkerPosition(double lat, double lng, string city)
        {
            var data = new { lat, lng, city };
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data).Replace("\"", "\\\"");

            var script = $@"
                if (typeof updateMarkerPosition === 'function') {{
                    var data = JSON.parse('{jsonData}');
                    updateMarkerPosition(data.lat, data.lng, data.city);
                }}";

            await webView21.CoreWebView2.ExecuteScriptAsync(script);
        }

        private async Task<(double lat, double lng)> GeocodeIPAsync(string ipAddress)
        {
            using var client = new HttpClient();
            try
            {
                var response = await client.GetStringAsync($"http://ip-api.com/json/{ipAddress}");
                var geoData = JObject.Parse(response);

                return ((double)geoData["lat"], (double)geoData["lon"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching geolocation data: " + ex.Message);
                return (0, 0);
            }
        }

        private void btnDisconnect_Click_1(object sender, EventArgs e)
        {
            File.WriteAllText(Path.Combine(FolderPath, "VpnDisconnect.bat"), "rasdial /d");

            var newProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(FolderPath, "VpnDisconnect.bat"),
                    WindowStyle = ProcessWindowStyle.Normal
                }
            };

            newProcess.Start();
            newProcess.WaitForExit();

            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
        }

        private void Update_IP_Click(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private async void DebounceUpdateLocation(object source, System.Timers.ElapsedEventArgs e)
        {
            _debounceTimer.Stop(); // Stop the timer to prevent re-entry
            await UpdateLocationAsync();
            var geoData = await GetGeolocationDataAsync();
            LoadMap(geoData); // Reset the map as if the application was exited and restarted
        }
    }
}
