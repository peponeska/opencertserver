namespace CertesSlim.Tests.Acme.Resource;

using CertesSlim.Acme.Resource;
using Xunit;

public class DirectoryMetaTests
{
    [Fact]
    public void CanGetSetProperties()
    {
        var data = new
        {
            Website = "http://certes.is.working",
            CaaIdentities = new[] { "caa1", "caa2" },
            ExternalAccountRequired = true,
            TermsOfService = "http://certes.is.working/tos"
        };

        var model = new DirectoryMeta(
            data.TermsOfService,
            data.Website,
            data.CaaIdentities,
            data.ExternalAccountRequired);

        Assert.Equal(data.TermsOfService, model.TermsOfService);
        Assert.Equal(data.Website, model.Website);
        Assert.Equal(data.CaaIdentities, model.CaaIdentities);
        Assert.Equal(data.ExternalAccountRequired, model.ExternalAccountRequired);
    }
}
