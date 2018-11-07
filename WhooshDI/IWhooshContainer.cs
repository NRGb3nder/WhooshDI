namespace WhooshDI
{
    public interface IWhooshContainer
    {
        T Resolve<T>();
        T Resolve<T>(object name);
    }
}