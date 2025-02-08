using System.Collections;
using System.Text;

namespace RTCodingExercise.Monolithic.Services
{
    public class PlateLetterHelper
    {
        // RC: Put this constant here, as we've only got one and it'll save us creating a whole class for it.
        public const int MaxPlateLength = 7;

        private IDictionary<char, char> _digitsToLetters;
        private IDictionary<int, List<int[]>> _memoizedPermutations
            = new Dictionary<int, List<int[]>>();

        private static IDictionary<char, char> DefaultDigitsToLetters =>
            new Dictionary<char, char>
            {
                ['0'] = 'O',
                ['1'] = 'I',
                ['2'] = 'Z',
                ['3'] = 'E',
                ['4'] = 'A',
                ['5'] = 'S',
                ['6'] = 'G',
                ['7'] = 'T',
                ['8'] = 'B',
            };

        public PlateLetterHelper()
            : this(DefaultDigitsToLetters)
        {
        }

        public PlateLetterHelper(IDictionary<char, char> digitsToLetters)
        {
            _digitsToLetters = digitsToLetters;
        }

        public string GetLettersFromPlate(string plate)
        {
            const int LetterCount = 3;
            string result = string.Empty;

            for (int i = 0, numLetters = 0; i < plate.Length && numLetters < LetterCount; ++i)
            {
                var plateChar = plate[i];

                if (char.IsLetter(plateChar))
                {
                    result += plateChar;
                    numLetters++;
                } 
                else if (_digitsToLetters.TryGetValue(plateChar, out char digitAsLetter))
                {
                    result += digitAsLetter;
                    numLetters++;
                }
            }

            return result;
        }

        public IList<string> GetAllPossiblePlatesFromSearch(string? search)
        {
            var results = new List<string>();

            if (search != null)
            {
                var platerizedSearch = search.Replace(" ", "").ToUpper();
                results.Add(platerizedSearch);

                // RC: Allow a maximum number of substitutions as this is 2^n at the worst case
                // but we can't have combination that exceed the maximum length anyway (max 128 searches here)
                var replacementIndicesDigits = new List<(int, char)>();
                var plateLength = Math.Min(platerizedSearch.Length, MaxPlateLength);
                for (var i = 0; i < plateLength; ++i)
                {
                    var plateChar = platerizedSearch[i];
                    var foundKv = _digitsToLetters.FirstOrDefault(kv => kv.Value == plateChar);

                    if (!foundKv.Equals(default(KeyValuePair<char, char>)))
                    {
                        replacementIndicesDigits.Add((i, foundKv.Key));
                    }
                }

                // RC: Generate all sets of permutations between 0..N to index our character replacement list
                var indexPermuations = GetLetterPermuationIndices(replacementIndicesDigits.Count);

                // RC: Substitute our letters for numbers for every possible permutation
                foreach(var indexPermuation in indexPermuations)
                {
                    var newPlaterizedSearchBuilder = new StringBuilder(platerizedSearch);
                    var replacements = indexPermuation.Select(i => replacementIndicesDigits[i]);
                    
                    foreach (var replacement in replacements)
                    {
                        newPlaterizedSearchBuilder[replacement.Item1] = replacement.Item2;
                    }

                    results.Add(newPlaterizedSearchBuilder.ToString());
                }
            }

            return results;
        }

        private List<int[]> GetLetterPermuationIndices(int count)
        {
            var items = Enumerable.Range(0, count);
            List<int[]>? result;

            if (!_memoizedPermutations.TryGetValue(count, out _))
            {
                result = new List<int[]>();
                PermuteRecursive(Enumerable.Empty<int>(), items, result);

                _memoizedPermutations[count] = result;
            }
            else
            {
                result = _memoizedPermutations[count];
            }

            return result;
        }

        private void PermuteRecursive(IEnumerable<int> head, IEnumerable<int> tail, IList<int[]> result)
        {
            if (!tail.Any())
                return;
            
            var tailCount = tail.Count();

            for (var i = 0; i < tailCount; ++i)
            {
                // RC: Append is immutable (returns new IEnumerable<int>)
                var newHead = head.Append(tail.ElementAt(i));
                result.Add(newHead.ToArray());

                PermuteRecursive(newHead, tail.Skip(i+1), result);
            }
        }
    }
}
