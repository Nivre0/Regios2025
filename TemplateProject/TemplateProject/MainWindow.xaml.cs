using System.Text;
using System.Windows;
using System.Windows.Navigation;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;


namespace TemplateProject;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly AppSettings _appSettings = AppSettingsReader.ReadAppSettings();

    public MainWindow()
    {
        InitializeComponent();
        LoadPersons();
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        string name = NameInput.Text;
        if (name.IsNullOrEmpty())
        {
            MessageBox.Show("Please enter a valid name", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(AgeInput.Text, out int age))
        {
            MessageBox.Show("Please enter a valid age.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        AddPerson(name, age);
        LoadPersons(); 
    }
    
    private void LoadPersons()
    {
        using (var conn = new SqlConnection(_appSettings.DbSettings.ConnectionString))
        {
            conn.Open();
            string sql = "SELECT Id, Name, Age FROM Person";
            List<Person> persons = conn.Query<Person>(sql).AsList();
            PersonDataGrid.ItemsSource = persons; 
        }
    }
    
    private void AddPerson(string name, int age)
    {
        using (var conn = new SqlConnection(_appSettings.DbSettings.ConnectionString))
        {
            conn.Open();
            string sql = "INSERT INTO Person (Id, Name, Age) VALUES (NEWID(), @name, @age)";
            conn.Execute(sql, new { name, age });
        }

        NameInput.Text = "";
        AgeInput.Text = "";
    }
}