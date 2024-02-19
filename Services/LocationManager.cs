using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Current_GPS_Teller
{
    public class LocationManager
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public async Task GetLocation()
        {
            try
            {
                await CheckAndRequestLocationPermission();

                var location = await Geolocation.GetLocationAsync();

                if (location != null && !location.IsFromMockProvider)
                {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                }

                else
                {
                    await Application.Current.MainPage.DisplayAlert("Location Services", "Unable to retrieve location. Please ensure location services are enabled and not using a mock location.", "OK");
                }
            }

            catch (Microsoft.Maui.ApplicationModel.FeatureNotSupportedException)
            {
                await Application.Current.MainPage.DisplayAlert("Location Services", "Location services are not supported on this device.", "OK");
            }

            catch (Microsoft.Maui.ApplicationModel.FeatureNotEnabledException)
            {
                await Application.Current.MainPage.DisplayAlert("Location Services", "Location services are not enabled on this device.", "OK");
            }

            catch (Microsoft.Maui.ApplicationModel.PermissionException)
            {
                await Application.Current.MainPage.DisplayAlert("Location Services", "Location permission not granted.", "OK");
            }

            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Location Services", $"An error occurred: {ex.Message}", "OK");
            }
        }

        public async Task CheckAndRequestLocationPermission()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
        }
    }
}
