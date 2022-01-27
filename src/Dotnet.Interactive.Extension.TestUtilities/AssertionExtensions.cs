using System.Diagnostics;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Server;

namespace Dotnet.Interactive.Extension.TestUtilities;

[DebuggerStepThrough]
public static class AssertionExtensions
{
    public static GenericCollectionAssertions<T> AllSatisfy<T>(
        this GenericCollectionAssertions<T> assertions,
        Action<T> assert)
    {
        using var _ = new AssertionScope();

        foreach (var item in assertions.Subject)
        {
            assert(item);
        }

        return assertions;
    }

    public static void BeEquivalentToRespectingRuntimeTypes<TExpectation>(
        this GenericCollectionAssertions<TExpectation> assertions,
        params object[] expectations)
    {
        assertions.BeEquivalentTo(expectations, o => o.RespectingRuntimeTypes());
    }

    public static void BeEquivalentToRespectingRuntimeTypes<TExpectation>(
        this ObjectAssertions assertions,
        TExpectation expectation,
        Func<EquivalencyAssertionOptions<TExpectation>, EquivalencyAssertionOptions<TExpectation>>? config = null)
    {
        assertions.BeEquivalentTo(expectation, o => config is { } ? config.Invoke(o).RespectingRuntimeTypes() : o.RespectingRuntimeTypes());
    }

    public static void BeJsonEquivalentTo<T>(this StringAssertions assertion, T expected)
    {
        var obj = System.Text.Json.JsonSerializer.Deserialize(assertion.Subject, expected!.GetType());
        obj.Should().BeEquivalentToRespectingRuntimeTypes(expected);
    }

    public static AndConstraint<GenericCollectionAssertions<T>> BeEquivalentSequenceTo<T>(
        this GenericCollectionAssertions<T> assertions,
        params object[] expectedValues)
    {
        var actualValues = assertions.Subject.ToArray();

        actualValues
            .Select(a => a?.GetType())
            .Should()
            .BeEquivalentTo(expectedValues.Select(e => e?.GetType()));

        using (new AssertionScope())
        {
            foreach (var tuple in actualValues
                         .Zip(expectedValues, (actual, expected) => (actual, expected))
                         .Where(t => t.expected is null || t.expected.GetType().GetProperties().Any()))
            {
                tuple.actual
                    .Should()
                    .BeEquivalentToRespectingRuntimeTypes(tuple.expected);
            }
        }

        return new AndConstraint<GenericCollectionAssertions<T>>(assertions);
    }

    public static AndConstraint<StringCollectionAssertions> BeEquivalentSequenceTo(
        this StringCollectionAssertions assertions,
        params string[] expectedValues)
    {
        return assertions.ContainInOrder(expectedValues).And.BeEquivalentTo(expectedValues);
    }

    public static AndWhichConstraint<ObjectAssertions, T> ContainSingle<T>(
        this GenericCollectionAssertions<KernelCommand> should,
        Func<T, bool>? where = null)
        where T : KernelCommand
    {
        T subject;

        if (where is null)
        {
            should.ContainSingle(e => e is T);

            subject = should.Subject
                .OfType<T>()
                .Single();
        }
        else
        {
            should.ContainSingle(e => e is T && where((T)e));

            subject = should.Subject
                .OfType<T>()
                .Where(where)
                .Single();
        }

        return new AndWhichConstraint<ObjectAssertions, T>(subject.Should(), subject);
    }

  
 
    public static AndWhichConstraint<ObjectAssertions, T> ContainSingle<T>(
        this GenericCollectionAssertions<KernelEvent> should,
        Func<T, bool>? where = null)
        where T : KernelEvent
    {
        T subject;

        if (where is null)
        {
            should.ContainSingle(e => e is T);

            subject = should.Subject
                .OfType<T>()
                .Single();
        }
        else
        {
            should.ContainSingle(e => e is T && where((T)e));

            subject = should.Subject
                .OfType<T>()
                .Where(where)
                .Single();
        }

        return new AndWhichConstraint<ObjectAssertions, T>(subject.Should(), subject);
    }

    public static AndWhichConstraint<ObjectAssertions, T> ContainSingle<T>(
        this GenericCollectionAssertions<IKernelEventEnvelope> should,
        Func<T, bool> where = null)
        where T : IKernelEventEnvelope
    {
        T subject;

        if (where is null)
        {
            should.ContainSingle(e => e is T);

            subject = should.Subject
                .OfType<T>()
                .Single();
        }
        else
        {
            should.ContainSingle(e => e is T && where((T)e));

            subject = should.Subject
                .OfType<T>()
                .Where(where)
                .Single();
        }

        return new AndWhichConstraint<ObjectAssertions, T>(subject.Should(), subject);
    }

    public static AndConstraint<GenericCollectionAssertions<KernelEvent>> NotContainErrors(
        this GenericCollectionAssertions<KernelEvent> should) =>
        should
            .NotContain(e => e is ErrorProduced)
            .And
            .NotContain(e => e is CommandFailed);

    public static AndWhichConstraint<ObjectAssertions, T> EventuallyContainSingle<T>(
        this GenericCollectionAssertions<KernelEvent> should,
        Func<T, bool>? where = null,
        int timeout = 3000)
        where T : KernelEvent
    {
        return Task.Run(async () =>
        {
            if (where is null)
            {
                where = _ => true;
            }

            var startTime = DateTime.UtcNow;
            var endTime = startTime + TimeSpan.FromMilliseconds(timeout);
            while (DateTime.UtcNow < endTime)
            {
                if (should.Subject.OfType<T>().Any(where))
                {
                    break;
                }

                await Task.Delay(200);
            }

            return should.ContainSingle(where);
        }).Result;
    }
}