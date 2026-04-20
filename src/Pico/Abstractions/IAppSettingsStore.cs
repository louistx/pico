namespace Pico.Abstractions;

public interface IAppSettingsStore
{
    AppSettings Load();

    void Save(AppSettings settings);
}
