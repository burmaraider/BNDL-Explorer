using OpenTK;

namespace BNDL_Explorer.Viewer
{
    internal interface ILight
    {
        Vector3 Position { get; set; }
        Vector3 AmbientColor { get; set; }
        Vector3 Color { get; set; }
        Vector3 SpecularColor { get; set; }
        float Intensity { get; set; }
    }
}
