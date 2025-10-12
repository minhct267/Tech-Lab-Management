using TechLabManagement.Core.Interfaces;
using TechLabManagement.Core.Models;

namespace TechLabManagement.Services.Induction;

public sealed class InductionEvaluator : IInductionEvaluator
{
	public int EvaluateScore(InductionTest test, IList<int> selectedOptionIndexes)
	{
		if (selectedOptionIndexes.Count != test.Questions.Count)
			throw new ArgumentException("Answer count mismatch");
		int correct = 0;
		for (int i = 0; i < test.Questions.Count; i++)
		{
			if (test.Questions[i].CorrectOptionIndex == selectedOptionIndexes[i]) correct++;
		}
		return (int)Math.Round(100.0 * correct / Math.Max(1, test.Questions.Count));
	}

	public bool IsPass(int score, int thresholdPercentage) => score >= thresholdPercentage;
}


