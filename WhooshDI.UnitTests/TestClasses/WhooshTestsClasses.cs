namespace WhooshDI.UnitTests.TestClasses
{
    public class ParamlessCtorClass
    {
    }

    public class ParameterizedCtorClass
    {
        public ParamlessCtorClass ParamlessCtorClass { get; }
        
        public ParameterizedCtorClass(ParamlessCtorClass obj)
        {
            ParamlessCtorClass = obj;
        }
    }
    
    public class TransientImplConfig : WhooshConfiguration
    {
        public TransientImplConfig()
        {
            Register<ParamlessCtorClass>().AsTransient();
        }
    }
}