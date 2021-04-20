
namespace Integration.Tests.TestFixture
{
    public class InMemoryFixture
    {
        public readonly InMemoryApi InMemoryApi;
        public readonly InMemoryIdentity InMemoryIdentity;

        public InMemoryFixture()
        {
            InMemoryApi = new InMemoryApi();
            InMemoryIdentity = new InMemoryIdentity();
        }


    }
}
