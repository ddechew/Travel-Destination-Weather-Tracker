using DestinationsApp.Services;
using System.Diagnostics;

namespace DestinationsApp
{
    public partial class App : Application
    {
        public static DatabaseService Database { get; private set; }

        public App()
        {
            InitializeComponent();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "destinations.db3");

            Database = new DatabaseService(dbPath);

            MainPage = new NavigationPage(new MainPage());

        }
    }
}
