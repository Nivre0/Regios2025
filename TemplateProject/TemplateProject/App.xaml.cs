using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace TemplateProject;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (!CanConnectToDb("Server=(localdb)\\MSSQLLocalDB;Database=TestDB;"))
        {
            MessageBox.Show(
                "Cannot connect to the database. Make sure to follow the instructions in the README file. The Application will now close.",
                "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }

    private bool CanConnectToDb(string connString)
    {
        try
        {
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Database connection failed:\n{e.Message}");
            return false;
        }
    }
}