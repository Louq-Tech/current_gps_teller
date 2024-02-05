namespace Current_GPS_Teller
{
    public class PermissionsManager
    {
        LocationManager locationManager = new LocationManager();
        public async Task CheckAndRequestPermissionsInvoker()
        {
            await locationManager.CheckAndRequestLocationPermission();
        }
    }
}
