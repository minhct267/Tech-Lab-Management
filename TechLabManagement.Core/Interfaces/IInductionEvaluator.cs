using TechLabManagement.Core.Models;

namespace TechLabManagement.Core.Interfaces;

public interface IInductionEvaluator
{
	int EvaluateScore(InductionTest test, IList<int> selectedOptionIndexes);
	bool IsPass(int score, int thresholdPercentage);
}


