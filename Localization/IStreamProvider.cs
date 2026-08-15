using System.IO;

namespace PaintTrek.Shared.Localization
{
    /// <summary>
    /// Platform bağımsız olarak yerelleştirme (JSON) dosyalarını okumayı sağlayan arayüz.
    /// MonoGame/XNA bağımlılığını kesmek (decoupling) için kullanılır.
    /// </summary>
    public interface IStreamProvider
    {
        Stream Open(string path);
    }
}
