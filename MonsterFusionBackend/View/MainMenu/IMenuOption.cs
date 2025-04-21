using System.Threading.Tasks;

namespace MonsterFusionBackend.View
{
    internal interface IMenuOption
    {
        string Name { get; }
        Task Start();
    }
}
