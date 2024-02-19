using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;

namespace Current_GPS_Teller
{
    public class MainPageBinder : BindableObject
    {
        public ObservableCollection<ContentView> LocationCards { get; set; } = new ObservableCollection<ContentView>();
        public ICommand ShutdownCommand { get; }
        public ICommand AddLocationCommand { get; }
        public string From { get; }
        public string To { get; }

        private string _temperature;
        public string Temperature
        {
            get => _temperature;
            set
            {
                if (_temperature != value)
                {
                    _temperature = value;
                    OnPropertyChanged(nameof(Temperature));
                }
            }
        }

        private string _batteryPercentage;
        public string BatteryPercentage
        {
            get => _batteryPercentage;
            set
            {
                if (_batteryPercentage != value)
                {
                    _batteryPercentage = value;
                    OnPropertyChanged(nameof(BatteryPercentage));
                }
            }
        }

        private bool _activityVisibility;
        public bool ActivityVisibility
        {
            get => _activityVisibility;
            set
            {
                if (_activityVisibility != value)
                {
                    _activityVisibility = value;
                    OnPropertyChanged(nameof(ActivityVisibility));
                }
            }
        }

        // Dealing with ActivityIndicator
        private bool _activityRunning;
        public bool ActivityRunning
        {
            get => _activityRunning;
            set
            {
                if (_activityRunning != value)
                {
                    _activityRunning = value;
                    OnPropertyChanged(nameof(ActivityRunning));
                }
            }
        }

        public MainPageBinder()
        {
            AddLocationCommand = new Command(LocationProcessing);
            ShutdownCommand = new Command(ShutdownProcessing);
            LocationCards = new ObservableCollection<ContentView>();

            // Check if server is ON to call ServerStatus() else to not call it
            ServerStatus();
        }

        public async void ShutdownProcessing()
        {

            bool confirmShutdown = await Application.Current.MainPage.DisplayAlert("Confirm Shutdown", "Are you sure you want to shutdown the server? All processes will be inaccessible", "Shutdown", "Go Back");

            if (confirmShutdown)
            {
                WebsocketConnection connection = new WebsocketConnection();
                await connection.WebsocketConnectingFunction();

                MainPageModel mainPageModel = new MainPageModel
                {
                    Type = "Shutdown"
                };

                string sendShutdown = JsonConvert.SerializeObject(mainPageModel);
                byte[] shutdownMessage = Encoding.UTF8.GetBytes(sendShutdown);
                await connection.Client.SendAsync(new ArraySegment<byte>(shutdownMessage), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        // The following codes are for receiving current server status
        public async void ServerStatus()
        {
            WebsocketConnection connection = new WebsocketConnection();
            try
            {
                await connection.WebsocketConnectingFunction();

                while (true)
                {
                    MainPageModel mainPageModelSender = new MainPageModel
                    {
                        Type = "Status"
                    };

                    string messageType = JsonConvert.SerializeObject(mainPageModelSender);
                    byte[] messageTypeSign = Encoding.UTF8.GetBytes(messageType);

                    try
                    {
                        await connection.Client.SendAsync(new ArraySegment<byte>(messageTypeSign), WebSocketMessageType.Text, true, CancellationToken.None);

                        byte[] serverStatusMessage = new byte[1024];
                        WebSocketReceiveResult webSocketReceiveResult = await connection.Client.ReceiveAsync(new ArraySegment<byte>(serverStatusMessage), CancellationToken.None);
                        string serverStatusMessageString = Encoding.UTF8.GetString(serverStatusMessage);

                        MainPageModel mainPageModel = await Task.Run(() => JsonConvert.DeserializeObject<MainPageModel>(serverStatusMessageString));

                        Temperature = mainPageModel.Temperature;
                        BatteryPercentage = mainPageModel.BatteryPercentage;
                    }
                    catch (WebSocketException ex)
                    {
                        // Handle WebSocket exceptions, such as when the server is offline
                        Console.WriteLine($"WebSocket error: {ex.Message}");
                        // You may want to break out of the loop or retry the connection here
                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(20));
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions that occur during the connection attempt
                Console.WriteLine($"Failed to connect to the WebSocket server: {ex.Message}");
            }
        }

        // The following codes are for sending current location to the server
        public async void LocationProcessing()
        {

            ActivityVisibility = true;
            ActivityRunning = true;

            // File name can't show
            string fileName = $"{From}_{To}";

            PermissionsManager permissionsManager = new PermissionsManager();
            await permissionsManager.CheckAndRequestPermissionsInvoker();

            var location = await Geolocation.GetLocationAsync();

            if (location != null && !location.IsFromMockProvider)
            {
                try
                {

                    WebsocketConnection connection = new WebsocketConnection();
                    await connection.WebsocketConnectingFunction();

                    MainPageModel mainPageModel = new MainPageModel
                    {
                        Type = "Location",
                        FileName = fileName,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                    };

                    string coordinatesInformations = JsonConvert.SerializeObject(mainPageModel);

                    byte[] coordinatesInformationMessage = Encoding.UTF8.GetBytes(coordinatesInformations);
                    await connection.Client.SendAsync(new ArraySegment<byte>(coordinatesInformationMessage), WebSocketMessageType.Text, true, CancellationToken.None);

                    RecentlyAddedLocationView recentlyAddedLocationView = new RecentlyAddedLocationView();
                    var newLocation = await recentlyAddedLocationView.BuildCard();

                    ActivityVisibility = false;
                    ActivityRunning = false;

                    // I have to deal with the list of only five cards, the 5th card should be deleted once 6th is added
                    if(LocationCards.Count >= 5) {
                        LocationCards.RemoveAt(4);
                        LocationCards.Insert(0, newLocation);
                    }

                    else
                    {
                        LocationCards.Insert(0, newLocation);
                    }
                }

                catch (Exception ex)
                {
                    Toast.Make($"{ex}", CommunityToolkit.Maui.Core.ToastDuration.Long, 10);
                    ActivityVisibility = false;
                    ActivityRunning = false;
                }
            }
        }
    }
}
