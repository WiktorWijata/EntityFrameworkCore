using MediatR;
using NSubstitute;
using RescuePC.Software.EntityFrameworkCore.Behaviors;

namespace RescuePC.Software.EntityFrameworkCore.Tests.Behaviors;

public class StreamUnitOfWorkBehaviorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ShouldYieldAllItems_AndSaveChanges()
    {
        var behavior = new StreamUnitOfWorkBehavior<TestStreamRequest, int, IUnitOfWork>(_unitOfWork);

        var results = new List<int>();
        await foreach (var item in behavior.Handle(new TestStreamRequest(), () => Items(), CancellationToken.None))
            results.Add(item);

        Assert.Equal([1, 2, 3], results);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldSaveChanges_AfterAllItemsYielded()
    {
        var callOrder = new List<string>();
        var behavior = new StreamUnitOfWorkBehavior<TestStreamRequest, int, IUnitOfWork>(_unitOfWork);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(_ =>
        {
            callOrder.Add("save");
            return 1;
        });

        await foreach (var item in behavior.Handle(new TestStreamRequest(), () => OrderedItems(callOrder), CancellationToken.None))
        { }

        Assert.Equal(["item", "item", "save"], callOrder);
    }

    private static async IAsyncEnumerable<int> Items()
    {
        yield return 1;
        yield return 2;
        yield return 3;
        await Task.CompletedTask;
    }

    private static async IAsyncEnumerable<int> OrderedItems(List<string> callOrder)
    {
        callOrder.Add("item");
        yield return 1;
        callOrder.Add("item");
        yield return 2;
        await Task.CompletedTask;
    }

    private record TestStreamRequest : IStreamRequest<int>;
}
