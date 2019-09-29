using Nett;

namespace BatchGasphacker.Configs.Utils
{
    public static class ConfigLoader
    {
        public const string DefaultFilename = "config.toml";

        public static Config LoadConfig(string filename)
        {
            return Toml.ReadFile<Config>(filename);
        }
    }
}
