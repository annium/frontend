using System;
using Annium.Core.Mapper.Attributes;
using Annium.Testing;
using Xunit;

namespace Annium.Blazor.Routing.Tests;

public class RouteTTest : TestBase
{
    public RouteTTest(ITestOutputHelper outputHelper)
        : base(outputHelper) { }

    [Fact]
    public void Link_Works()
    {
        // arrange
        var route = GetRouting<Routing>().Search;

        // assert
        route
            .Link(
                new SearchData
                {
                    Sex = Sex.Male,
                    Name = ["alex", "anna"],
                    Age = 30,
                }
            )
            .Is("pages/search/Male?name=alex&name=anna&age=30");
    }

    [Fact]
    public void Go_Works()
    {
        // arrange
        var route = GetRouting<Routing>().Search;

        // act
        route.Go(
            new SearchData
            {
                Sex = Sex.Male,
                Name = ["alex", "anna"],
                Age = 30,
            }
        );

        // assert
        NavigationManager.Locations.At(0).Is("pages/search/Male?name=alex&name=anna&age=30");
    }

    [Fact]
    public void IsAt_Works()
    {
        // arrange
        var route = GetRouting<Routing>().Search;

        // assert
        NavigationManager.NavigateTo("pages/search/Male?name=alex&name=anna&age=30");
        route.IsAt().IsTrue();
        route.IsAt(new SearchData { Sex = Sex.Male }).IsTrue();
        route.IsAt(new SearchData { Sex = Sex.Female }).IsFalse();
        route
            .IsAt(
                new SearchData
                {
                    Sex = Sex.Male,
                    Name = ["alex", "anna"],
                    Age = 30,
                }
            )
            .IsTrue();
        route.IsAt(new SearchData { Sex = Sex.Male, Name = ["alex", "ann"] }).IsFalse();
        NavigationManager.NavigateTo("pages");
        route.IsAt().IsFalse();
        route.IsAt(null, PathMatch.Start).IsTrue();
    }

    [Fact]
    public void GetParams_Works()
    {
        // arrange
        var route = GetRouting<Routing>().Search;

        // assert
        NavigationManager.NavigateTo("pages/search/Male?name=alex&name=anna&age=30");
        route
            .GetParams()
            .IsEqual(
                new SearchData
                {
                    Sex = Sex.Male,
                    Name = ["alex", "anna"],
                    Age = 30,
                }
            );
        NavigationManager.NavigateTo("profile/Male?name=alex&name=anna&age=30");
        route.TryGetParams(out _).IsFalse();
    }

    public class Routing : IRouting
    {
        public IRoute<SearchData> Search { get; }

        public Routing(IRouteFactory routeFactory)
        {
            Search = routeFactory.Create<SearchPage, SearchData>("/pages/search/{sex}");
        }
    }

    public class SearchPage { }

    public sealed record SearchData
    {
        public Sex Sex { get; init; }
        public string[] Name { get; init; } = [];
        public int Age { get; init; }

        public bool Equals(SearchData? other) => other is not null && GetHashCode() == other.GetHashCode();

        public override int GetHashCode() => HashCode.Combine((int)Sex, HashCodeSeq.Combine(Name), Age);
    }

    [AutoMapped]
    public enum Sex
    {
        Male,
        Female,
    }
}
