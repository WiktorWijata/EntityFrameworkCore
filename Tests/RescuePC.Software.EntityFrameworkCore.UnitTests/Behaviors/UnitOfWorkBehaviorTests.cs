using MediatR;
using NSubstitute;
using RescuePC.Software.EntityFrameworkCore.Behaviors;

namespace RescuePC.Software.EntityFrameworkCore.Tests.Behaviors;

public class UnitOfWorkBehaviorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ShouldCallNext_AndSaveChanges()
    {
        var behavior = new UnitOfWorkBehavior<TestRequest, TestResponse, IUnitOfWork>(_unitOfWork);
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = _ => { nextCalled = true; return Task.FromResult(new TestResponse()); };

        await behavior.Handle(new TestRequest(), next, CancellationToken.None);

        Assert.True(nextCalled);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldReturnResponseFromNext()
    {
        var expected = new TestResponse();
        var behavior = new UnitOfWorkBehavior<TestRequest, TestResponse, IUnitOfWork>(_unitOfWork);
        RequestHandlerDelegate<TestResponse> next = _ => Task.FromResult(expected);

        var result = await behavior.Handle(new TestRequest(), next, CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_ShouldSaveChanges_AfterNext()
    {
        var callOrder = new List<string>();
        var behavior = new UnitOfWorkBehavior<TestRequest, TestResponse, IUnitOfWork>(_unitOfWork);
        RequestHandlerDelegate<TestResponse> next = _ => { callOrder.Add("next"); return Task.FromResult(new TestResponse()); };
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            callOrder.Add("save");
            return 1;
        });

        await behavior.Handle(new TestRequest(), next, CancellationToken.None);

        Assert.Equal(["next", "save"], callOrder);
    }

    private record TestRequest : IRequest<TestResponse>;
    private record TestResponse;
}
