using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools
{
    public interface IPluginable
    {
        void KeyPressed();
        void KeyReleased();
        void UpdateSettings(JObject payload);
        void OnTick();
        void Dispose();
    }
}
