namespace WhooshDI
{
    public interface IWhooshContainer
    {
        T Resolve<T>();
        T Resolve<T>(int name);
        T Resolve<T>(string name);
    }
}