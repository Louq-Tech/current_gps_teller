// Bug: Line 30, issues with awaiting to receive processed location for card -- State: Commented out

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace Current_GPS_Teller
{
    public class RecentlyAddedLocationView : ContentView
    {
        private HorizontalStackLayout cardActions;
        private ImageButton undoButton;
        private ImageButton menuButton;

        public string LocationName { get; set; } = "N/A";
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
        public string Time { get; set; } = "N/A";

        public async Task<ContentView> BuildCard()
        {
            WebsocketConnection connection = new WebsocketConnection();
            await connection.WebsocketConnectingFunction();

            byte[] receivingProcessedLocationMessage = new byte[1024];

            // Bug below, await makes the cards to not show => To be worked on soon
            //await connection.Client.ReceiveAsync(new ArraySegment<byte>(receivingProcessedLocationMessage), CancellationToken.None);

            // Temporary replacement, to be removed when the problem with location receiving is worked on
            connection.Client.ReceiveAsync(new ArraySegment<byte>(receivingProcessedLocationMessage), CancellationToken.None);

            string processedLocationString = Encoding.UTF8.GetString(receivingProcessedLocationMessage);

            MainPageModel mainPageModel = await Task.Run(() => JsonConvert.DeserializeObject<MainPageModel>(processedLocationString));

            //LocationName = mainPageModel.LocationName;
            //Latitude = mainPageModel.Latitude;
            //Longitude = mainPageModel.Longitude;
            //Time = mainPageModel.Time;

            var locationHeader = new Label
            {
                Text = $"{LocationName}",
                FontAttributes = FontAttributes.Bold,
                FontSize = 20
            };

            var latitude = new Label
            {
                Text = $"Latitude: {Latitude}",
            };

            var longitude = new Label
            {
                Text = $"Longitude: {Longitude}",
            };

            menuButton = new ImageButton
            {
                Source = "menu.png",
                WidthRequest = 20,
                HeightRequest = 20,
                CornerRadius = 5,
            };
            menuButton.Clicked += CardActionShower;

            undoButton = new ImageButton
            {
                Source = "undo.png",
                WidthRequest = 18,
                HeightRequest = 18,
                CornerRadius = 5,
            };
            undoButton.Clicked += UndoAction;

            var deleteButton = new ImageButton
            {
                Source = "trash.png",
                WidthRequest = 18,
                HeightRequest = 18,
                CornerRadius = 5,
            };
            deleteButton.Clicked += DeleteAction;


            cardActions = new HorizontalStackLayout
            {
                Children = { undoButton, deleteButton },
                Spacing = 20,
                IsVisible = false,
            };

            var time = new Label
            {
                Text = $"{Time}",
                TextColor = Color.FromArgb("#B5B5B5"),
                HorizontalTextAlignment = TextAlignment.End,
            };

            var grid = new Grid
            {
                Padding = 15,
                RowSpacing = 6,

                RowDefinitions =
                {
                    new RowDefinition{ Height = GridLength.Auto},
                    new RowDefinition{ Height = GridLength.Auto},
                    new RowDefinition{ Height = GridLength.Auto},
                },

                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star},
                    new ColumnDefinition{ Width = GridLength.Auto },
                }
            };

            grid.SetRow(locationHeader, 0);
            grid.SetColumn(locationHeader, 0);
            grid.Children.Add(locationHeader);

            grid.SetRow(latitude, 1);
            grid.SetColumn(latitude, 0);
            grid.Children.Add(latitude);

            grid.SetRow(longitude, 2);
            grid.SetColumn(longitude, 0);
            grid.Children.Add(longitude);

            grid.SetRow(menuButton, 0);
            grid.SetColumn(menuButton, 2);
            grid.Children.Add(menuButton);

            grid.SetRow(cardActions, 0);
            grid.SetColumn(cardActions, 2);
            grid.Children.Add(cardActions);

            grid.SetRow(time, 3);
            grid.SetColumn(time, 2);
            grid.Children.Add(time);

            var border = new Border
            {
                BackgroundColor = Color.FromArgb("#FF333333"),
                HeightRequest = 100,
                Margin = new Thickness(25, 0, 25, 10),
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(15)
                },
                Content = grid,
            };

            Content = border;

            return this;
        }

        private async void CardActionShower(object sender, EventArgs e)
        {
            cardActions.IsVisible = true;
            menuButton.IsVisible = false;

            await Task.Delay(5000);

            (sender as ImageButton).BackgroundColor = Colors.Transparent;
        }

        // Event handler for undoButton
        private void UndoAction(object sender, EventArgs e)
        {
            cardActions.IsVisible = false;
            menuButton.IsVisible = true;
        }

        // Event handler for deleteButton
        private void DeleteAction(object sender, EventArgs e)
        {
            Toast.Make("Delete not yet implemented", ToastDuration.Long, 10).Show();
        }
    }
}
