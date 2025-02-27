using System.Windows;
using Dapper;
using Microsoft.Data.SqlClient;

namespace TemplateProject;

public partial class EditWindow : Window
{
    private readonly AppSettings _appSettings = AppSettingsReader.ReadAppSettings();
    public Person _person { get; private set; }
    
    public EditWindow(Person person)
    {
        InitializeComponent();
        _person = person;

        NameInput.Text = _person.Name;
        AgeInput.Text = _person.Age.ToString();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameInput.Text))
        {
            MessageBox.Show("Please enter a valid name", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(AgeInput.Text, out int age))
        {
            MessageBox.Show("Please enter a valid age", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _person = new Person(_person.Id, NameInput.Text, int.Parse(AgeInput.Text));
        using (var conn = new SqlConnection(_appSettings.DbSettings.ConnectionString))
        {
            conn.Open();
            string sql = "UPDATE Person SET Name = @Name, Age = @Age WHERE Id = @Id";
            conn.Execute(sql, new { _person.Name, _person.Age, _person.Id });
        }
        Console.WriteLine($"Edited Person: {_person.Id}");
        
        DialogResult = true;
        Close();
    }
    
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}