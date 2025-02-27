using System.Text;
using System.Windows;
using Dapper;
using Microsoft.Data.SqlClient;


namespace TemplateProject;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private readonly AppSettings _appSettings = AppSettingsReader.ReadAppSettings();
    
    private void ProcessInput_Click(object sender, RoutedEventArgs e)
    {
        using (var conn = new SqlConnection(_appSettings.DbSettings.ConnectionString))
        {
            conn.Open();
            conn.Query(@"INSERT INTO Person (Id, Name, Age) VALUES
                                (NEWID(), @Name, @Age)", new {Name = InputField1.Text, Age = InputField2.Text});

        }
        
        Console.WriteLine($"Writing to DB: Name = {InputField1.Text}, Age = {InputField2.Text}");
    }
}