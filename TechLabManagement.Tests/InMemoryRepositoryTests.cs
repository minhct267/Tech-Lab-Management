using TechLabManagement.Core.Interfaces;
using TechLabManagement.Services.Repositories;

namespace TechLabManagement.Tests;

public sealed class InMemoryRepositoryTests
{
    private sealed class Dummy : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    [Test]
    public void Add_And_GetById_Works()
    {
        IRepository<Dummy> repo = new InMemoryRepository<Dummy>();
        var d = new Dummy { Name = "A", Value = 1 };
        repo.Add(d);

        var fetched = repo.GetById(d.Id);
        Assert.That(fetched, Is.Not.Null);
        Assert.That(fetched!.Name, Is.EqualTo("A"));
    }

    [Test]
    public void Update_ReplacesEntity()
    {
        IRepository<Dummy> repo = new InMemoryRepository<Dummy>();
        var d = repo.Add(new Dummy { Name = "A", Value = 1 });
        d.Name = "B";
        d.Value = 2;
        repo.Update(d);

        var fetched = repo.GetById(d.Id)!;
        Assert.That(fetched.Name, Is.EqualTo("B"));
        Assert.That(fetched.Value, Is.EqualTo(2));
    }

    [Test]
    public void Delete_RemovesEntity()
    {
        IRepository<Dummy> repo = new InMemoryRepository<Dummy>();
        var d = repo.Add(new Dummy { Name = "A", Value = 1 });

        var ok = repo.Delete(d.Id);
        Assert.That(ok, Is.True);
        Assert.That(repo.GetById(d.Id), Is.Null);
    }

    [Test]
    public void Query_FiltersCorrectly()
    {
        IRepository<Dummy> repo = new InMemoryRepository<Dummy>();
        repo.Add(new Dummy { Name = "A", Value = 1 });
        repo.Add(new Dummy { Name = "B", Value = 2 });
        repo.Add(new Dummy { Name = "C", Value = 3 });

        var results = repo.Query(x => x.Value >= 2).ToList();
        Assert.That(results.Select(r => r.Name), Is.EquivalentTo(new[] { "B", "C" }));
    }
}


