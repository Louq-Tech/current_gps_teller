namespace Current_GPS_Teller
{
    public partial class MainPage : ContentPage
    {

        PermissionsManager permissionsManager = new PermissionsManager();

        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainPageBinder();

            // Location Permission
            PermissionsAsync();

        }

        public async void PermissionsAsync()
        {
            await permissionsManager.CheckAndRequestPermissionsInvoker();
        }
    }
}