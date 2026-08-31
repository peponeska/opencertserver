namespace CertesSlim.Tests;

using CertesSlim.Acme.Resource;
using CertesSlim.Extensions;
using NSubstitute;
using Xunit;
using Directory = CertesSlim.Acme.Resource.Directory;

public class IAcmeContextExtensionsTests
{
    [Fact]
    public async Task CanGetTos()
    {
        const string tosUri = "http://acme.d/tos";
        var ctxMock = Substitute.For<IAcmeContext>();
        ctxMock.GetDirectory()
            .Returns(new Directory(null!, null!, null!, null!, null!, new DirectoryMeta(tosUri, null!, null, null)));
        Assert.Equal(tosUri, (await ctxMock.TermsOfService())?.AbsoluteUri);

        ctxMock.GetDirectory()
            .Returns(new Directory(null!, null!, null!, null!, null!, new DirectoryMeta(null!, null!, null, null)));
        Assert.Null(await ctxMock.TermsOfService());

        ctxMock.GetDirectory().Returns(new Directory(null!, null!, null!, null!, null!, null));
        Assert.Null(await ctxMock.TermsOfService());
    }
}
