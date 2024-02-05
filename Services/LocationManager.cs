namespace Current_GPS_Teller
{
    public class LocationManager
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public async Task GetLocation()
        {
            await CheckAndRequestLocationPermission();

                var location = await Microsoft.Maui.Devices.Sensors.Geolocation.GetLocationAsync();

                if (location != null && !location.IsFromMockProvider)
                {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                }
        }

        public async Task CheckAndRequestLocationPermission()
        {
            Microsoft.Maui.ApplicationModel.PermissionStatus status = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<Microsoft.Maui.ApplicationModel.Permissions.LocationWhenInUse>();
            if (status != Microsoft.Maui.ApplicationModel.PermissionStatus.Granted)
            {
                await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<Microsoft.Maui.ApplicationModel.Permissions.LocationWhenInUse>();
            }
        }
    }
}
