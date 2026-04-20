using Pico.Abstractions;

namespace Pico.Tests.Support;

internal sealed class InMemorySettingsStore : IAppSettingsStore
{
    private AppSettings _settings;

    public InMemorySettingsStore(string? defaultFolderPath = null)
    {
        _settings = new AppSettings(defaultFolderPath);
    }

    public AppSettings Load() => _settings;

    public void Save(AppSettings settings)
    {
        _settings = settings;
    }
}
