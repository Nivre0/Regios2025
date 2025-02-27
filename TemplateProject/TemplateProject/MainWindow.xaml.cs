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
            Console.WriteLine($"Loaded {persons.Count} persons from DB.");
            // Ensure UI update on the main thread

            Dispatcher.Invoke(() =>
            {
                PersonDataGrid.ItemsSource = null; 
                PersonDataGrid.Items.Clear();      
                PersonDataGrid.ItemsSource = persons; 
            });

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
    
    private void DeletePerson_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.Tag is String personId)
        {
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete this person?", "Confirm Deletion",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var conn = new SqlConnection(_appSettings.DbSettings.ConnectionString))
                    {
                        conn.Open();
                        string sql = "DELETE FROM Person WHERE Id = @Id";
                        conn.Execute(sql, new { Id = personId });
                    }

                    Console.WriteLine($"Deleted Person: {personId}");

                    LoadPersons(); // Refresh list after deletion
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting person: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void EditPerson_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.Tag is String personId)
        {

            using (var conn = new SqlConnection(_appSettings.DbSettings.ConnectionString))
            {
                conn.Open();
                string sql = "SELECT Id, Name, Age FROM Person WHERE Id = @Id";
                var person = conn.QuerySingleOrDefault<Person>(sql, new { Id = personId });

                if (person == null)
                {
                    MessageBox.Show("Person not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var editWindow = new EditWindow(person);
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    LoadPersons(); // Refresh the list after editing
                }
            }
        }
    }
}