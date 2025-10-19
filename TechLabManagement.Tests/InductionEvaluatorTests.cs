using TechLabManagement.Core.Models;
using TechLabManagement.Services.Induction;

namespace TechLabManagement.Tests;

public sealed class InductionEvaluatorTests
{
    [Test]
    public void EvaluateScore_AllCorrect_Returns100()
    {
        var test = new InductionTest
        {
            Questions = new()
            {
                new Question { Text = "Q1", Options = new() { "A", "B" }, CorrectOptionIndex = 0 },
                new Question { Text = "Q2", Options = new() { "A", "B" }, CorrectOptionIndex = 1 },
                new Question { Text = "Q3", Options = new() { "A", "B" }, CorrectOptionIndex = 0 },
            }
        };

        var evaluator = new InductionEvaluator();

        var score = evaluator.EvaluateScore(test, new List<int> { 0, 1, 0 });

        Assert.That(score, Is.EqualTo(100));
        Assert.That(evaluator.IsPass(score, 80), Is.True);
    }

    [Test]
    public void EvaluateScore_SomeCorrect_RoundsToNearestPercent()
    {
        var test = new InductionTest
        {
            Questions = new()
            {
                new Question { Text = "Q1", Options = new() { "A", "B" }, CorrectOptionIndex = 0 },
                new Question { Text = "Q2", Options = new() { "A", "B" }, CorrectOptionIndex = 1 },
                new Question { Text = "Q3", Options = new() { "A", "B" }, CorrectOptionIndex = 0 },
                new Question { Text = "Q4", Options = new() { "A", "B" }, CorrectOptionIndex = 1 },
            }
        };

        var evaluator = new InductionEvaluator();

        var score = evaluator.EvaluateScore(test, new List<int> { 0, 1, 1, 1 }); // 3/4 correct => 75%

        Assert.That(score, Is.EqualTo(75));
        Assert.That(evaluator.IsPass(score, 80), Is.False);
    }

    [Test]
    public void EvaluateScore_WrongAnswerCount_Throws()
    {
        var test = new InductionTest
        {
            Questions = new()
            {
                new Question { Text = "Q1", Options = new() { "A", "B" }, CorrectOptionIndex = 0 },
                new Question { Text = "Q2", Options = new() { "A", "B" }, CorrectOptionIndex = 1 },
            }
        };

        var evaluator = new InductionEvaluator();

        Assert.That(() => evaluator.EvaluateScore(test, new List<int> { 0 }), Throws.TypeOf<ArgumentException>());
    }
}


