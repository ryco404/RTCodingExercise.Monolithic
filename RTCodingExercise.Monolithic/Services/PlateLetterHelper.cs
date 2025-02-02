namespace RTCodingExercise.Monolithic.Services
{
    public class PlateLetterHelper
    {
        private IDictionary<char, char> _digitsToLetters;

        private static IDictionary<char, char> DefaultDigitsToLetters =>
            new Dictionary<char, char>
            {
                ['0'] = 'O',
                ['1'] = 'L',
                ['2'] = 'Z',
                ['3'] = 'E',
                ['4'] = 'A',
                ['5'] = 'S',
                ['6'] = 'B',
                ['7'] = 'T',
                ['8'] = 'B',
                ['9'] = 'G'
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
    }
}
