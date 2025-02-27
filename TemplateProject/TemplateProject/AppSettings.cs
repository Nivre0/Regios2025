namespace TemplateProject;

internal record DbSettings(string ConnectionString);

internal record AppSettings(DbSettings DbSettings);